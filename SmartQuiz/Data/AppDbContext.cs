using Microsoft.EntityFrameworkCore;
using SmartQuiz.Models;

namespace SmartQuiz.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // ✅ Your Users table
        public DbSet<User> Users => Set<User>();
    }
}
