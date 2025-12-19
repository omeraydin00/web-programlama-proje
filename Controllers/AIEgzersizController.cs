using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SporSalonuYonetim.Models;
using SporSalonuYonetim.Services;

namespace SporSalonuYonetim.Controllers;

[Authorize]
public class AIEgzersizController : Controller
{
    private readonly OllamaService _ollamaService;
    private readonly ILogger<AIEgzersizController> _logger;

    public AIEgzersizController(OllamaService ollamaService, ILogger<AIEgzersizController> logger)
    {
        _ollamaService = ollamaService;
        _logger = logger;
    }

    // GET: AIEgzersiz
    public IActionResult Index()
    {
        return View();
    }

    // POST: AIEgzersiz/GetRecommendation
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetRecommendation(EgzersizOnerisiViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        try
        {
            string? fotoYolu = null;

            // Fotoğraf yükleme işlemi
            if (model.Foto != null && model.Foto.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "fotolar");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Foto.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Foto.CopyToAsync(fileStream);
                }

                fotoYolu = $"/uploads/fotolar/{uniqueFileName}";
                ViewBag.FotoYolu = fotoYolu;
            }

            var recommendation = await _ollamaService.GetExerciseRecommendationAsync(
                model.VucutTipi,
                model.Boy,
                model.Kilo,
                model.Hedef,
                model.GorselSonucIste);

            ViewBag.Recommendation = recommendation;
            ViewBag.GorselSonucIste = model.GorselSonucIste;
            return View("Index", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Egzersiz önerisi alınırken hata oluştu.");
            ModelState.AddModelError("", "Öneri alınırken bir hata oluştu. Lütfen tekrar deneyin.");
            return View("Index", model);
        }
    }
}




