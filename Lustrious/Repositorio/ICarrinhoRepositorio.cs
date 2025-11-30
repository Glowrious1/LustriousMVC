using Lustrious.Models;

namespace Lustrious.Repositorio
{
    public interface ICarrinhoRepositorio
    {
        /// <summary>
        /// Adiciona o item ao carrinho. Retorna true se adicionado com sucesso (estoque disponível), false caso contrário.
        /// </summary>
        /// <param name="produtoId">O ID do produto a ser adicionado.</param>
        /// <param name="userId">O ID do usuário dono do carrinho.</param>
        /// <returns>Retorna um valor booleano indicando o sucesso da operação.</returns>
        public bool AdicionarItem(int produtoId, int userId);
        public void RemoverItem(int produtoId, int userId);
        public void LimparCarrinho(int userId);
        public void FinalizarCompra(int idEnd, int userId);
        public Carrinho AcharCarrinho(int userId);

        // Novos métodos para ajustar quantidade
        public bool IncrementarItem(int produtoId, int userId);
        public bool DecrementarItem(int produtoId, int userId);
    }
}
