# Simulasi Wawancara Kerja Berbasis Teks dengan AI (SIWATE)

**SIWATE** adalah aplikasi web modern yang dirancang untuk membantu pelamar kerja berlatih wawancara. Aplikasi ini mensimulasikan sesi wawancara berbasis teks dan menggunakan **Google Gemini AI** untuk memberikan penilaian skor dan umpan balik (*feedback*) otomatis terhadap jawaban pengguna.

---

## 📋 Daftar Isi
1. [Deskripsi Umum](#-deskripsi-umum)
2. [Fitur Utama](#-fitur-utama)
3. [Arsitektur Sistem](#-arsitektur-sistem)
4. [AI Engine (Google Gemini)](#-ai-engine-google-gemini)
5. [Teknologi](#-teknologi)
6. [Struktur Project](#-struktur-project)
7. [Database (Supabase)](#-database-supabase)
8. [Instalasi & Cara Menjalankan](#-instalasi--cara-menjalankan)
9. [Catatan Penting](#-catatan-penting)

---

## 📖 Deskripsi Umum

Dalam dunia kerja yang kompetitif, persiapan wawancara adalah kunci. SIWATE hadir sebagai platform latihan mandiri di mana pengguna dapat:
1.  Menjawab pertanyaan wawancara yang umum ditanyakan HRD.
2.  Mendapatkan skor relevansi jawaban secara instan (0-100).
3.  Menerima saran perbaikan untuk meningkatkan kualitas jawaban.

Sistem ini dibangun dengan **ASP.NET Core MVC** untuk performa tinggi dan **Google Gemini API** untuk kemampuan analisis cerdas berbasis Large Language Model (LLM).

---

## 🚀 Fitur Utama

### 👨‍💼 Modul User (Pelamar)
*   **Simulasi Wawancara**: Menjawab pertanyaan acak dari database.
*   **Penilaian AI Otomatis**: Mendapatkan skor dan feedback langsung setelah submit menggunakan Google Gemini AI.
*   **Riwayat Latihan**: Melihat kembali hasil simulasi sebelumnya untuk melacak kemajuan.
*   **Antarmuka Modern**: Desain responsif dan bersih dengan Tailwind CSS.

### 👮 Modul Admin (Dashboard)
*   **Dashboard Statistik**: Melihat jumlah pertanyaan, pengguna, dan status AI Engine.
*   **Manajemen Pertanyaan**: Menambah, mengedit, atau menghapus bank soal wawancara.

---

## 🏗 Arsitektur Sistem

Sistem ini menggunakan arsitektur **MVC (Model-View-Controller)** yang terstruktur:

*   **Frontend**: Razor Views (.cshtml) dengan styling **Tailwind CSS**.
*   **Backend**: ASP.NET Core 9.0 (C#).
*   **Database**: PostgreSQL (di-hosting di **Supabase**), diakses menggunakan Entity Framework Core (EF Core).
*   **AI Engine**: Google Gemini API yang terhubung melalui REST API.

---

## 🤖 AI Engine (Google Gemini)

Aplikasi ini menggunakan **Generative AI (LLM)** melalui Google Gemini API untuk analisis jawaban yang lebih cerdas dan kontekstual.

### Keunggulan
*   **Pemahaman Konteks**: AI memahami makna pertanyaan dan jawaban, bukan hanya mencocokkan kata kunci.
*   **Deteksi Bahasa**: Sistem dapat mendeteksi jika jawaban tidak menggunakan Bahasa Indonesia yang baik dan benar.
*   **Anti-Cheating**: AI dilatih untuk mendeteksi jawaban asal-asalan (gibberish) meskipun panjang.
*   **No Training Required**: Menggunakan model pre-trained dari Google, tidak perlu melatih model sendiri.

### Alur Penilaian AI
1.  User submit jawaban.
2.  Sistem mengirimkan **Prompt** khusus ke Gemini API yang berisi:
    *   Peran: "Asisten HRD Profesional"
    *   Teks Pertanyaan
    *   Teks Jawaban Kandidat
    *   Instruksi Penilaian (Validasi bahasa, relevansi, metode STAR)
3.  Gemini API mengembalikan respons dalam format **JSON** yang berisi:
    *   `score`: Nilai numerik (0-100)
    *   `feedback`: Saran perbaikan yang spesifik dan membangun.

---

## 💻 Teknologi

*   **Framework**: ASP.NET Core 9.0
*   **Language**: C#
*   **Database**: PostgreSQL 15 (Supabase)
*   **ORM**: Entity Framework Core 9.0
*   **AI Engine**: Google Gemini API (Model: gemini-3-flash-preview)
*   **HTTP Client**: System.Net.Http
*   **CSS Framework**: Tailwind CSS (via CDN)
*   **IDE**: Visual Studio Code / Visual Studio 2022

---

## 📂 Struktur Project

```
Siwate.Web/
├── Controllers/        # Logika Bisnis (Interview, Admin, Account)
├── Data/               # Konfigurasi EF Core (DbContext)
├── Models/             # Representasi Tabel Database (User, Question, dll)
├── Services/           # Logika AI (GeminiService.cs)
│   ├── GeminiService.cs         # Integrasi Google Gemini API
│   ├── IMachineLearningService.cs # Interface untuk AI Service
│   └── ...
├── Views/              # Tampilan Antarmuka (Razor Pages)
│   ├── Admin/          # Dashboard Admin
│   ├── Interview/      # Halaman Simulasi Wawancara
│   ├── Account/        # Login & Register
│   └── Shared/         # Layout & Partial Views
├── wwwroot/            # File Statis (CSS, JS, Images)
├── appsettings.json    # Konfigurasi Database & API Key
└── Program.cs          # Entry Point & Dependency Injection
```

---

## 🗄 Database (Supabase)

Proyek ini menggunakan **Supabase** sebagai layanan Backend-as-a-Service (BaaS) untuk database PostgreSQL.

**Tabel Utama:**
1.  `users`: Menyimpan data akun (Admin/User).
2.  `questions`: Bank soal wawancara.
3.  `answers`: Jawaban yang disubmit user beserta skor dan feedback.
4.  `interview_results`: Hasil penilaian (Skor & Feedback).

---

## 🛠 Instalasi & Cara Menjalankan

### Prasyarat
*   .NET SDK 9.0 (atau lebih baru).
*   Akun **Supabase** (untuk database).
*   **Google Gemini API Key** (untuk fitur penilaian AI).

### Langkah-langkah

1.  **Clone Repository**
    ```bash
    git clone https://github.com/username/SIWATE.git
    cd SIWATE/Siwate.Web
    ```

2.  **Konfigurasi Database & API Key**
    *   Buat project baru di Supabase.
    *   Salin file `appsettings.Example.json` menjadi `appsettings.json`.
    *   Sesuaikan konfigurasi:
        ```json
        {
            "ConnectionStrings": {
                "DefaultConnection": "Host=db.YOUR_PROJECT_ID.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR_PASSWORD"
            },
            "Supabase": {
                "Url": "https://YOUR_PROJECT_ID.supabase.co",
                "Key": "YOUR_SUPABASE_ANON_KEY",
                "ServiceRole": "YOUR_SUPABASE_SERVICE_ROLE_KEY"
            },
            "GeminiApiKey": "YOUR_GEMINI_API_KEY"
        }
        ```

3.  **Restore Dependency**
    ```bash
    dotnet restore
    ```

4.  **Jalankan Aplikasi**
    ```bash
    dotnet run
    ```
    Buka browser dan akses `http://localhost:5155`.

5.  **Login Awal (Admin)**
    *   Email: `admin@siwate.com`
    *   Password: `admin`

---

## ⚠️ Catatan Penting

1.  **Konfigurasi API Key**:
    Aplikasi memerlukan API Key Google Gemini agar fitur penilaian berfungsi.
    *   Dapatkan API Key di [Google AI Studio](https://aistudio.google.com/).
    *   Masukkan Key ke `appsettings.json` pada bagian `GeminiApiKey`.

2.  **Keamanan**:
    Password user saat ini disimpan dalam bentuk *plain-text* untuk tujuan simulasi/pembelajaran. Untuk produksi, **WAJIB** menggunakan hashing (BCrypt/Argon2).

3.  **Rate Limit**:
    Google Gemini API memiliki batas request per menit. Pastikan tidak menggunakan secara berlebihan.

---

## 📄 Lisensi

Proyek ini dibuat untuk tujuan tugas besar perkuliahan (PBO - Pemrograman Berorientasi Objek).

---
*Dibuat dengan ❤️ oleh Tim Pengembang SIWATE*
