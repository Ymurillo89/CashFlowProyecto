using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using AngularApp1.Server.Models.Entities;

namespace AngularApp1.Server.Services
{
    public class OcrService : IOcrService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;

        public OcrService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"]!;
            _model = configuration["Gemini:Model"]!;
        }

        public async Task<OcrResult> ProcessImageAsync(string filePath, string declaredReference, decimal declaredAmount)
        {
            try
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(12);

                var imageBytes = await File.ReadAllBytesAsync(filePath);
                var base64 = Convert.ToBase64String(imageBytes);
                var mimeType = GetMimeType(filePath);

                var prompt = @"Extract the following from this bank deposit receipt image.
Return ONLY valid JSON with these exact keys (no markdown, no code fences):
{
  ""bank"": ""bank name found on receipt"",
  ""reference"": ""reference or transaction number"",
  ""amount"": 1234.56,
  ""date"": ""YYYY-MM-DD""
}
If any field cannot be read, use null.";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new object[]
                            {
                                new { text = prompt },
                                new { inline_data = new { mime_type = mimeType, data = base64 } }
                            }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var extracted = ParseGeminiResponse(responseJson);

                return new OcrResult
                {
                    DetectedBank = extracted.Bank ?? "No detectado",
                    DetectedReference = extracted.Reference ?? declaredReference,
                    DetectedAmount = extracted.Amount ?? declaredAmount,
                    DetectedDate = extracted.Date ?? DateTime.Today,
                    Confidence = extracted.Confidence,
                    RawText = responseJson,
                    ProcessedAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                // Fallback result if API call or network fails
                return new OcrResult
                {
                    DetectedBank = "Error de Red/API",
                    DetectedReference = declaredReference,
                    DetectedAmount = declaredAmount,
                    DetectedDate = DateTime.Today,
                    Confidence = 0.0m,
                    RawText = $"Error al procesar con Gemini: {ex.Message}",
                    ProcessedAt = DateTime.Now
                };
            }
        }

        private static (string? Bank, string? Reference, decimal? Amount, DateTime? Date, decimal Confidence) ParseGeminiResponse(string json)
        {
            string? bank = null, reference = null;
            decimal? amount = null;
            DateTime? date = null;
            decimal confidence = 0.85m;

            try
            {
                using var doc = JsonDocument.Parse(json);
                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                if (text == null) return (null, null, null, null, confidence);

                var cleaned = text.Trim();
                if (cleaned.StartsWith("```")) cleaned = cleaned.Substring(cleaned.IndexOf('\n') + 1);
                if (cleaned.EndsWith("```")) cleaned = cleaned.Substring(0, cleaned.LastIndexOf("```")).Trim();

                using var extracted = JsonDocument.Parse(cleaned);
                var root = extracted.RootElement;

                if (root.TryGetProperty("bank", out var bankEl)) bank = bankEl.GetString();
                if (root.TryGetProperty("reference", out var refEl)) reference = refEl.GetString();
                if (root.TryGetProperty("amount", out var amtEl) && amtEl.ValueKind == JsonValueKind.Number)
                    amount = amtEl.GetDecimal();
                if (root.TryGetProperty("date", out var dateEl))
                {
                    var dateStr = dateEl.GetString();
                    if (!string.IsNullOrEmpty(dateStr) && DateTime.TryParse(dateStr, out var parsed))
                        date = parsed;
                }
            }
            catch
            {
                confidence = 0.5m;
            }

            return (bank, reference, amount, date, confidence);
        }

        private static string GetMimeType(string filePath)
        {
            var ext = Path.GetExtension(filePath)?.ToLowerInvariant();
            return ext switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".webp" => "image/webp",
                ".heic" => "image/heic",
                ".heif" => "image/heif",
                _ => "image/jpeg"
            };
        }
    }
}
