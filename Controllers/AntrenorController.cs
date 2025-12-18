using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Data;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Controllers;

public class AntrenorController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AntrenorController> _logger;

    public AntrenorController(ApplicationDbContext context, ILogger<AntrenorController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Antrenor/PublicList
    public async Task<IActionResult> PublicList()
    {
        var antrenorler = await _context.Antrenorler
            .Include(a => a.SporSalonu)
            .Include(a => a.Müsaitlikler)
            .ToListAsync();

        return View(antrenorler);
    }

    // GET: Antrenor/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var antrenor = await _context.Antrenorler
            .Include(a => a.SporSalonu)
            .Include(a => a.Müsaitlikler)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (antrenor == null)
        {
            return NotFound();
        }

        return View(antrenor);
    }
}



<<<<<<< Updated upstream

=======
>>>>>>> Stashed changes
