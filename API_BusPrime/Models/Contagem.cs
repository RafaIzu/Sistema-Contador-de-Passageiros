using System.ComponentModel.DataAnnotations.Schema;

namespace BusPrime.Models
{
    [Table("Contagem")]
    public class Contagem
    {
        public int Id { get; set; }
        public int Onibus_Id { get; set; }
        public int Motorista_Id { get; set; }
        public DateTime DataContagem { get; set; }
        public int TotalPessoas { get; set; }

        public virtual Motorista Motorista { get; set; }
        public virtual Onibus Onibus { get; set; }
    }
}
