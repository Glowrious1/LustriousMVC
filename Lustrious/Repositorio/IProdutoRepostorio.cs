using Lustrious.Models;

namespace Lustrious.Repositorio
{
    public interface IProdutoRepostorio
    {
        public void CadastrarProduto(Produto produto);
        public Produto AcharProduto(int id);
        public IEnumerable<Produto> ListarProdutos();
        public void AlterarProduto(Produto produto);
        public void ExcluirProduto(int id);
    }
}
