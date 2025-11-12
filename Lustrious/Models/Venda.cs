namespace Lustrious.Models
{
    public class Venda
    {
        public int IdVenda { get; set; }
        public string NomeProd { get; set; }
        public int IdUser { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal ValorTotal { get; set; }
        public int NF { get; set; }
        public int IdEntrega { get; set; }
    }
}
