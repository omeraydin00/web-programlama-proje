using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Data;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class AntrenorController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AntrenorController> _logger;

    public AntrenorController(ApplicationDbContext context, ILogger<AntrenorController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Admin/Antrenor
    public async Task<IActionResult> Index()
    {
        try
        {
        var antrenorler = await _context.Antrenorler
            .Include(a => a.SporSalonu)
            .ToListAsync();
            return View("~/Views/Admin/Antrenor/Index.cshtml", antrenorler);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Antrenörler listelenirken hata oluştu: {ErrorMessage}", ex.Message);
            return View("~/Views/Admin/Antrenor/Index.cshtml", new List<Antrenor>());
        }
    }

    // GET: Admin/Antrenor/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var antrenor = await _context.Antrenorler
            .Include(a => a.SporSalonu)
            .Include(a => a.Müsaitlikler)
            .Include(a => a.Randevular)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (antrenor == null)
        {
            return NotFound();
        }

        return View("~/Views/Admin/Antrenor/Details.cshtml", antrenor);
    }

    // GET: Admin/Antrenor/Create
    public IActionResult Create()
    {
        ViewData["SporSalonuId"] = new SelectList(_context.SporSalonlari, "Id", "Ad");
        return View("~/Views/Admin/Antrenor/Create.cshtml");
    }

    // POST: Admin/Antrenor/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Ad,Soyad,Email,Telefon,UzmanlikAlanlari,Aciklama,SporSalonuId")] Antrenor antrenor)
    {
        if (ModelState.IsValid)
        {
            antrenor.OlusturmaTarihi = DateTime.Now;
            _context.Add(antrenor);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Yeni antrenör eklendi: {Ad} {Soyad}", antrenor.Ad, antrenor.Soyad);
            return RedirectToAction(nameof(Index));
        }
        ViewData["SporSalonuId"] = new SelectList(_context.SporSalonlari, "Id", "Ad", antrenor.SporSalonuId);
        return View("~/Views/Admin/Antrenor/Create.cshtml", antrenor);
    }

    // GET: Admin/Antrenor/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var antrenor = await _context.Antrenorler.FindAsync(id);
        if (antrenor == null)
        {
            return NotFound();
        }
        ViewData["SporSalonuId"] = new SelectList(_context.SporSalonlari, "Id", "Ad", antrenor.SporSalonuId);
        return View("~/Views/Admin/Antrenor/Edit.cshtml", antrenor);
    }

    // POST: Admin/Antrenor/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,Soyad,Email,Telefon,UzmanlikAlanlari,Aciklama,SporSalonuId,OlusturmaTarihi")] Antrenor antrenor)
    {
        if (id != antrenor.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(antrenor);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Antrenör güncellendi: {Ad} {Soyad}", antrenor.Ad, antrenor.Soyad);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AntrenorExists(antrenor.Id))
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
        ViewData["SporSalonuId"] = new SelectList(_context.SporSalonlari, "Id", "Ad", antrenor.SporSalonuId);
        return View("~/Views/Admin/Antrenor/Edit.cshtml", antrenor);
    }

    // GET: Admin/Antrenor/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var antrenor = await _context.Antrenorler
            .Include(a => a.SporSalonu)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (antrenor == null)
        {
            return NotFound();
        }

        return View("~/Views/Admin/Antrenor/Delete.cshtml", antrenor);
    }

    // POST: Admin/Antrenor/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var antrenor = await _context.Antrenorler.FindAsync(id);
        if (antrenor != null)
        {
            _context.Antrenorler.Remove(antrenor);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Antrenör silindi: {Ad} {Soyad}", antrenor.Ad, antrenor.Soyad);
        }

        return RedirectToAction(nameof(Index));
    }

    private bool AntrenorExists(int id)
    {
        return _context.Antrenorler.Any(e => e.Id == id);
    }
}







<<<<<<< Updated upstream

=======
>>>>>>> Stashed changes
