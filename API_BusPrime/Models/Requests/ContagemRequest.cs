using System.ComponentModel.DataAnnotations.Schema;

namespace BusPrime.Models
{
    public class ContagemRequest
    {
        public int contP { get; set; }
        public int idBus { get; set; }
        public int idMot { get; set; }
    }

}
