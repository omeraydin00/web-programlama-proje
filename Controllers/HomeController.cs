using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using SporSalonuYonetim.Data;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var sporSalonlari = await _context.SporSalonlari
                .Include(s => s.Antrenorler)
                .Include(s => s.Hizmetler)
                .Take(3)
                .ToListAsync();

            ViewBag.SporSalonlari = sporSalonlari ?? new List<SporSalonu>();
            ViewBag.ToplamAntrenor = await _context.Antrenorler.CountAsync();
            ViewBag.ToplamHizmet = await _context.Hizmetler.CountAsync();
            ViewBag.ToplamRandevu = await _context.Randevular.CountAsync();

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ana sayfa yüklenirken hata oluştu: {ErrorMessage}", ex.Message);
            ViewBag.SporSalonlari = new List<SporSalonu>();
            ViewBag.ToplamAntrenor = 0;
            ViewBag.ToplamHizmet = 0;
            ViewBag.ToplamRandevu = 0;
            return View();
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        
        if (exceptionFeature != null)
        {
            ViewBag.ErrorMessage = exceptionFeature.Error.Message;
            ViewBag.StackTrace = exceptionFeature.Error.StackTrace;
            ViewBag.RequestId = HttpContext.TraceIdentifier;
            _logger.LogError(exceptionFeature.Error, "Bir hata oluştu: {RequestId}", HttpContext.TraceIdentifier);
        }
        
        return View();
    }
}







