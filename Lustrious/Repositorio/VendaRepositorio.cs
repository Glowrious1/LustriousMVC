using Lustrious.Data;
using Lustrious.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Lustrious.Repositorio
{
    public class VendaRepositorio : IVendaRepositorio
    {
        private readonly DataBase _dataBase;
        public VendaRepositorio(DataBase dataBase)
        {
            _dataBase = dataBase;
        }

        public void RegistrarVenda(Venda venda, IEnumerable<VendaProduto> itens)
        {
            using var conn = _dataBase.GetConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();
            try
            {
                // Inserir venda (assumindo stored procedure insertVenda que retorna id via SELECT LAST_INSERT_ID())
                using (var cmd = new MySqlCommand("insertVenda", conn, transaction))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vIdUser", venda.IdUser);
                    cmd.Parameters.AddWithValue("vDataVenda", venda.DataVenda);
                    cmd.Parameters.AddWithValue("vValorTotal", venda.ValorTotal);
                    cmd.Parameters.AddWithValue("vNF", venda.NF);
                    cmd.Parameters.AddWithValue("vIdEntrega", venda.IdEntrega);
                    // Execute and get inserted id
                    cmd.ExecuteNonQuery();
                }

                // Obter id da venda inserida
                int idVenda;
                using (var cmd = new MySqlCommand("SELECT LAST_INSERT_ID();", conn, transaction))
                {
                    idVenda = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Inserir itens
                foreach (var item in itens)
                {
                    using var cmd = new MySqlCommand("insertVendaProduto", conn, transaction);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vIdVenda", idVenda);
                    cmd.Parameters.AddWithValue("vCodigoBarras", item.CodigoBarras.ToString());
                    cmd.Parameters.AddWithValue("vQtd", item.Qtd);
                    cmd.Parameters.AddWithValue("vValorItem", item.ValorItem);
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public int RegistrarEntrega(Entrega entrega)
        {
            using var conn = _dataBase.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("insertEntrega", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("vIdEndereco", entrega.IdEndereco);
            cmd.Parameters.AddWithValue("vDataEntrega", entrega.DataEntrega);
            cmd.Parameters.AddWithValue("vValorFrete", entrega.ValorFrete);
            cmd.Parameters.AddWithValue("vDataPrevista", entrega.DataPrevista);
            cmd.Parameters.AddWithValue("vStatus", entrega.Status);
            cmd.ExecuteNonQuery();
            using var cmdId = new MySqlCommand("SELECT LAST_INSERT_ID();", conn);
            return Convert.ToInt32(cmdId.ExecuteScalar());
        }

        public Venda AcharVenda(int id)
        {
            Venda venda = null;
            using var conn = _dataBase.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("obterVenda", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("vId", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                venda = new Venda
                {
                    IdVenda = reader["IdVenda"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdVenda"]),
                    NomeProd = reader["NomeProd"] == DBNull.Value ? string.Empty : reader["NomeProd"].ToString(),
                    IdUser = reader["IdUser"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdUser"]),
                    DataVenda = reader["DataVenda"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["DataVenda"]),
                    ValorTotal = reader["ValorTotal"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["ValorTotal"]),
                    NF = reader["NF"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NF"]),
                    IdEntrega = reader["IdEntrega"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdEntrega"])
                };
            }
            return venda;
        }

        public IEnumerable<Venda> ListarVendasPorUsuario(int userId)
        {
            var lista = new List<Venda>();
            using var conn = _dataBase.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("obterVendasPorUsuario", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("vUserId", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var venda = new Venda
                {
                    IdVenda = reader["IdVenda"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdVenda"]),
                    NomeProd = reader["NomeProd"] == DBNull.Value ? string.Empty : reader["NomeProd"].ToString(),
                    IdUser = reader["IdUser"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdUser"]),
                    DataVenda = reader["DataVenda"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["DataVenda"]),
                    ValorTotal = reader["ValorTotal"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["ValorTotal"]),
                    NF = reader["NF"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NF"]),
                    IdEntrega = reader["IdEntrega"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdEntrega"])
                };
                lista.Add(venda);
            }
            return lista;
        }
    }
}
