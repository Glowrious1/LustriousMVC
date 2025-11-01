using Lustrious.Data;
using Lustrious.Models;
using MySql.Data.MySqlClient;

namespace Lustrious.Repositorio
{
    //Completar depois
    public class FavoritosRepositorio : IFavoritosRepositorio
    {
        private readonly DataBase _dataBase;
        public FavoritosRepositorio(DataBase dataBase)
        {
            _dataBase = dataBase;
        }
        
        public void AdicionarFavorito(int produtoId, int userId)
        {
            throw new NotImplementedException();
        }

        public void LimparFavoritos(int userId)
        {
            throw new NotImplementedException();
        }

        public void RemoverFavorito(int produtoId, int userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Produto> AcharFavoritos(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
