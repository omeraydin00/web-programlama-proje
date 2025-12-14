using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Data;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class SporSalonuController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SporSalonuController> _logger;

    public SporSalonuController(ApplicationDbContext context, ILogger<SporSalonuController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Admin/SporSalonu
    public async Task<IActionResult> Index()
    {
        try
        {
            var sporSalonlari = await _context.SporSalonlari.ToListAsync();
            return View("~/Views/Admin/SporSalonu/Index.cshtml", sporSalonlari);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Spor salonları listelenirken hata oluştu: {ErrorMessage}", ex.Message);
            return View("~/Views/Admin/SporSalonu/Index.cshtml", new List<SporSalonu>());
        }
    }

    // GET: Admin/SporSalonu/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sporSalonu = await _context.SporSalonlari
            .Include(s => s.Hizmetler)
            .Include(s => s.Antrenorler)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (sporSalonu == null)
        {
            return NotFound();
        }

        return View(sporSalonu);
    }

    // GET: Admin/SporSalonu/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/SporSalonu/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Ad,Adres,Telefon,Email,AcilisSaati,KapanisSaati")] SporSalonu sporSalonu)
    {
        if (ModelState.IsValid)
        {
            sporSalonu.OlusturmaTarihi = DateTime.Now;
            _context.Add(sporSalonu);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Yeni spor salonu eklendi: {Ad}", sporSalonu.Ad);
            return RedirectToAction(nameof(Index));
        }
        return View(sporSalonu);
    }

    // GET: Admin/SporSalonu/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sporSalonu = await _context.SporSalonlari.FindAsync(id);
        if (sporSalonu == null)
        {
            return NotFound();
        }
        return View(sporSalonu);
    }

    // POST: Admin/SporSalonu/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,Adres,Telefon,Email,AcilisSaati,KapanisSaati,OlusturmaTarihi")] SporSalonu sporSalonu)
    {
        if (id != sporSalonu.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(sporSalonu);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Spor salonu güncellendi: {Ad}", sporSalonu.Ad);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SporSalonuExists(sporSalonu.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(sporSalonu);
    }

    // GET: Admin/SporSalonu/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sporSalonu = await _context.SporSalonlari
            .FirstOrDefaultAsync(m => m.Id == id);
        if (sporSalonu == null)
        {
            return NotFound();
        }

        return View(sporSalonu);
    }

    // POST: Admin/SporSalonu/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var sporSalonu = await _context.SporSalonlari.FindAsync(id);
        if (sporSalonu != null)
        {
            _context.SporSalonlari.Remove(sporSalonu);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Spor salonu silindi: {Ad}", sporSalonu.Ad);
        }

        return RedirectToAction(nameof(Index));
    }

    private bool SporSalonuExists(int id)
    {
        return _context.SporSalonlari.Any(e => e.Id == id);
    }
}





