using Lustrious.Models;
namespace Lustrious.Repositorio
{
    public interface IFavoritosRepositorio
    {
        public void AdicionarFavorito(int produtoId, int userId);
        public void RemoverFavorito(int produtoId, int userId);
        public void LimparFavoritos(int userId);
        public IEnumerable<Produto> AcharFavoritos(int userId);
    }
}
