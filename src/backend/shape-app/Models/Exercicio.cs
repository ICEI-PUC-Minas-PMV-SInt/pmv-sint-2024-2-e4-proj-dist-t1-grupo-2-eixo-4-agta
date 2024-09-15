using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace shape_app.Models
{
    [Table("Exercicios")]
    public class Exercicio
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public int Series { get; set; }
        [Required]
        [Display(Name = "Repetições")]
        public int Repeticoes { get; set; }
    }
}
