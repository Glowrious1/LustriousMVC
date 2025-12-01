using Lustrious.Data;
using Lustrious.Models;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Numerics;

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
                // Garantir DataVenda
                if (venda.DataVenda == DateTime.MinValue)
                    venda.DataVenda = DateTime.Now;

                // Garantir que exista uma NotaFiscal válida. Se NF for0, criar uma nova nota fiscal
                if (venda.NF ==0)
                {
                    using var cmdNota = new MySqlCommand("INSERT INTO NotaFiscal (TotalNota, DataEmissao) VALUES (@total, @data)", conn, transaction);
                    cmdNota.Parameters.Add("@total", MySqlDbType.Decimal).Value = venda.ValorTotal;
                    cmdNota.Parameters.Add("@data", MySqlDbType.Date).Value = DateTime.Now;
                    cmdNota.ExecuteNonQuery();

                    using var cmdGetNota = new MySqlCommand("SELECT LAST_INSERT_ID();", conn, transaction);
                    var inserted = cmdGetNota.ExecuteScalar();
                    if (inserted == null || inserted == DBNull.Value)
                    {
                        transaction.Rollback();
                        throw new InvalidOperationException("Falha ao inserir NotaFiscal: LAST_INSERT_ID() retornou nulo.");
                    }

                    venda.NF = Convert.ToInt32(inserted);
                    if (venda.NF <=0)
                    {
                        transaction.Rollback();
                        throw new InvalidOperationException("Falha ao inserir NotaFiscal: id retornado inválido (<=0).");
                    }

                    // Verificar que a nota fiscal inserida existe (ajuda a diagnosticar FK)
                    using var cmdCheckNota = new MySqlCommand("SELECT COUNT(1) FROM NotaFiscal WHERE NF = @nf", conn, transaction);
                    cmdCheckNota.Parameters.Add("@nf", MySqlDbType.Int32).Value = venda.NF;
                    var notaExists = Convert.ToInt32(cmdCheckNota.ExecuteScalar()) >0;
                    if (!notaExists)
                    {
                        transaction.Rollback();
                        throw new InvalidOperationException($"NotaFiscal inserida (NF={venda.NF}) não encontrada no banco. Isso causará falha de FK.");
                    }
                }

                // Verificação final antes de inserir Venda
                if (venda.NF <=0)
                {
                    transaction.Rollback();
                    throw new InvalidOperationException("NF inválida ao tentar inserir Venda. Certifique-se de que NotaFiscal foi criada corretamente.");
                }

                // Se foi informado IdEntrega, validar que a entrega existe (evita FK inválida)
                if (venda.IdEntrega >0)
                {
                    using var cmdCheckEntrega = new MySqlCommand("SELECT COUNT(1) FROM Entrega WHERE IdEntrega = @id", conn, transaction);
                    cmdCheckEntrega.Parameters.Add("@id", MySqlDbType.Int32).Value = venda.IdEntrega;
                    var exists = Convert.ToInt32(cmdCheckEntrega.ExecuteScalar()) >0;
                    if (!exists)
                    {
                        transaction.Rollback();
                        throw new InvalidOperationException("Entrega informada não existe. Verifique o endereço/entrega do cliente.");
                    }
                }

                // Inserir venda (assumindo stored procedure insertVenda que retorna id via SELECT LAST_INSERT_ID())
                using (var cmd = new MySqlCommand("insertVenda", conn, transaction))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("vIdUser", MySqlDbType.Int32).Value = venda.IdUser;
                    cmd.Parameters.Add("vDataVenda", MySqlDbType.DateTime).Value = venda.DataVenda;
                    cmd.Parameters.Add("vValorTotal", MySqlDbType.Decimal).Value = venda.ValorTotal;
                    cmd.Parameters.Add("vNF", MySqlDbType.Int32).Value = venda.NF;

                    // passar NULL para vIdEntrega quando não informado (0)
                    var pIdEntrega = cmd.Parameters.Add("vIdEntrega", MySqlDbType.Int32);
                    if (venda.IdEntrega >0)
                        pIdEntrega.Value = venda.IdEntrega;
                    else
                        pIdEntrega.Value = DBNull.Value;

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
                    cmd.Parameters.Add("vIdVenda", MySqlDbType.Int32).Value = idVenda;

                    // Enviar CodigoBarras como bigint: converter BigInteger para long
                    long codigoLong;
                    try
                    {
                        codigoLong = (long)item.CodigoBarras;
                    }
                    catch (Exception)
                    {
                        // Se não couber em long, falhar com mensagem clara
                        transaction.Rollback();
                        throw new InvalidOperationException($"CodigoBarras inválido para inserção (fora do intervalo Int64): {item.CodigoBarras}");
                    }

                    cmd.Parameters.Add("vCodigoBarras", MySqlDbType.Int64).Value = codigoLong;
                    cmd.Parameters.Add("vQtd", MySqlDbType.Int32).Value = item.Qtd;
                    cmd.Parameters.Add("vValorItem", MySqlDbType.Decimal).Value = item.ValorItem;
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                try { transaction.Rollback(); } catch { }
                // Re-lançar com contexto para facilitar debug do FK
                throw new InvalidOperationException($"Erro ao registrar venda. NF={venda?.NF}, IdEntrega={venda?.IdEntrega}. Mensagem original: {ex.Message}", ex);
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
                    IdVenda = reader["IdVenda"] == DBNull.Value ?0 : Convert.ToInt32(reader["IdVenda"]),
                    NomeProd = reader["NomeProd"] == DBNull.Value ? string.Empty : reader["NomeProd"].ToString(),
                    IdUser = reader["IdUser"] == DBNull.Value ?0 : Convert.ToInt32(reader["IdUser"]),
                    DataVenda = reader["DataVenda"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["DataVenda"]),
                    ValorTotal = reader["ValorTotal"] == DBNull.Value ?0m : Convert.ToDecimal(reader["ValorTotal"]),
                    NF = reader["NF"] == DBNull.Value ?0 : Convert.ToInt32(reader["NF"]),
                    IdEntrega = reader["IdEntrega"] == DBNull.Value ?0 : Convert.ToInt32(reader["IdEntrega"])
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
                    IdVenda = reader["IdVenda"] == DBNull.Value ?0 : Convert.ToInt32(reader["IdVenda"]),
                    NomeProd = reader["NomeProd"] == DBNull.Value ? string.Empty : reader["NomeProd"].ToString(),
                    IdUser = reader["IdUser"] == DBNull.Value ?0 : Convert.ToInt32(reader["IdUser"]),
                    DataVenda = reader["DataVenda"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["DataVenda"]),
                    ValorTotal = reader["ValorTotal"] == DBNull.Value ?0m : Convert.ToDecimal(reader["ValorTotal"]),
                    NF = reader["NF"] == DBNull.Value ?0 : Convert.ToInt32(reader["NF"]),
                    IdEntrega = reader["IdEntrega"] == DBNull.Value ?0 : Convert.ToInt32(reader["IdEntrega"])
                };
                lista.Add(venda);
            }
            return lista;
        }

        public IEnumerable<Venda> ListarTodasVendas()
        {
            var lista = new List<Venda>();
            using var conn = _dataBase.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT IdVenda, NomeProd, ValorTotal, DataVenda, IdUser, NF, IdEntrega FROM Venda ORDER BY DataVenda DESC", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var venda = new Venda
                {
                    IdVenda = reader["IdVenda"] == DBNull.Value ?0 : Convert.ToInt32(reader["IdVenda"]),
                    NomeProd = reader["NomeProd"] == DBNull.Value ? string.Empty : reader["NomeProd"].ToString(),
                    IdUser = reader["IdUser"] == DBNull.Value ?0 : Convert.ToInt32(reader["IdUser"]),
                    DataVenda = reader["DataVenda"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["DataVenda"]),
                    ValorTotal = reader["ValorTotal"] == DBNull.Value ?0m : Convert.ToDecimal(reader["ValorTotal"]),
                    NF = reader["NF"] == DBNull.Value ?0 : Convert.ToInt32(reader["NF"]),
                    IdEntrega = reader["IdEntrega"] == DBNull.Value ?0 : Convert.ToInt32(reader["IdEntrega"])
                };
                lista.Add(venda);
            }
            return lista;
        }

        public void NotificarClienteVenda(int userId, string mensagem)
        {
            using var conn = _dataBase.GetConnection();
            conn.Open();
            // Create table with proper syntax for default0
            using var cmd = new MySqlCommand(@"CREATE TABLE IF NOT EXISTS Notificacao (Id int primary key auto_increment, IdUser int, Mensagem varchar(500), DataEnvio datetime, Lida tinyint(1) default0)", conn);
            cmd.ExecuteNonQuery();

            using var cmdIns = new MySqlCommand("INSERT INTO Notificacao (IdUser, Mensagem, DataEnvio, Lida) VALUES (@idUser, @msg, @data, @lida)", conn);
            cmdIns.Parameters.AddWithValue("@idUser", userId);
            cmdIns.Parameters.AddWithValue("@msg", mensagem);
            cmdIns.Parameters.AddWithValue("@data", DateTime.Now);
            cmdIns.Parameters.AddWithValue("@lida", false);
            cmdIns.ExecuteNonQuery();
        }

        public IEnumerable<Notificacao> ListarNotificacoes(int userId)
        {
            var lista = new List<Notificacao>();
            using var conn = _dataBase.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT Id, IdUser, Mensagem, DataEnvio, Lida FROM Notificacao WHERE IdUser = @id ORDER BY DataEnvio DESC", conn);
            cmd.Parameters.AddWithValue("@id", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Notificacao
                {
                    Id = reader["Id"] == DBNull.Value ?0 : Convert.ToInt32(reader["Id"]),
                    IdUser = reader["IdUser"] == DBNull.Value ?0 : Convert.ToInt32(reader["IdUser"]),
                    Mensagem = reader["Mensagem"] == DBNull.Value ? string.Empty : reader["Mensagem"].ToString(),
                    DataEnvio = reader["DataEnvio"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["DataEnvio"]),
                    Lida = reader["Lida"] == DBNull.Value ? false : Convert.ToBoolean(reader["Lida"]) 
                });
            }
            return lista;
        }

        public int ContarNotificacoesNaoLidas(int userId)
        {
            using var conn = _dataBase.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT COUNT(1) FROM Notificacao WHERE IdUser = @id AND Lida =0", conn);
            cmd.Parameters.AddWithValue("@id", userId);
            var obj = cmd.ExecuteScalar();
            return obj == null || obj == DBNull.Value ?0 : Convert.ToInt32(obj);
        }

        public IEnumerable<Notificacao> ListarUltimasNotificacoes(int userId, int max)
        {
            var lista = new List<Notificacao>();
            using var conn = _dataBase.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT Id, IdUser, Mensagem, DataEnvio, Lida FROM Notificacao WHERE IdUser = @id ORDER BY DataEnvio DESC LIMIT @max", conn);
            cmd.Parameters.AddWithValue("@id", userId);
            cmd.Parameters.AddWithValue("@max", max);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Notificacao
                {
                    Id = reader["Id"] == DBNull.Value ?0 : Convert.ToInt32(reader["Id"]),
                    IdUser = reader["IdUser"] == DBNull.Value ?0 : Convert.ToInt32(reader["IdUser"]),
                    Mensagem = reader["Mensagem"] == DBNull.Value ? string.Empty : reader["Mensagem"].ToString(),
                    DataEnvio = reader["DataEnvio"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["DataEnvio"]),
                    Lida = reader["Lida"] == DBNull.Value ? false : Convert.ToBoolean(reader["Lida"]) 
                });
            }
            return lista;
        }

        public void MarcarUltimasNotificacoesComoLidas(int userId, int max)
        {
            using var conn = _dataBase.GetConnection();
            conn.Open();
            // Atualiza as últimas 'max' notificações (ordenadas por DataEnvio desc)
            var sql = @"UPDATE Notificacao n
                        JOIN (
                            SELECT Id FROM Notificacao WHERE IdUser = @id ORDER BY DataEnvio DESC LIMIT @max
                        ) t ON n.Id = t.Id
                        SET n.Lida =1";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", userId);
            cmd.Parameters.AddWithValue("@max", max);
            cmd.ExecuteNonQuery();
        }
    }
}
