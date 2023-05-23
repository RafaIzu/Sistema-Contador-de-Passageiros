using System.ComponentModel.DataAnnotations.Schema;

namespace BusPrime.Models
{
    [Table("Onibus")]
    public class Onibus
    {
        public Onibus()
        {
            Contagem = new HashSet<Contagem>();
        }

        public int Id { get; set; }
        public string Empresa { get; set; }
        public string Placa { get; set; }
        public int NumeroAssentos { get; set; }
        public virtual ICollection<Contagem> Contagem { get; set; }

    }
}
