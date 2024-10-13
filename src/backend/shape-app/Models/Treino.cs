using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace shape_app.Models
{
    [Table("Treinos")]
    public class Treino
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public DateTime Data { get; set; }

        public ICollection<TreinoExercicio> Exercicios { get; set; }

    }
}
