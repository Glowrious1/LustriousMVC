using Lustrious.Models;
using System.Collections.Generic;

namespace Lustrious.Repositorio
{
    public interface IProdutoRepostorio
    {
        public void CadastrarProduto(Produto produto);
        public Produto AcharProduto(int id);
        // Ajustado para permitir filtro por tipo de produto; padrão0 retorna todos dependendo da procedure
        public IEnumerable<Produto> ListarProdutos(int codTipoProduto =0);
        public void AlterarProduto(Produto produto);
        public void ExcluirProduto(int id);
    }
}
