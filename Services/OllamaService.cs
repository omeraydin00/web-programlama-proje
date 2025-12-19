using System.Text;
using System.Text.Json;

namespace SporSalonuYonetim.Services;

public class OllamaService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OllamaService> _logger;

    public OllamaService(HttpClient httpClient, IConfiguration configuration, ILogger<OllamaService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        // Ollama base URL'i ayarla (varsayılan: http://localhost:11434)
        var baseUrl = _configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.Timeout = TimeSpan.FromMinutes(2); // Ollama biraz yavaş olabilir
    }

    public async Task<string> GetExerciseRecommendationAsync(
        string? vucutTipi = null,
        double? boy = null,
        double? kilo = null,
        string? hedef = null,
        bool gorselSonucIste = false)
    {
        var baseUrl = _configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
        var model = _configuration["Ollama:Model"] ?? "llama3.2"; // Varsayılan model

        try
        {
            var prompt = BuildPrompt(vucutTipi, boy, kilo, hedef, gorselSonucIste);

            // Ollama Chat API formatı
            var requestBody = new
            {
                model = model,
                messages = new[]
                {
                    new { role = "system", content = "Sen bir fitness ve egzersiz uzmanısın. Türkçe olarak kullanıcılara kişiselleştirilmiş egzersiz önerileri sunuyorsun. Detaylı, anlaşılır ve uygulanabilir öneriler veriyorsun." },
                    new { role = "user", content = prompt }
                },
                stream = false,
                options = new
                {
                    temperature = 0.7,
                    num_predict = 1000 // Maksimum token sayısı
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/chat", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Ollama API hatası: {StatusCode} - {Error}", response.StatusCode, errorContent);

                // Ollama çalışmıyorsa kullanıcıya bilgi ver
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound ||
                    response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    return "Ollama servisi çalışmıyor. Lütfen Ollama'nın kurulu ve çalışır durumda olduğundan emin olun.\n\n" +
                           "Kurulum için: https://ollama.com\n" +
                           "Model indirmek için terminal'de: ollama pull llama3.2";
                }

                return $"Ollama API hatası: {response.StatusCode}";
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonDocument.Parse(responseContent);

            // Ollama response formatı
            var message = responseJson.RootElement
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return message ?? "Öneri alınamadı.";
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Ollama bağlantı hatası.");
            return "Ollama servisine bağlanılamadı. Lütfen Ollama'nın çalıştığından emin olun.\n\n" +
                   "Kontrol için terminal'de: curl http://localhost:11434/api/tags";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ollama API çağrısı başarısız oldu.");
            return $"Hata oluştu: {ex.Message}";
        }
    }

    private string BuildPrompt(string? vucutTipi, double? boy, double? kilo, string? hedef, bool gorselSonucIste = false)
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

        if (gorselSonucIste)
        {
            prompt.AppendLine("\n5. Bu egzersiz programını takip ederse kullanıcı nasıl görünecek? Fiziksel değişiklikleri detaylı açıkla.");
        }

        prompt.AppendLine("\nLütfen Türkçe olarak, anlaşılır ve uygulanabilir bir şekilde yanıt ver.");

        return prompt.ToString();
    }
}

