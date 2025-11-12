namespace Lustrious.Models
{
    public class Entrega
    {
        public int IdEntrega { get; set; }

        public int IdEndereco { get; set; }

        public DateTime DataEntrega { get; set; }
        
        public  decimal ValorFrete  { get; set; }

        public DateTime DataPrevista { get; set; }

        public string Status { get; set; }
    }
}
