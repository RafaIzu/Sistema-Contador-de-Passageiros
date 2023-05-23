using System.ComponentModel.DataAnnotations.Schema;

namespace BusPrime.Models
{
    [Table("Motorista")]
    public class Motorista
    {
        public Motorista()
        {
            Contagem = new HashSet<Contagem>();
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public string RG { get; set; }
        public string CNH { get; set; }
        public virtual ICollection<Contagem> Contagem { get; set; }
    }

}
