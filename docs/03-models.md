# 📊 Models (Entity Database)

## Diagram Relasi Database

```
┌──────────────┐       ┌──────────────┐       ┌──────────────────┐
│    users     │       │  questions   │       │     datasets     │
├──────────────┤       ├──────────────┤       ├──────────────────┤
│ id (PK)      │       │ id (PK)      │       │ id (PK)          │
│ name         │       │ question_text│       │ answer_text      │
│ email        │       │ created_at   │       │ score            │
│ password     │       └──────────────┘       │ created_at       │
│ role         │              │               └──────────────────┘
│ created_at   │              │
└──────────────┘              │
       │                      │
       │    ┌─────────────────┴─────────────────┐
       │    │                                   │
       ▼    ▼                                   │
┌──────────────────┐                            │
│     answers      │                            │
├──────────────────┤                            │
│ id (PK)          │←───────────────────────────┤
│ user_id (FK)     │→ users.id                  │
│ question_id (FK) │→ questions.id              │
│ answer_text      │                            │
│ created_at       │                            │
└──────────────────┘                            │
       │                                        │
       ▼                                        │
┌──────────────────────┐                        │
│  interview_results   │                        │
├──────────────────────┤                        │
│ id (PK)              │                        │
│ user_id (FK)         │→ users.id              │
│ answer_id (FK)       │→ answers.id            │
│ score                │                        │
│ feedback             │                        │
│ created_at           │                        │
└──────────────────────┘                        
```

---

## 📝 Detail Setiap Model

### 1. User.cs
**Lokasi:** `Siwate.Web/Models/User.cs`  
**Tabel:** `users`

```csharp
[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("name")]
    public string Name { get; set; }

    [Required]
    [Column("email")]
    public string Email { get; set; }

    [Required]
    [Column("password")]
    public string Password { get; set; }

    [Column("role")]
    public string Role { get; set; } = "user";  // "user" atau "admin"

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

| Kolom | Tipe | Deskripsi |
|-------|------|-----------|
| Id | Guid | Primary key, auto-generated |
| Name | string | Nama lengkap pengguna |
| Email | string | Email (unique) untuk login |
| Password | string | Password (plain text - hanya untuk simulasi) |
| Role | string | "user" atau "admin" |
| CreatedAt | DateTime | Waktu registrasi |

---

### 2. Question.cs
**Lokasi:** `Siwate.Web/Models/Question.cs`  
**Tabel:** `questions`

```csharp
[Table("questions")]
public class Question
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("question_text")]
    public string QuestionText { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

| Kolom | Tipe | Deskripsi |
|-------|------|-----------|
| Id | Guid | Primary key |
| QuestionText | string | Teks pertanyaan wawancara |
| CreatedAt | DateTime | Waktu dibuat |

---

### 3. Answer.cs
**Lokasi:** `Siwate.Web/Models/Answer.cs`  
**Tabel:** `answers`

```csharp
[Table("answers")]
public class Answer
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("user_id")]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    [Column("question_id")]
    public Guid QuestionId { get; set; }

    [ForeignKey("QuestionId")]
    public Question Question { get; set; }

    [Required]
    [Column("answer_text")]
    public string AnswerText { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

| Kolom | Tipe | Deskripsi |
|-------|------|-----------|
| Id | Guid | Primary key |
| UserId | Guid | Foreign key ke users |
| QuestionId | Guid | Foreign key ke questions |
| AnswerText | string | Teks jawaban user |
| CreatedAt | DateTime | Waktu submit |

---

### 4. InterviewResult.cs
**Lokasi:** `Siwate.Web/Models/InterviewResult.cs`  
**Tabel:** `interview_results`

```csharp
[Table("interview_results")]
public class InterviewResult
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("user_id")]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    [Column("answer_id")]
    public Guid AnswerId { get; set; }

    [ForeignKey("AnswerId")]
    public Answer Answer { get; set; }

    [Column("score")]
    [Range(0, 100)]
    public int Score { get; set; }

    [Column("feedback")]
    public string Feedback { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

| Kolom | Tipe | Deskripsi |
|-------|------|-----------|
| Id | Guid | Primary key |
| UserId | Guid | Foreign key ke users |
| AnswerId | Guid | Foreign key ke answers |
| Score | int | Skor 0-100 dari AI |
| Feedback | string | Feedback/saran dari AI |
| CreatedAt | DateTime | Waktu evaluasi |

---

### 5. Dataset.cs (Legacy)
**Lokasi:** `Siwate.Web/Models/Dataset.cs`  
**Tabel:** `datasets`

```csharp
[Table("datasets")]
public class Dataset
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("answer_text")]
    public string AnswerText { get; set; }

    [Column("score")]
    [Range(0, 100)]
    public int Score { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

> ⚠️ **Note:** Model ini adalah legacy untuk ML.NET training. Saat ini tidak aktif digunakan karena telah diganti dengan Google Gemini API.

---

### 6. ErrorViewModel.cs
**Lokasi:** `Siwate.Web/Models/ErrorViewModel.cs`

```csharp
public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
```

Model sederhana untuk menampilkan informasi error.

---

## 🔗 DbContext

**Lokasi:** `Siwate.Web/Data/SiwateDbContext.cs`

```csharp
public class SiwateDbContext : DbContext
{
    public SiwateDbContext(DbContextOptions<SiwateDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<InterviewResult> InterviewResults { get; set; }
    public DbSet<Dataset> Datasets { get; set; }
}
```

---

**Sebelumnya:** [← Arsitektur](./02-architecture.md)  
**Selanjutnya:** [Controllers →](./04-controllers.md)
