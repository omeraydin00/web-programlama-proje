using System.Text;
using System.Text.Json;

namespace SporSalonuYonetim.Services;

public class OpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OpenAIService> _logger;

    public OpenAIService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenAIService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        
        var apiKey = _configuration["OpenAI:ApiKey"];
        if (!string.IsNullOrEmpty(apiKey) && apiKey != "YOUR_OPENAI_API_KEY_HERE")
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }
    }

    public async Task<string> GetExerciseRecommendationAsync(
        string? vucutTipi = null,
        double? boy = null,
        double? kilo = null,
        string? hedef = null)
    {
        var apiKey = _configuration["OpenAI:ApiKey"];
        if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_OPENAI_API_KEY_HERE")
        {
            return "OpenAI API anahtarı yapılandırılmamış. Lütfen appsettings.json dosyasında OpenAI:ApiKey değerini ayarlayın.";
        }

        try
        {
            var prompt = BuildPrompt(vucutTipi, boy, kilo, hedef);

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "Sen bir fitness ve egzersiz uzmanısın. Kullanıcılara kişiselleştirilmiş egzersiz önerileri sunuyorsun." },
                    new { role = "user", content = prompt }
                },
                max_tokens = 500,
                temperature = 0.7
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonDocument.Parse(responseContent);

            var recommendation = responseJson.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return recommendation ?? "Öneri alınamadı.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI API çağrısı başarısız oldu.");
            return $"Hata oluştu: {ex.Message}";
        }
    }

    private string BuildPrompt(string? vucutTipi, double? boy, double? kilo, string? hedef)
    {
        var prompt = new StringBuilder();
        prompt.AppendLine("Aşağıdaki bilgilere göre bana kişiselleştirilmiş bir egzersiz programı öner:");

        if (!string.IsNullOrEmpty(vucutTipi))
        {
            prompt.AppendLine($"- Vücut Tipi: {vucutTipi}");
        }

        if (boy.HasValue)
        {
            prompt.AppendLine($"- Boy: {boy} cm");
        }

        if (kilo.HasValue)
        {
            prompt.AppendLine($"- Kilo: {kilo} kg");
        }

        if (!string.IsNullOrEmpty(hedef))
        {
            prompt.AppendLine($"- Hedef: {hedef}");
        }

        prompt.AppendLine();
        prompt.AppendLine("Lütfen şunları içeren detaylı bir egzersiz programı öner:");
        prompt.AppendLine("1. Haftalık egzersiz planı");
        prompt.AppendLine("2. Her egzersiz için set ve tekrar sayıları");
        prompt.AppendLine("3. Beslenme önerileri");
        prompt.AppendLine("4. İlerleme takibi için ipuçları");

        return prompt.ToString();
    }
}




