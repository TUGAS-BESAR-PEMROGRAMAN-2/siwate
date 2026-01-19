# 🛠 Instalasi & Cara Menjalankan

## Prasyarat

- ✅ **.NET SDK 9.0** atau lebih baru
- ✅ **Akun Supabase** (untuk database PostgreSQL)
- ✅ **API Key Google Gemini** (untuk fitur AI)

---

## Langkah-langkah

### 1. Clone Repository

```bash
git clone https://github.com/username/SIWATE.git
cd SIWATE/Siwate.Web
```

### 2. Konfigurasi Database (Supabase)

1. Buat project baru di [Supabase](https://supabase.com)
2. Jalankan SQL untuk membuat tabel (lihat README utama)
3. Ubah `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.xxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

### 3. Konfigurasi API Key Gemini

1. Dapatkan API Key di [Google AI Studio](https://aistudio.google.com/)
2. Tambahkan ke `appsettings.json`:

```json
{
  "GeminiApiKey": "AIzaSy..."
}
```

### 4. Restore Dependency

```bash
dotnet restore
```

### 5. Jalankan Aplikasi

```bash
dotnet run
```

Buka browser: `http://localhost:5xxx`

---

## Login Awal (Admin)

- **Email:** `admin@siwate.com`
- **Password:** `admin`

---

## Catatan Keamanan

> ⚠️ Password disimpan plain-text (hanya untuk simulasi). Untuk produksi, gunakan BCrypt/Argon2 hashing.

---

**Sebelumnya:** [← Alur Kerja](./07-workflow.md)
