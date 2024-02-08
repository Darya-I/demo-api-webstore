using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using WebApiDemo_ML_lesson.Models;

namespace WebApiDemo_ML_lesson.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
            
        }
    }
}
