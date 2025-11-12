using Lustrious.Data;
using Lustrious.Models;
using MySql.Data.MySqlClient;

namespace Lustrious.Repositorio
{
    //Completar depois
    public class CarrinhoRepositorio : ICarrinhoRepositorio
    {
        private readonly DataBase _dataBase;
        public CarrinhoRepositorio(DataBase dataBase)
        {
            _dataBase = dataBase;
        }
        public void AdicionarItem(int produtoId, int userId)
        {
            using(var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using(var cmd = new MySqlCommand("insertCarrinho"))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vUserId", userId);
                    cmd.Parameters.AddWithValue("vProdutoId", produtoId);
                    cmd.ExecuteNonQuery();
                }
            }
            throw new NotImplementedException();
        }
        public Carrinho AcharCarrinho(int userId)
        {
            throw new NotImplementedException();
        }
        public void FinalizarCompra(int idEnd, int userId)
        {
            throw new NotImplementedException();
        }
        public void LimparCarrinho(int userId)
        {
            throw new NotImplementedException();
        }
        public void RemoverItem(int produtoId, int userId)
        {
            throw new NotImplementedException();
        }
    }
}
