# 🎮 Controllers

Controllers adalah otak dari aplikasi yang menangani HTTP requests, memproses logika bisnis, dan mengembalikan responses.

---

## 1. HomeController

**Lokasi:** `Siwate.Web/Controllers/HomeController.cs`  
**Akses:** Publik (tidak perlu login)

### Actions

| Action | HTTP | Route | Deskripsi |
|--------|------|-------|-----------|
| `Index()` | GET | `/` atau `/Home` | Landing page utama |
| `Privacy()` | GET | `/Home/Privacy` | Halaman kebijakan privasi |
| `Error()` | GET | `/Home/Error` | Halaman error handling |

### Kode

```csharp
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
```

---

## 2. AccountController

**Lokasi:** `Siwate.Web/Controllers/AccountController.cs`  
**Akses:** Publik

### Actions

| Action | HTTP | Route | Deskripsi |
|--------|------|-------|-----------|
| `Login()` | GET | `/Account/Login` | Form login |
| `Login(email, password)` | POST | `/Account/Login` | Proses login |
| `Register()` | GET | `/Account/Register` | Form registrasi |
| `Register(user)` | POST | `/Account/Register` | Proses registrasi |
| `Logout()` | GET | `/Account/Logout` | Hapus session |

### Alur Login

```
1. User submit form (email + password)
        ↓
2. Query database: users WHERE email = ? AND password = ?
        ↓
3. Jika ditemukan:
   - Buat ClaimsIdentity (NameIdentifier, Name, Role)
   - SignInAsync (set cookie)
   - Redirect berdasarkan role:
     • admin → /Admin/Index
     • user → /Home/Index
        ↓
4. Jika tidak ditemukan:
   - Tampilkan error "Email atau password salah"
```

### Alur Registrasi

```
1. User submit form (name, email, password)
        ↓
2. Validasi: Cek apakah email sudah terdaftar
        ↓
3. Jika email unique:
   - Simpan user baru ke database
   - Redirect ke /Account/Login
        ↓
4. Jika email sudah ada:
   - Tampilkan error "Email sudah terdaftar"
```

### Kode Penting

```csharp
[HttpPost]
public async Task<IActionResult> Login(string email, string password)
{
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
    if (user == null)
    {
        ViewBag.Error = "Email atau password salah.";
        return View();
    }

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Role, user.Role)
    };

    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

    if (user.Role == "admin")
        return RedirectToAction("Index", "Admin");

    return RedirectToAction("Index", "Home");
}
```

---

## 3. InterviewController ⭐ (Core)

**Lokasi:** `Siwate.Web/Controllers/InterviewController.cs`  
**Akses:** `[Authorize]` - Harus login

### Actions

| Action | HTTP | Route | Deskripsi |
|--------|------|-------|-----------|
| `Index()` | GET | `/Interview` | Halaman persiapan simulasi |
| `Start()` | GET | `/Interview/Start` | Ambil pertanyaan random |
| `SubmitAnswer(questionId, answerText)` | POST | `/Interview/SubmitAnswer` | **Submit jawaban + Proses AI** |
| `Result(id)` | GET | `/Interview/Result/{id}` | Tampilkan hasil penilaian |
| `History()` | GET | `/Interview/History` | Daftar riwayat user |
| `Delete(id)` | POST | `/Interview/Delete/{id}` | Hapus riwayat |

### Alur Simulasi Wawancara (Core Flow)

```
┌──────────────────────────────────────────────────────────────┐
│                    MULAI SIMULASI                            │
└──────────────────────────────┬───────────────────────────────┘
                               ↓
┌──────────────────────────────────────────────────────────────┐
│ 1. Start() - Ambil pertanyaan random dari database          │
│    var question = await _context.Questions                   │
│        .OrderBy(q => Guid.NewGuid())                        │
│        .FirstOrDefaultAsync();                               │
└──────────────────────────────┬───────────────────────────────┘
                               ↓
┌──────────────────────────────────────────────────────────────┐
│ 2. User menjawab pertanyaan di form                          │
└──────────────────────────────┬───────────────────────────────┘
                               ↓
┌──────────────────────────────────────────────────────────────┐
│ 3. SubmitAnswer() - Simpan jawaban ke database               │
│    var answer = new Answer { ... };                          │
│    _context.Answers.Add(answer);                             │
│    await _context.SaveChangesAsync();                        │
└──────────────────────────────┬───────────────────────────────┘
                               ↓
┌──────────────────────────────────────────────────────────────┐
│ 4. Panggil AI untuk scoring                                  │
│    var aiResult = await _mlService.PredictAsync(             │
│        questionText, answerText);                            │
└──────────────────────────────┬───────────────────────────────┘
                               ↓
┌──────────────────────────────────────────────────────────────┐
│ 5. Simpan hasil penilaian ke database                        │
│    var result = new InterviewResult {                        │
│        Score = aiResult.Score,                               │
│        Feedback = aiResult.Feedback                          │
│    };                                                        │
│    _context.InterviewResults.Add(result);                    │
└──────────────────────────────┬───────────────────────────────┘
                               ↓
┌──────────────────────────────────────────────────────────────┐
│ 6. Redirect ke halaman Result                                │
│    return RedirectToAction("Result", new { id = result.Id });│
└──────────────────────────────────────────────────────────────┘
```

### Kode SubmitAnswer (Fungsi Inti)

```csharp
[HttpPost]
public async Task<IActionResult> SubmitAnswer(Guid questionId, string answerText)
{
    // Validasi
    if (string.IsNullOrWhiteSpace(answerText))
    {
        var question = await _context.Questions.FindAsync(questionId);
        ViewBag.Error = "Jawaban tidak boleh kosong.";
        return View("Question", question);
    }

    var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

    // 1. Simpan Jawaban
    var answer = new Answer
    {
        QuestionId = questionId,
        UserId = userId,
        AnswerText = answerText
    };
    _context.Answers.Add(answer);
    await _context.SaveChangesAsync();

    // 2. Panggil AI untuk scoring
    var questionObj = await _context.Questions.FindAsync(questionId);
    var aiResult = await _mlService.PredictAsync(questionObj.QuestionText, answerText);

    // 3. Simpan Hasil
    var result = new InterviewResult
    {
        UserId = userId,
        AnswerId = answer.Id,
        Score = (int)Math.Round(aiResult.Score),
        Feedback = aiResult.Feedback,
        CreatedAt = DateTime.UtcNow
    };
    _context.InterviewResults.Add(result);
    await _context.SaveChangesAsync();

    return RedirectToAction("Result", new { id = result.Id });
}
```

---

## 4. AdminController

**Lokasi:** `Siwate.Web/Controllers/AdminController.cs`  
**Akses:** `[Authorize(Roles = "admin")]` - Hanya admin

### Actions

| Action | HTTP | Route | Deskripsi |
|--------|------|-------|-----------|
| `Index()` | GET | `/Admin` | Dashboard admin |
| `Questions()` | GET | `/Admin/Questions` | Daftar pertanyaan |
| `AddQuestion(questionText)` | POST | `/Admin/AddQuestion` | Tambah pertanyaan baru |
| `DeleteQuestion(id)` | POST | `/Admin/DeleteQuestion` | Hapus pertanyaan |

### Kode

```csharp
[Authorize(Roles = "admin")]
public class AdminController : Controller
{
    private readonly SiwateDbContext _context;
    private readonly IMachineLearningService _mlService;

    public AdminController(SiwateDbContext context, IMachineLearningService mlService)
    {
        _context = context;
        _mlService = mlService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Questions()
    {
        var questions = await _context.Questions
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
        return View(questions);
    }

    [HttpPost]
    public async Task<IActionResult> AddQuestion(string questionText)
    {
        if (!string.IsNullOrWhiteSpace(questionText))
        {
            _context.Questions.Add(new Question { QuestionText = questionText });
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Questions");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteQuestion(Guid id)
    {
        var q = await _context.Questions.FindAsync(id);
        if (q != null)
        {
            _context.Questions.Remove(q);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Questions");
    }
}
```

---

**Sebelumnya:** [← Models](./03-models.md)  
**Selanjutnya:** [Services →](./05-services.md)
