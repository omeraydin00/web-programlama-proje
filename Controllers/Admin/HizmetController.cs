using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Data;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class HizmetController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HizmetController> _logger;

    public HizmetController(ApplicationDbContext context, ILogger<HizmetController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Admin/Hizmet
    public async Task<IActionResult> Index()
    {
        try
        {
            var hizmetler = await _context.Hizmetler
                .Include(h => h.SporSalonu)
                .ToListAsync();
            return View("~/Views/Admin/Hizmet/Index.cshtml", hizmetler);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hizmetler listelenirken hata oluştu: {ErrorMessage}", ex.Message);
            return View("~/Views/Admin/Hizmet/Index.cshtml", new List<Hizmet>());
        }
    }

    // GET: Admin/Hizmet/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var hizmet = await _context.Hizmetler
            .Include(h => h.SporSalonu)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (hizmet == null)
        {
            return NotFound();
        }

        return View(hizmet);
    }

    // GET: Admin/Hizmet/Create
    public IActionResult Create()
    {
        ViewData["SporSalonuId"] = new SelectList(_context.SporSalonlari, "Id", "Ad");
        return View();
    }

    // POST: Admin/Hizmet/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Ad,Aciklama,Sure,Ucret,HizmetTuru,SporSalonuId")] Hizmet hizmet)
    {
        if (ModelState.IsValid)
        {
            hizmet.OlusturmaTarihi = DateTime.Now;
            _context.Add(hizmet);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Yeni hizmet eklendi: {Ad}", hizmet.Ad);
            return RedirectToAction(nameof(Index));
        }
        ViewData["SporSalonuId"] = new SelectList(_context.SporSalonlari, "Id", "Ad", hizmet.SporSalonuId);
        return View(hizmet);
    }

    // GET: Admin/Hizmet/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var hizmet = await _context.Hizmetler.FindAsync(id);
        if (hizmet == null)
        {
            return NotFound();
        }
        ViewData["SporSalonuId"] = new SelectList(_context.SporSalonlari, "Id", "Ad", hizmet.SporSalonuId);
        return View(hizmet);
    }

    // POST: Admin/Hizmet/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,Aciklama,Sure,Ucret,HizmetTuru,SporSalonuId,OlusturmaTarihi")] Hizmet hizmet)
    {
        if (id != hizmet.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(hizmet);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Hizmet güncellendi: {Ad}", hizmet.Ad);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HizmetExists(hizmet.Id))
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
        ViewData["SporSalonuId"] = new SelectList(_context.SporSalonlari, "Id", "Ad", hizmet.SporSalonuId);
        return View(hizmet);
    }

    // GET: Admin/Hizmet/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var hizmet = await _context.Hizmetler
            .Include(h => h.SporSalonu)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (hizmet == null)
        {
            return NotFound();
        }

        return View(hizmet);
    }

    // POST: Admin/Hizmet/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var hizmet = await _context.Hizmetler.FindAsync(id);
        if (hizmet != null)
        {
            _context.Hizmetler.Remove(hizmet);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Hizmet silindi: {Ad}", hizmet.Ad);
        }

        return RedirectToAction(nameof(Index));
    }

    private bool HizmetExists(int id)
    {
        return _context.Hizmetler.Any(e => e.Id == id);
    }
}


