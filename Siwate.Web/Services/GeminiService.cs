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



        public async Task<GeminiResult> PredictAsync(string questionText, string answerText, int durationSeconds)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new Exception("Gemini API Key belum dikonfigurasi di appsettings.json");

            var prompt = $@"
                Anda adalah System Interview AI yang Cerdas.
                Tugas: Analisis jawaban kandidat untuk menentukan kualitas, kedalaman, dan perilaku (berdasarkan waktu jawab).

                PERTANYAAN: ""{questionText}""
                JAWABAN KANDIDAT: ""{answerText}""
                DURASI MENJAWAB: {durationSeconds} detik.

                LOGIKA KEPUTUSAN:
                1. JIKA jawaban terlalu singkat (kurang dari 5 kata) ATAU sangat umum (cliche) ATAU tidak menjawab inti masalah:
                - Set ""isGeneric"": true
                - Buat ""followUpQuestion"": Pertanyaan pancingan untuk menggali lebih dalam detail STAR (Situation, Task, Action, Result) yang hilang.
                2. JIKA jawaban cukup detail DAN relevan:
                - Set ""isGeneric"": false
                - Berikan ""score"" (0-100) dan ""feedback"" seperti biasa.
                3. PERTIMBANGKAN WAKTU:
                - Jika durasi < 10 detik untuk jawaban panjang -> Indikasi Copy-Paste -> Kurangi nilai.
                - Jika durasi sangat lama (> 3 menit) -> Pertimbangkan keragu-raguan.

                OUTPUT WAJIB JSON (Tanpa Markdown):
                {{
                ""score"": (0-100, jika isGeneric=true berikan 0),
                ""feedback"": ""(saran perbaikan atau alasan butuh follow-up)"",
                ""isGeneric"": (true/false),
                ""followUpQuestion"": ""(isi hanya jika isGeneric=true, jika tidak kosongkan string)""
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
                return new GeminiResult { Score = 0, Feedback = $"Error AI: {response.StatusCode}" };
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
                return aiResult;
            }
            catch
            {
                return new GeminiResult { Score = 0, Feedback = "Gagal memproses respons AI." };
            }
        }
    }
}
