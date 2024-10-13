using System.ComponentModel.DataAnnotations.Schema;

namespace shape_app.Models
{
    [Table("TreinoExercicios")]
    public class TreinoExercicio
    {
        public int TreinoId { get; set; }
        public Treino Treino { get; set; }
        public int ExercicioId { get; set; }
        public Exercicio Exercicio { get; set; }
    }
}
