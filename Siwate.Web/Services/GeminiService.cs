using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Siwate.Web.Models;

namespace Siwate.Web.Services
{
    public class GeminiService : IMachineLearningService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private const string GEMINI_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent";

        public GeminiService(IConfiguration configuration)
        {
            _apiKey = configuration["GeminiApiKey"];
            _httpClient = new HttpClient();
        }



        public async Task<(float Score, string Feedback)> PredictAsync(string questionText, string answerText)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new Exception("Gemini API Key belum dikonfigurasi di appsettings.json");

            var prompt = $@"
Anda adalah Asisten HRD Profesional. Tugas Anda menilai jawaban wawancara kerja.

PERTANYAAN: ""{questionText}""
JAWABAN KANDIDAT: ""{answerText}""

INSTRUKSI PENILAIAN:
1. Validasi Bahasa: Jika jawaban BUKAN dalam Bahasa Indonesia, berikan Skor 0 dan Feedback ""Mohon jawab menggunakan Bahasa Indonesia.""
2. Validasi Relevansi: Jika jawaban Melenceng/Tidak Nyambung/Gibberish/Asal-asalan, berikan Skor 0-10.
3. Kualitas: Nilai berdasarkan kejelasan, metode STAR (Situation, Task, Action, Result), dan profesionalisme.
4. Jangan tertipu panjang teks. Jawaban panjang tapi tidak berbobot harus nilai rendah.

OUTPUT WAJIB JSON (Tanpa Markdown):
{{
  ""score"": (angka 0-100),
  ""feedback"": ""(kalimat saran singkat max 30 kata)""
}}
";
            
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{GEMINI_URL}?key={_apiKey}", jsonContent);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return (0, $"Error AI: {response.StatusCode} - {error}");
            }

            var resultJson = await response.Content.ReadAsStringAsync();
            
            try 
            {
                using var doc = JsonDocument.Parse(resultJson);
                var textResponse = doc.RootElement
                                    .GetProperty("candidates")[0]
                                    .GetProperty("content")
                                    .GetProperty("parts")[0]
                                    .GetProperty("text")
                                    .GetString();

                // Bersihkan markdown ```json jika ada
                textResponse = textResponse.Replace("```json", "").Replace("```", "").Trim();

                var aiResult = JsonSerializer.Deserialize<GeminiResult>(textResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return (aiResult.Score, aiResult.Feedback);
            }
            catch
            {
                return (0, "Gagal memproses respons AI.");
            }
        }
    }

    class GeminiResult
    {
        public float Score { get; set; }
        public string Feedback { get; set; }
    }
}
