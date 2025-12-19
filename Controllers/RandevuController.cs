using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Data;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Controllers;

[Authorize]
public class RandevuController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RandevuController> _logger;

    public RandevuController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<RandevuController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    // GET: Randevu
    public async Task<IActionResult> Index()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var randevular = await _context.Randevular
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .Where(r => r.UyeId == user.Id)
                .OrderByDescending(r => r.RandevuTarihi)
                .ThenByDescending(r => r.BaslangicSaati)
                .ToListAsync();

            return View(randevular);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Randevular listelenirken hata oluştu: {ErrorMessage}", ex.Message);
            return View(new List<Randevu>());
        }
    }

    // GET: Randevu/Create
    public async Task<IActionResult> Create(int? antrenorId = null)
    {
        var antrenorler = await _context.Antrenorler
            .Select(a => new { a.Id, Ad = a.Ad + " " + a.Soyad })
            .ToListAsync();

        ViewData["AntrenorId"] = new SelectList(antrenorler, "Id", "Ad", antrenorId);
        ViewData["HizmetId"] = new SelectList(_context.Hizmetler, "Id", "Ad");

        if (antrenorId.HasValue)
        {
            ViewBag.SelectedAntrenorId = antrenorId.Value;
        }

        return View();
    }

    // POST: Randevu/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("RandevuTarihi,BaslangicSaati,AntrenorId,HizmetId,Notlar")] Randevu randevu)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        // Hizmet bilgilerini al
        var hizmet = await _context.Hizmetler.FindAsync(randevu.HizmetId);
        if (hizmet == null)
        {
            ModelState.AddModelError("", "Seçilen hizmet bulunamadı.");
            ViewData["AntrenorId"] = new SelectList(_context.Antrenorler, "Id", "Ad", randevu.AntrenorId);
            ViewData["HizmetId"] = new SelectList(_context.Hizmetler, "Id", "Ad", randevu.HizmetId);
            return View(randevu);
        }

        // Bitiş saatini hesapla
        randevu.BitisSaati = randevu.BaslangicSaati.Add(TimeSpan.FromMinutes(hizmet.Sure));
        randevu.ToplamUcret = hizmet.Ucret;
        randevu.UyeId = user.Id;
        randevu.Durum = "Beklemede";

        // Müsaitlik kontrolü
        var antrenor = await _context.Antrenorler
            .Include(a => a.Müsaitlikler)
            .FirstOrDefaultAsync(a => a.Id == randevu.AntrenorId);

        if (antrenor == null)
        {
            ModelState.AddModelError("", "Seçilen antrenör bulunamadı.");
            ViewData["AntrenorId"] = new SelectList(_context.Antrenorler, "Id", "Ad", randevu.AntrenorId);
            ViewData["HizmetId"] = new SelectList(_context.Hizmetler, "Id", "Ad", randevu.HizmetId);
            return View(randevu);
        }

        // Antrenörün o gün müsait olup olmadığını kontrol et
        var randevuGunu = randevu.RandevuTarihi.DayOfWeek;
        var musaitlik = antrenor.Müsaitlikler.FirstOrDefault(m => m.HaftaninGunu == randevuGunu);

        if (musaitlik == null)
        {
            ModelState.AddModelError("", "Antrenör bu gün müsait değil.");
            ViewData["AntrenorId"] = new SelectList(_context.Antrenorler, "Id", "Ad", randevu.AntrenorId);
            ViewData["HizmetId"] = new SelectList(_context.Hizmetler, "Id", "Ad", randevu.HizmetId);
            return View(randevu);
        }

        // Saat aralığı kontrolü
        if (randevu.BaslangicSaati < musaitlik.BaslangicSaati || randevu.BitisSaati > musaitlik.BitisSaati)
        {
            ModelState.AddModelError("", $"Antrenör bu saatlerde müsait değil. Müsait saatler: {musaitlik.BaslangicSaati:hh\\:mm} - {musaitlik.BitisSaati:hh\\:mm}");
            ViewData["AntrenorId"] = new SelectList(_context.Antrenorler, "Id", "Ad", randevu.AntrenorId);
            ViewData["HizmetId"] = new SelectList(_context.Hizmetler, "Id", "Ad", randevu.HizmetId);
            return View(randevu);
        }

        // Çakışma kontrolü - Aynı antrenör ve saatte başka randevu var mı?
        // SQLite TimeSpan karşılaştırmalarını desteklemediği için önce veritabanından çekip sonra memory'de filtreliyoruz
        var randevuTarihiDate = randevu.RandevuTarihi.Date;
        var potansiyelCakisanRandevular = await _context.Randevular
            .Where(r => r.AntrenorId == randevu.AntrenorId
                && r.RandevuTarihi.Date == randevuTarihiDate
                && r.Durum != "İptal")
            .ToListAsync();

        var cakisma = potansiyelCakisanRandevular
            .Any(r => (r.BaslangicSaati <= randevu.BaslangicSaati && r.BitisSaati > randevu.BaslangicSaati)
                    || (r.BaslangicSaati < randevu.BitisSaati && r.BitisSaati >= randevu.BitisSaati)
                    || (r.BaslangicSaati >= randevu.BaslangicSaati && r.BitisSaati <= randevu.BitisSaati));

        if (cakisma)
        {
            ModelState.AddModelError("", "Bu saatte antrenörün başka bir randevusu var. Lütfen farklı bir saat seçin.");
            ViewData["AntrenorId"] = new SelectList(_context.Antrenorler, "Id", "Ad", randevu.AntrenorId);
            ViewData["HizmetId"] = new SelectList(_context.Hizmetler, "Id", "Ad", randevu.HizmetId);
            return View(randevu);
        }

        if (ModelState.IsValid)
        {
            randevu.OlusturmaTarihi = DateTime.Now;
            _context.Add(randevu);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Yeni randevu oluşturuldu: {RandevuId}", randevu.Id);
            return RedirectToAction(nameof(Index));
        }

        ViewData["AntrenorId"] = new SelectList(_context.Antrenorler, "Id", "Ad", randevu.AntrenorId);
        ViewData["HizmetId"] = new SelectList(_context.Hizmetler, "Id", "Ad", randevu.HizmetId);
        return View(randevu);
    }

    // GET: Randevu/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var randevu = await _context.Randevular
            .Include(r => r.Antrenor)
            .Include(r => r.Hizmet)
            .Include(r => r.Uye)
            .FirstOrDefaultAsync(m => m.Id == id && m.UyeId == user.Id);

        if (randevu == null)
        {
            return NotFound();
        }

        return View(randevu);
    }

    // POST: Randevu/Cancel/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var randevu = await _context.Randevular
            .FirstOrDefaultAsync(r => r.Id == id && r.UyeId == user.Id);

        if (randevu == null)
        {
            return NotFound();
        }

        if (randevu.Durum == "Tamamlandı")
        {
            TempData["Error"] = "Tamamlanmış randevu iptal edilemez.";
            return RedirectToAction(nameof(Index));
        }

        randevu.Durum = "İptal";
        await _context.SaveChangesAsync();
        _logger.LogInformation("Randevu iptal edildi: {RandevuId}", randevu.Id);

        return RedirectToAction(nameof(Index));
    }
}


