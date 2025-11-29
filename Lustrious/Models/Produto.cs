namespace Lustrious.Models
{
    public class Produto
    {
        public int CodigoBarras { get; set; }
        public string NomeProd { get; set; }
        public int qtd { get; set; }
       
        public string foto { get; set; }

        public string Genero { get; set; }
        public string Descricao { get; set; }
        public decimal ValorUnitario { get; set; }

        public int codCategoria  { get ; set ;}

        public int codTipoProduto { get; set; }



        public List<Produto>? ListarProdutos { get; set; }
    }
}
