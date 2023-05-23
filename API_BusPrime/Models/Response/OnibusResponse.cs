using System.ComponentModel.DataAnnotations.Schema;

namespace BusPrime.Models.Response
{
    public class OnibusResponse
    {
        public int Id { get; set; }
        public string Empresa { get; set; }
        public string Placa { get; set; }

    }
}
