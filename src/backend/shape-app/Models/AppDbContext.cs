using Microsoft.EntityFrameworkCore;

namespace shape_app.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base (options)
        {    
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TreinoExercicio>()
                .HasKey(c => new { c.TreinoId, c.ExercicioId });

            builder.Entity<TreinoExercicio>()
                .HasOne(c => c.Treino).WithMany(c => c.Exercicios)
                .HasForeignKey(c => c.TreinoId);

            builder.Entity<TreinoExercicio>()
            .HasOne(c => c.Exercicio).WithMany(c => c.Treinos)
            .HasForeignKey(c => c.ExercicioId);
        }

        public DbSet<Exercicio> Exercicios { get; set; }
        public DbSet<Treino> Treinos { get; set; }
        public DbSet<TreinoExercicio> TreinoExercicios { get; set; }
    }
}
