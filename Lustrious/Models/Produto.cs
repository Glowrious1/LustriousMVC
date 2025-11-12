namespace Lustrious.Models
{
    public class Produto
    {
        public int CodigoBarras { get; set; }
        public string NomeProd { get; set; }
        public int qtd { get; set; }

        public string Genero { get; set; }
        public string Descricao { get; set; }
        public decimal ValorUnitario { get; set; }
    }
}
