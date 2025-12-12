using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SporSalonuYonetim.Models;
using SporSalonuYonetim.Services;

namespace SporSalonuYonetim.Controllers;

[Authorize]
public class AIEgzersizController : Controller
{
    private readonly OpenAIService _openAIService;
    private readonly ILogger<AIEgzersizController> _logger;

    public AIEgzersizController(OpenAIService openAIService, ILogger<AIEgzersizController> logger)
    {
        _openAIService = openAIService;
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
            var recommendation = await _openAIService.GetExerciseRecommendationAsync(
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

