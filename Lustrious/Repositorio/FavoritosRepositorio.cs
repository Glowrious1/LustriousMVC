using Lustrious.Data;
using Lustrious.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Lustrious.Repositorio
{
    public class FavoritosRepositorio : IFavoritosRepositorio
    {
        private readonly DataBase _dataBase;
        public FavoritosRepositorio(DataBase dataBase)
        {
            _dataBase = dataBase;
        }
        
        public void AdicionarFavorito(int produtoId, int userId)
        {
            using var conn = _dataBase.GetConnection();
            conn.Open();

            // Usando nomes de colunas conforme Models/Favoritos.cs: id_prod, id_user
            var sql = "INSERT  INTO favoritos (id_prod, id_user) VALUES (@produtoId, @userId);";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@produtoId", produtoId);
            cmd.Parameters.AddWithValue("@userId", userId);

            cmd.ExecuteNonQuery();
        }

        public void LimparFavoritos(int userId)
        {
            using var conn = _dataBase.GetConnection();
            conn.Open();

            var sql = "DELETE FROM favoritos WHERE id_user = @userId;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@userId", userId);

            cmd.ExecuteNonQuery();
        }

        public void RemoverFavorito(int produtoId, int userId)
        {
            using var conn = _dataBase.GetConnection();
            conn.Open();

            var sql = "DELETE FROM favoritos WHERE id_prod = @produtoId AND id_user = @userId;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@produtoId", produtoId);
            cmd.Parameters.AddWithValue("@userId", userId);

            cmd.ExecuteNonQuery();
        }

        public IEnumerable<Produto> AcharFavoritos(int userId)
        {
            var lista = new List<Produto>();

            using var conn = _dataBase.GetConnection();
            conn.Open();

            // Seleciona campos mapeando para as propriedades da classe Produto
            var sql = @"SELECT p.codigo_barras AS CodigoBarras,
                        p.nome_prod AS NomeProd,
                        p.qtd AS qtd,
                        p.genero AS Genero,
                        p.descricao AS Descricao,
                        p.valor_unitario AS ValorUnitario
                        FROM produtos p
                        INNER JOIN favoritos f ON p.id = f.id_prod
                        WHERE f.id_user = @userId;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@userId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var produto = new Produto
                {
                    CodigoBarras = reader["CodigoBarras"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CodigoBarras"]),
                    NomeProd = reader["NomeProd"] == DBNull.Value ? string.Empty : reader["NomeProd"].ToString(),
                    Qtd = reader["qtd"] == DBNull.Value ? 0 : Convert.ToInt32(reader["qtd"]),
                    Genero = reader["Genero"] == DBNull.Value ? string.Empty : reader["Genero"].ToString(),
                    Descricao = reader["Descricao"] == DBNull.Value ? string.Empty : reader["Descricao"].ToString(),
                    ValorUnitario = reader["ValorUnitario"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["ValorUnitario"])
                };

                lista.Add(produto);
            }

            return lista;
        }
    }
}
