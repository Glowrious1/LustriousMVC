using Lustrious.Models;

namespace Lustrious.Repositorio
{
    public interface ICarrinhoRepositorio
    {
        public void AdicionarItem(int produtoId, int userId);
        public void RemoverItem(int produtoId, int userId);
        public void LimparCarrinho(int userId);
        public void FinalizarCompra(int idEnd, int userId);
        public Carrinho AcharCarrinho(int userId);
    }
}
