using Microsoft.EntityFrameworkCore;
using Siwate.Web.Models;

namespace Siwate.Web.Data
{
    public class SiwateDbContext : DbContext
    {
        public SiwateDbContext(DbContextOptions<SiwateDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<InterviewResult> InterviewResults { get; set; }

    }
}
