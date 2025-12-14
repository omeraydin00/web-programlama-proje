using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Data;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class RandevuYonetimController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RandevuYonetimController> _logger;

    public RandevuYonetimController(ApplicationDbContext context, ILogger<RandevuYonetimController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Admin/RandevuYonetim
    public async Task<IActionResult> Index(string? durum)
    {
        try
        {
            var query = _context.Randevular
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .Include(r => r.Uye)
                .AsQueryable();

            if (!string.IsNullOrEmpty(durum))
            {
                query = query.Where(r => r.Durum == durum);
            }

            var randevular = await query
                .OrderByDescending(r => r.RandevuTarihi)
                .ThenByDescending(r => r.BaslangicSaati)
                .ToListAsync();

            ViewBag.Durum = durum;
            return View("~/Views/Admin/RandevuYonetim/Index.cshtml", randevular);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Randevular listelenirken hata oluştu: {ErrorMessage}", ex.Message);
            return View("~/Views/Admin/RandevuYonetim/Index.cshtml", new List<Randevu>());
        }
    }

    // POST: Admin/RandevuYonetim/Onayla/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Onayla(int id)
    {
        var randevu = await _context.Randevular.FindAsync(id);
        if (randevu == null)
        {
            return NotFound();
        }

        randevu.Durum = "Onaylandı";
        await _context.SaveChangesAsync();
        _logger.LogInformation("Randevu onaylandı: {RandevuId}", randevu.Id);

        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/RandevuYonetim/Reddet/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reddet(int id)
    {
        var randevu = await _context.Randevular.FindAsync(id);
        if (randevu == null)
        {
            return NotFound();
        }

        randevu.Durum = "İptal";
        await _context.SaveChangesAsync();
        _logger.LogInformation("Randevu reddedildi: {RandevuId}", randevu.Id);

        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/RandevuYonetim/Tamamla/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Tamamla(int id)
    {
        var randevu = await _context.Randevular.FindAsync(id);
        if (randevu == null)
        {
            return NotFound();
        }

        randevu.Durum = "Tamamlandı";
        await _context.SaveChangesAsync();
        _logger.LogInformation("Randevu tamamlandı: {RandevuId}", randevu.Id);

        return RedirectToAction(nameof(Index));
    }
}


