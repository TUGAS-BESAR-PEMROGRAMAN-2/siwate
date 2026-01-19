# 🏗 Arsitektur Sistem

## Pola Arsitektur: MVC (Model-View-Controller)

SIWATE menggunakan pola arsitektur **MVC** yang memisahkan aplikasi menjadi tiga komponen utama:

```
┌─────────────────────────────────────────────────────────┐
│                      USER (Browser)                      │
└─────────────────────────┬───────────────────────────────┘
                          │ HTTP Request/Response
                          ▼
┌─────────────────────────────────────────────────────────┐
│                    VIEWS (Razor Pages)                   │
│  • Home/Index.cshtml      • Interview/Question.cshtml   │
│  • Account/Login.cshtml   • Interview/Result.cshtml     │
│  • Admin/Index.cshtml     • Shared/_Layout.cshtml       │
└─────────────────────────┬───────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│                     CONTROLLERS                          │
│  • HomeController       • InterviewController           │
│  • AccountController    • AdminController               │
└─────────────────────────┬───────────────────────────────┘
                          │
          ┌───────────────┴───────────────┐
          ▼                               ▼
┌──────────────────────┐    ┌──────────────────────────────┐
│       SERVICES       │    │          MODELS              │
│  • GeminiService     │    │  • User       • Answer       │
│  • MLService (legacy)│    │  • Question   • InterviewResult│
└──────────┬───────────┘    └──────────────┬───────────────┘
           │                               │
           ▼                               ▼
┌──────────────────────┐    ┌──────────────────────────────┐
│   GOOGLE GEMINI API  │    │    DATABASE (Supabase)       │
│   (External Service) │    │    PostgreSQL                │
└──────────────────────┘    └──────────────────────────────┘
```

---

## 📁 Struktur Folder

```
Siwate/
├── Siwate.sln                 # Solution file
├── README.md                  # Dokumentasi utama
├── docs/                      # Dokumentasi detail (folder ini)
│
└── Siwate.Web/                # Proyek utama
    ├── Controllers/           # Business logic
    │   ├── HomeController.cs
    │   ├── AccountController.cs
    │   ├── InterviewController.cs
    │   └── AdminController.cs
    │
    ├── Data/                  # Database context
    │   └── SiwateDbContext.cs
    │
    ├── Models/                # Entity database
    │   ├── User.cs
    │   ├── Question.cs
    │   ├── Answer.cs
    │   ├── InterviewResult.cs
    │   ├── Dataset.cs
    │   └── ErrorViewModel.cs
    │
    ├── Services/              # AI/ML services
    │   ├── IMachineLearningService.cs
    │   ├── GeminiService.cs
    │   ├── MachineLearningService.cs
    │   ├── ModelInput.cs
    │   └── ModelOutput.cs
    │
    ├── Views/                 # User interface
    │   ├── Home/
    │   ├── Account/
    │   ├── Interview/
    │   ├── Admin/
    │   └── Shared/
    │
    ├── wwwroot/               # Static files
    ├── Program.cs             # Entry point
    └── appsettings.json       # Configuration
```

---

## 🔄 Aliran Data

### Request Flow
```
1. User mengirim HTTP Request (GET/POST)
        ↓
2. Routing (`{controller}/{action}/{id?}`)
        ↓
3. Controller menerima request
        ↓
4. Controller berinteraksi dengan:
   - DbContext (untuk database)
   - Services (untuk AI processing)
        ↓
5. Controller return View dengan Model
        ↓
6. View merender HTML dengan data
        ↓
7. HTML dikirim ke browser User
```

---

## 🔐 Autentikasi

Sistem menggunakan **Cookie-based Authentication**:

```csharp
// Program.cs
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });
```

**Claims yang disimpan:**
- `ClaimTypes.NameIdentifier` - User ID
- `ClaimTypes.Name` - Nama user
- `ClaimTypes.Role` - Role (user/admin)

---

## 🔗 Dependency Injection

```csharp
// Program.cs
// Database
builder.Services.AddDbContext<SiwateDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// AI Service (GeminiService implements IMachineLearningService)
builder.Services.AddScoped<IMachineLearningService, GeminiService>();
```

---

**Sebelumnya:** [← Gambaran Umum](./01-overview.md)  
**Selanjutnya:** [Models →](./03-models.md)
