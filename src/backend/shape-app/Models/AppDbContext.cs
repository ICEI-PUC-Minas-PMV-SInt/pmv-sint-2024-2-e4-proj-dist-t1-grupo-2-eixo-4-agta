using Microsoft.EntityFrameworkCore;

namespace shape_app.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base (options)
        {    
        }

        public DbSet<Exercicio> Exercicios { get; set; }

    }
}
