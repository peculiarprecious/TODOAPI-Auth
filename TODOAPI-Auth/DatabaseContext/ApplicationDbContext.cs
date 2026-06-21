using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TODOAPI_Auth.Models;

namespace TODOAPI_Auth.DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> Options) : base(Options)
        {

        }
        public DbSet<User> Users { get; set; } 
        public DbSet<TodoItem> TodoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One-to-Many: User → Todos
            modelBuilder.Entity<User>()
                .HasMany(u => u.Todos)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade); // delete todos if user deleted

            // Email must be unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.email)
                .IsUnique();
        }
    }

}
