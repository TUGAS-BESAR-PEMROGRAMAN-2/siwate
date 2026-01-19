# 🎨 Views (User Interface)

Views adalah layer presentasi yang merender HTML menggunakan **Razor Pages (.cshtml)** dengan **Tailwind CSS**.

---

## Struktur Folder Views

```
Views/
├── _ViewImports.cshtml      # Import namespace
├── _ViewStart.cshtml        # Default layout
├── Shared/                  # Komponen bersama
│   ├── _Layout.cshtml       # Layout utama user
│   ├── _AdminLayout.cshtml  # Layout admin
│   └── Error.cshtml         # Halaman error
├── Home/                    # Halaman publik
│   ├── Index.cshtml         # Landing page
│   └── Privacy.cshtml       
├── Account/                 # Autentikasi
│   ├── Login.cshtml         # Form login
│   └── Register.cshtml      # Form registrasi
├── Interview/               # Simulasi wawancara
│   ├── Index.cshtml         # Halaman persiapan
│   ├── Question.cshtml      # Form pertanyaan
│   ├── Result.cshtml        # Hasil penilaian
│   └── History.cshtml       # Riwayat simulasi
└── Admin/                   # Dashboard admin
    ├── Index.cshtml         
    └── Questions.cshtml     # Kelola pertanyaan
```

---

## Detail Setiap View

### 1. Shared/_Layout.cshtml
Layout master dengan Navbar, Footer, dan `@RenderBody()` untuk konten.

### 2. Home/Index.cshtml  
Landing page dengan Hero Section, Features Cards, dan CTA Section.

### 3. Account/Login.cshtml & Register.cshtml
Form autentikasi dengan validasi.

### 4. Interview/Index.cshtml
Instruksi persiapan dengan tips metode STAR.

### 5. Interview/Question.cshtml
Form untuk menjawab pertanyaan wawancara.

### 6. Interview/Result.cshtml
Hasil penilaian dengan skor visual dan feedback AI.

### 7. Interview/History.cshtml
Daftar riwayat simulasi dengan opsi hapus.

### 8. Admin/Questions.cshtml
CRUD pertanyaan wawancara.

---

## Teknologi UI

| Komponen | Teknologi |
|----------|-----------|
| Template | Razor (.cshtml) |
| CSS | Tailwind CSS (CDN) |
| Font | Google Fonts (Inter) |
| Icons | Heroicons (SVG) |

---

**Sebelumnya:** [← Services](./05-services.md)  
**Selanjutnya:** [Alur Kerja Lengkap →](./07-workflow.md)
