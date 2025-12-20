using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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

    public async Task<(string Recommendation, string? VisualResult)> GetExerciseRecommendationAsync(
        string? vucutTipi = null,
        double? boy = null,
        double? kilo = null,
        string? hedef = null)
    {
        var baseUrl = _configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
        var model = _configuration["Ollama:Model"] ?? "llama3.2"; // Varsayılan model

        try
        {
            var prompt = BuildPrompt(vucutTipi, boy, kilo, hedef);

            // Normal text-only model
            var textRequest = new
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

            var json = JsonSerializer.Serialize(textRequest);

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
                    var errorMsg = "Ollama servisi çalışmıyor. Lütfen Ollama'nın kurulu ve çalışır durumda olduğundan emin olun.\n\n" +
                           "Kurulum için: https://ollama.com\n" +
                           "Model indirmek için terminal'de: ollama pull llama3.2";
                    return (errorMsg, null);
                }

                return ($"Ollama API hatası: {response.StatusCode}", null);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonDocument.Parse(responseContent);

            // Ollama response formatı
            var message = responseJson.RootElement
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            var recommendation = message ?? "Öneri alınamadı.";

            return (recommendation, null);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Ollama bağlantı hatası.");
            var errorMsg = "Ollama servisine bağlanılamadı. Lütfen Ollama'nın çalıştığından emin olun.\n\n" +
                   "Kontrol için terminal'de: curl http://localhost:11434/api/tags";
            return (errorMsg, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ollama API çağrısı başarısız oldu.");
            return ($"Hata oluştu: {ex.Message}", null);
        }
    }

    private async Task<(string VisualResult, string? ImagePrompt)> GetVisualResultAsync(
        string? vucutTipi,
        double? boy,
        double? kilo,
        string? hedef,
        string? fotoYolu,
        string? base64Image)
    {
        var baseUrl = _configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
        var model = _configuration["Ollama:Model"] ?? "llama3.2";
        var visionModel = _configuration["Ollama:VisionModel"] ?? "llava";

        try
        {
            var (visualPrompt, imageGenerationPrompt) = BuildVisualPrompt(vucutTipi, boy, kilo, hedef, fotoYolu != null);

            string json;
            if (!string.IsNullOrEmpty(base64Image))
            {
                var visionRequest = new Dictionary<string, object>
                {
                    ["model"] = visionModel,
                    ["messages"] = new object[]
                    {
                        new Dictionary<string, object>
                        {
                            ["role"] = "system",
                            ["content"] = "Sen bir fitness ve vücut transformasyon uzmanısın. Türkçe olarak, kullanıcıların egzersiz programını takip ettikten sonra nasıl görüneceklerini çok detaylı ve görsel bir şekilde açıklıyorsun. Fiziksel değişiklikleri, vücut kompozisyonunu, kas gelişimini, duruşu ve genel görünümü çok detaylı bir şekilde betimliyorsun."
                        },
                        new Dictionary<string, object>
                        {
                            ["role"] = "user",
                            ["content"] = visualPrompt,
                            ["images"] = new[] { base64Image }
                        }
                    },
                    ["stream"] = false,
                    ["options"] = new Dictionary<string, object>
                    {
                        ["temperature"] = 0.8,
                        ["num_predict"] = 2000
                    }
                };
                json = JsonSerializer.Serialize(visionRequest);
            }
            else
            {
                var textRequest = new
                {
                    model = model,
                    messages = new[]
                    {
                        new { role = "system", content = "Sen bir fitness ve vücut transformasyon uzmanısın. Türkçe olarak, kullanıcıların egzersiz programını takip ettikten sonra nasıl görüneceklerini çok detaylı ve görsel bir şekilde açıklıyorsun. Fiziksel değişiklikleri, vücut kompozisyonunu, kas gelişimini, duruşu ve genel görünümü çok detaylı bir şekilde betimliyorsun." },
                        new { role = "user", content = visualPrompt }
                    },
                    stream = false,
                    options = new
                    {
                        temperature = 0.8,
                        num_predict = 2000
                    }
                };
                json = JsonSerializer.Serialize(textRequest);
            }

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/chat", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseJson = JsonDocument.Parse(responseContent);
                var message = responseJson.RootElement
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return (message ?? "Görsel sonuç alınamadı.", imageGenerationPrompt);
            }

            return ("Görsel sonuç alınamadı.", imageGenerationPrompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Görsel sonuç alınırken hata oluştu.");
            return ("Görsel sonuç alınırken bir hata oluştu.", null);
        }
    }

    private (string VisualPrompt, string? ImageGenerationPrompt) BuildVisualPrompt(string? vucutTipi, double? boy, double? kilo, string? hedef, bool fotoVarMi)
    {
        var prompt = new StringBuilder();

        prompt.AppendLine("Aşağıdaki bilgilere göre, kullanıcının önerilen egzersiz programını takip ettikten sonra (3-6 ay sonra) nasıl görüneceğini ÇOK DETAYLI bir şekilde açıkla:");
        prompt.AppendLine();

        if (fotoVarMi)
        {
            prompt.AppendLine("Yüklenen fotoğrafı referans alarak:");
        }

        if (!string.IsNullOrEmpty(vucutTipi))
        {
            prompt.AppendLine($"- Mevcut Vücut Tipi: {vucutTipi}");
        }

        if (boy.HasValue && kilo.HasValue)
        {
            prompt.AppendLine($"- Mevcut Ölçüler: {boy} cm, {kilo} kg");
        }

        if (!string.IsNullOrEmpty(hedef))
        {
            prompt.AppendLine($"- Hedef: {hedef}");
        }

        prompt.AppendLine();
        prompt.AppendLine("Lütfen şunları ÇOK DETAYLI ve GÖRSEL bir şekilde açıkla (sanki bir fotoğrafı betimliyormuş gibi):");
        prompt.AppendLine();
        prompt.AppendLine("1. VÜCUT KOMPOZİSYONU:");
        prompt.AppendLine("   - Yağ oranındaki değişiklikler (yüzde olarak)");
        prompt.AppendLine("   - Kas kütlesindeki artış");
        prompt.AppendLine("   - Genel vücut ölçüleri (göğüs, bel, kalça, kol, bacak)");
        prompt.AppendLine();
        prompt.AppendLine("2. KAS GELİŞİMİ:");
        prompt.AppendLine("   - Hangi kas grupları nasıl gelişecek (göğüs, sırt, omuz, kol, bacak, karın)");
        prompt.AppendLine("   - Kas tanımları ve hatlar");
        prompt.AppendLine("   - Vücut simetrisi");
        prompt.AppendLine();
        prompt.AppendLine("3. DURUŞ VE POSTÜR:");
        prompt.AppendLine("   - Omuz duruşu");
        prompt.AppendLine("   - Bel eğrisi");
        prompt.AppendLine("   - Genel vücut hizalaması");
        prompt.AppendLine();
        prompt.AppendLine("4. GENEL GÖRÜNÜM:");
        prompt.AppendLine("   - Kıyafet ölçülerindeki değişiklikler");
        prompt.AppendLine("   - Vücut hatları ve siluet");
        prompt.AppendLine("   - Cilt tonu ve sağlıklı görünüm");
        prompt.AppendLine();
        prompt.AppendLine("5. ENERJİ VE ÖZGÜVEN:");
        prompt.AppendLine("   - Yüz ifadesi ve enerji seviyesi");
        prompt.AppendLine("   - Özgüven ve duruş");
        prompt.AppendLine("   - Genel sağlıklı görünüm");
        prompt.AppendLine();
        prompt.AppendLine("Lütfen bu açıklamayı sanki bir 'önce-sonra' fotoğrafını betimliyormuş gibi, çok görsel, detaylı ve ilham verici bir şekilde yaz. Türkçe olarak, anlaşılır ve motivasyonel bir ton kullan.");

        // Görsel üretme için prompt oluştur
        var imagePrompt = new StringBuilder();
        imagePrompt.Append("A professional fitness transformation photo showing: ");
        if (fotoVarMi)
        {
            imagePrompt.Append("the person from the uploaded photo after ");
        }
        else
        {
            imagePrompt.Append("a person after ");
        }

        if (!string.IsNullOrEmpty(hedef))
        {
            imagePrompt.Append($"{hedef.ToLower()}, ");
        }

        imagePrompt.Append("3-6 months of following a personalized exercise program. ");
        imagePrompt.Append("The person should look fit, healthy, confident, with improved muscle definition, better posture, ");
        imagePrompt.Append("and a positive transformation. Professional photography style, realistic, inspiring, motivational.");

        if (!string.IsNullOrEmpty(vucutTipi))
        {
            imagePrompt.Append($" Body type: {vucutTipi}.");
        }

        return (prompt.ToString(), imagePrompt.ToString());
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

        // Görsel sonuç ayrı bir çağrıda alınacak, burada sadece egzersiz programı

        prompt.AppendLine("\nLütfen Türkçe olarak, anlaşılır ve uygulanabilir bir şekilde yanıt ver.");

        return prompt.ToString();
    }
}


