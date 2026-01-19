# 📚 Dokumentasi SIWATE

Selamat datang di dokumentasi lengkap proyek **SIWATE** (Simulasi Wawancara Kerja Berbasis Teks dengan Penilaian Machine Learning).

---

## Daftar Isi

| No | Dokumen | Deskripsi |
|----|---------|-----------|
| 1 | [Gambaran Umum](./01-overview.md) | Pengenalan, tujuan, dan fitur SIWATE |
| 2 | [Arsitektur](./02-architecture.md) | Pola MVC, struktur folder, dependency injection |
| 3 | [Models](./03-models.md) | Entity database (User, Question, Answer, dll) |
| 4 | [Controllers](./04-controllers.md) | Business logic dan HTTP routing |
| 5 | [Services](./05-services.md) | AI layer dan integrasi Google Gemini |
| 6 | [Views](./06-views.md) | User interface dengan Razor + Tailwind |
| 7 | [Alur Kerja](./07-workflow.md) | Diagram lengkap user journey |
| 8 | [Instalasi](./08-installation.md) | Cara setup dan menjalankan aplikasi |

---

## Quick Start

```bash
# Clone
git clone https://github.com/username/SIWATE.git
cd SIWATE/Siwate.Web

# Konfigurasi appsettings.json (isi ConnectionString & GeminiApiKey)

# Jalankan
dotnet restore
dotnet run
```

---

## Teknologi

| Komponen | Teknologi |
|----------|-----------|
| Backend | ASP.NET Core 9.0 (C#) |
| Frontend | Razor Views + Tailwind CSS |
| Database | PostgreSQL (Supabase) |
| AI Engine | Google Gemini API |

---

**Dibuat untuk Tugas Besar Pemrograman**
