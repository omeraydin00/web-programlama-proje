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
            var (recommendation, _) = await _ollamaService.GetExerciseRecommendationAsync(
                model.VucutTipi,
                model.Boy,
                model.Kilo,
                model.Hedef);

            ViewBag.Recommendation = recommendation;
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




