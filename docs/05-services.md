# 🤖 Services (AI Layer)

Services adalah layer yang menangani logika AI/ML untuk scoring dan feedback jawaban wawancara.

---

## Diagram Arsitektur Services

```
┌─────────────────────────────────────────────────────────────┐
│                    InterviewController                       │
│                            │                                 │
│                            ▼                                 │
│              IMachineLearningService                         │
│              (Interface - Dependency Injection)              │
└─────────────────────────────┬───────────────────────────────┘
                              │
            ┌─────────────────┴─────────────────┐
            ▼                                   ▼
┌───────────────────────┐         ┌───────────────────────────┐
│    GeminiService      │         │  MachineLearningService   │
│    (AKTIF) ⭐          │         │  (LEGACY - tidak aktif)   │
│                       │         │                           │
│  → Google Gemini API  │         │  → ML.NET (offline)       │
└───────────┬───────────┘         └───────────────────────────┘
            │
            ▼
┌───────────────────────┐
│   Google Gemini API   │
│   (External Cloud)    │
│                       │
│  gemini-3-flash       │
└───────────────────────┘
```

---

## 1. IMachineLearningService (Interface)

**Lokasi:** `Siwate.Web/Services/IMachineLearningService.cs`

```csharp
public interface IMachineLearningService
{
    // Legacy: Training model (tidak digunakan untuk Gemini)
    void Train(IEnumerable<Dataset> data);

    // Predict score + feedback (async untuk API call)
    Task<(float Score, string Feedback)> PredictAsync(string questionText, string answerText);
}
```

Interface ini mendefinisikan kontrak untuk semua service ML/AI:
- `Train()` - Untuk training model (legacy, tidak digunakan Gemini)
- `PredictAsync()` - Untuk prediksi skor dan feedback

---

## 2. GeminiService ⭐ (Implementasi Aktif)

**Lokasi:** `Siwate.Web/Services/GeminiService.cs`

### Konfigurasi

Service ini menggunakan Google Gemini API dengan model `gemini-3-flash-preview`:

```csharp
private const string GEMINI_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent";
```

API Key diambil dari `appsettings.json`:
```json
{
  "GeminiApiKey": "AIzaSy..."
}
```

### Alur Kerja PredictAsync

```
┌─────────────────────────────────────────────────────────────┐
│ Input: questionText, answerText                              │
└──────────────────────────────┬──────────────────────────────┘
                               ↓
┌─────────────────────────────────────────────────────────────┐
│ 1. Buat Prompt dengan instruksi penilaian:                   │
│    - Role: "Asisten HRD Profesional"                        │
│    - Pertanyaan wawancara                                   │
│    - Jawaban kandidat                                       │
│    - Instruksi validasi (bahasa, relevansi, STAR)           │
└──────────────────────────────┬──────────────────────────────┘
                               ↓
┌─────────────────────────────────────────────────────────────┐
│ 2. Kirim HTTP POST ke Gemini API                             │
│    URL: {GEMINI_URL}?key={API_KEY}                          │
│    Body: { contents: [{ parts: [{ text: prompt }] }] }      │
└──────────────────────────────┬──────────────────────────────┘
                               ↓
┌─────────────────────────────────────────────────────────────┐
│ 3. Parse Response JSON                                       │
│    Ambil: candidates[0].content.parts[0].text               │
│    Bersihkan markdown (```json```)                          │
└──────────────────────────────┬──────────────────────────────┘
                               ↓
┌─────────────────────────────────────────────────────────────┐
│ 4. Deserialize ke GeminiResult                               │
│    { "score": 85, "feedback": "Jawaban cukup baik..." }     │
└──────────────────────────────┬──────────────────────────────┘
                               ↓
┌─────────────────────────────────────────────────────────────┐
│ Output: (Score: 85, Feedback: "Jawaban cukup baik...")       │
└─────────────────────────────────────────────────────────────┘
```

### Prompt Template

```text
Anda adalah Asisten HRD Profesional. Tugas Anda menilai jawaban wawancara kerja.

PERTANYAAN: "[questionText]"
JAWABAN KANDIDAT: "[answerText]"

INSTRUKSI PENILAIAN:
1. Validasi Bahasa: Jika jawaban BUKAN dalam Bahasa Indonesia, berikan Skor 0 
   dan Feedback "Mohon jawab menggunakan Bahasa Indonesia."
2. Validasi Relevansi: Jika jawaban Melenceng/Tidak Nyambung/Gibberish/Asal-asalan, 
   berikan Skor 0-10.
3. Kualitas: Nilai berdasarkan kejelasan, metode STAR (Situation, Task, Action, Result), 
   dan profesionalisme.
4. Jangan tertipu panjang teks. Jawaban panjang tapi tidak berbobot harus nilai rendah.

OUTPUT WAJIB JSON (Tanpa Markdown):
{
  "score": (angka 0-100),
  "feedback": "(kalimat saran singkat max 30 kata)"
}
```

### Kode Lengkap

```csharp
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

        var aiResult = JsonSerializer.Deserialize<GeminiResult>(textResponse, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return (aiResult.Score, aiResult.Feedback);
    }
    catch
    {
        return (0, "Gagal memproses respons AI.");
    }
}
```

---

## 3. MachineLearningService (Legacy)

**Lokasi:** `Siwate.Web/Services/MachineLearningService.cs`

> ⚠️ **Service ini TIDAK AKTIF**. Telah digantikan oleh GeminiService.

### Deskripsi

Service ini menggunakan **ML.NET** untuk machine learning offline:
- Algoritma: SDCA Regression
- Features: TF-IDF dari AnswerText + TextLength
- Model disimpan ke file `interview_model.zip`

### Kode Training (Legacy)

```csharp
public void Train(IEnumerable<Dataset> data)
{
    var trainData = data.Select(d => new ModelInput
    {
        AnswerText = d.AnswerText,
        TextLength = (float)d.AnswerText.Length,
        Score = (float)d.Score
    });

    IDataView dataView = _mlContext.Data.LoadFromEnumerable(trainData);

    var pipeline = _mlContext.Transforms.Text.FeaturizeText("TextFeatures", nameof(ModelInput.AnswerText))
        .Append(_mlContext.Transforms.Concatenate("Features", "TextFeatures", nameof(ModelInput.TextLength)))
        .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(ModelInput.Score), 
            maximumNumberOfIterations: 100));

    _model = pipeline.Fit(dataView);
    _mlContext.Model.Save(_model, dataView.Schema, _modelPath);
}
```

---

## 4. Model Classes (ML.NET Legacy)

### ModelInput.cs
```csharp
public class ModelInput
{
    [LoadColumn(0)]
    public string AnswerText { get; set; }

    [LoadColumn(1)]
    public float TextLength { get; set; }

    [LoadColumn(2)]
    public float Score { get; set; } // Label
}
```

### ModelOutput.cs
```csharp
public class ModelOutput
{
    [ColumnName("Score")]
    public float Score { get; set; }
}
```

---

## Perbandingan GeminiService vs MachineLearningService

| Aspek | GeminiService ⭐ | MachineLearningService |
|-------|-----------------|------------------------|
| Status | **AKTIF** | Legacy (tidak digunakan) |
| Teknologi | Google Gemini API | ML.NET |
| Training Required | ❌ Tidak perlu | ✅ Perlu dataset |
| Internet Required | ✅ Ya | ❌ Tidak |
| Feedback Quality | Sangat baik (contextual) | Basic (score only) |
| Anti-Cheating | ✅ Deteksi gibberish | ❌ Tidak |
| Language Validation | ✅ Ya | ❌ Tidak |
| Cost | API usage | Free (offline) |

---

**Sebelumnya:** [← Controllers](./04-controllers.md)  
**Selanjutnya:** [Views →](./06-views.md)
