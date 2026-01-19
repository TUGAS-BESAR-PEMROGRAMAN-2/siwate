# 📋 Gambaran Umum SIWATE

## Apa itu SIWATE?

**SIWATE** (Simulasi Wawancara Kerja Berbasis Teks dengan Penilaian Machine Learning) adalah aplikasi web yang dirancang untuk membantu pelamar kerja berlatih wawancara. 

Aplikasi ini mensimulasikan sesi wawancara berbasis teks dan menggunakan **kecerdasan buatan (AI)** untuk memberikan:
- ✅ **Skor penilaian** (0-100)
- ✅ **Feedback konstruktif** untuk perbaikan jawaban

---

## 🎯 Tujuan Aplikasi

1. Membantu pelamar kerja mempersiapkan diri menghadapi wawancara
2. Memberikan pengalaman latihan yang realistis dengan pertanyaan HRD
3. Menyediakan penilaian objektif menggunakan AI
4. Memberikan saran perbaikan yang actionable

---

## 👥 Target Pengguna

| Role | Deskripsi |
|------|-----------|
| **User (Pelamar)** | Menjawab pertanyaan wawancara, melihat skor dan feedback, melacak progress |
| **Admin** | Mengelola bank soal pertanyaan wawancara |

---

## 🚀 Fitur Utama

### Untuk User
- 🎤 Simulasi wawancara dengan pertanyaan random
- 🤖 Penilaian otomatis oleh AI (Google Gemini)
- 📊 Skor 0-100 dengan feedback detail
- 📜 Riwayat latihan untuk tracking progress
- 🗑️ Hapus riwayat yang tidak diperlukan

### Untuk Admin
- ➕ Tambah pertanyaan baru
- 📋 Lihat daftar semua pertanyaan
- ❌ Hapus pertanyaan

---

## 💡 Teknologi Inti

| Komponen | Teknologi |
|----------|-----------|
| Backend | ASP.NET Core 9.0 (C#) |
| Frontend | Razor Views + Tailwind CSS |
| Database | PostgreSQL (Supabase) |
| AI Engine | Google Gemini API |
| ORM | Entity Framework Core |

---

## 📂 File Terkait

- [README.md](../README.md) - Dokumentasi utama proyek
- [Program.cs](../Siwate.Web/Program.cs) - Entry point aplikasi
- [appsettings.json](../Siwate.Web/appsettings.json) - Konfigurasi

---

**Selanjutnya:** [Arsitektur Sistem →](./02-architecture.md)
