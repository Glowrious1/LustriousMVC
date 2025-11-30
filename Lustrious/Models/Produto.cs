namespace Lustrious.Models
{
    public class Produto
    {
        // DB uses BIGINT for CodigoBarras
        public long CodigoBarras { get; set; }
        public string NomeProd { get; set; }
        public int Qtd { get; set; }

        // Store relative path like "fotosProduto/filename.png"
        public string? Foto { get; set; }

        public string Genero { get; set; }
        public string Descricao { get; set; }
        public decimal ValorUnitario { get; set; }

        // Foreign keys
        public int? CodCategoria { get; set; }
        public int? CodTipoProduto { get; set; }

        // Optional display helpers filled by repository when joining
        public string? NomeCategoria { get; set; }
        public string? NomeTipoProduto { get; set; }
    }
}
