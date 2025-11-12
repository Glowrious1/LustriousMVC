using System.Numerics;

namespace Lustrious.Models
{
    public class VendaProduto
    {
        public decimal ValorItem { get; set; }

        public int Qtd { get; set; }

        public BigInteger CodigoBarras { get; set; }

        public int IdVenda { get; set; }
    }
}
