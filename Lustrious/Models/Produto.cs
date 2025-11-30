using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lustrious.Models
{
    public class Produto
    {
        public long CodigoBarras { get; set; }
        public string NomeProd { get; set; }
        public int Qtd { get; set; }

        public string? Foto { get; set; }

        public string Genero { get; set; }
        public string Descricao { get; set; }
        public decimal ValorUnitario { get; set; }

        public int? CodCategoria { get; set; }
        public int? CodTipoProduto { get; set; }

        public string? NomeCategoria { get; set; }
        public string? NomeTipoProduto { get; set; }

        public List<SelectListItem> CategoriaNome { get; set; } = new();
        public List<SelectListItem> TipoProdutoNome { get; set; } = new();
    }
}
