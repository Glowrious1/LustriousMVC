namespace Lustrious.Models
{
    public class Carrinho
    {
        public int IdCarrinho { get; set; }
        public int IdProd { get; set; }
        public int Qtd { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorUnitario { get; set; }

        // Lista de itens no carrinho
        public List<VendaProduto> Items { get; set; } = new List<VendaProduto>();
    }
}
