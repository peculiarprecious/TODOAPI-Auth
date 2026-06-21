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
    }

}
