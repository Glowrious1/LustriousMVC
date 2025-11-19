using Lustrious.Data;
using Lustrious.Models;
using MySql.Data.MySqlClient;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using System;
using Lustrious.Services;

namespace Lustrious.Repositorio
{
    //Completar depois
    public class CarrinhoRepositorio : ICarrinhoRepositorio
    {
        private readonly DataBase _dataBase;
        private readonly IVendaRepositorio _vendaRepositorio;
        private readonly IFreteService _freteService;
        public CarrinhoRepositorio(DataBase dataBase, IVendaRepositorio vendaRepositorio, IFreteService freteService)
        {
            _dataBase = dataBase;
            _vendaRepositorio = vendaRepositorio;
            _freteService = freteService;
        }
        public void AdicionarItem(int produtoId, int userId)
        {
            using(var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using(var cmd = new MySqlCommand("insertCarrinho", conexao))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vUserId", userId);
                    cmd.Parameters.AddWithValue("vProdutoId", produtoId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public Carrinho AcharCarrinho(int userId)
        {
            var carrinho = new Carrinho();

            using(var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using(var cmd = new MySqlCommand(@"SELECT c.id AS IdCarrinho, c.id_prod AS IdProd, c.qtd AS Qtd, p.valor_unitario AS ValorUnitario, p.codigo_barras AS CodigoBarras
FROM carrinho c
INNER JOIN produtos p ON p.id = c.id_prod
WHERE c.id_user = @userId", conexao))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using var reader = cmd.ExecuteReader();

                    int totalQtd =0;
                    decimal total =0m;

                    while(reader.Read())
                    {
                        var idCarrinho = reader["IdCarrinho"] == DBNull.Value ?0 : Convert.ToInt32(reader["IdCarrinho"]);
                        var idProd = reader["IdProd"] == DBNull.Value ?0 : Convert.ToInt32(reader["IdProd"]);
                        var qtd = reader["Qtd"] == DBNull.Value ?0 : Convert.ToInt32(reader["Qtd"]);
                        var valorUnit = reader["ValorUnitario"] == DBNull.Value ?0m : Convert.ToDecimal(reader["ValorUnitario"]);
                        var codigo = reader["CodigoBarras"] == DBNull.Value ?0L : Convert.ToInt64(reader["CodigoBarras"]);

                        totalQtd += qtd;
                        total += valorUnit * qtd;

                        carrinho.Items.Add(new VendaProduto
                        {
                            CodigoBarras = new BigInteger(codigo),
                            ValorItem = valorUnit,
                            Qtd = qtd
                        });
                    }

                    carrinho.Qtd = totalQtd;
                    carrinho.ValorTotal = total;
                }
            }

            return carrinho;
        }
        public void FinalizarCompra(int idEnd, int userId)
        {
            var itens = new List<VendaProduto>();
            using(var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                // Ler itens do carrinho com join em produtos
                using(var cmd = new MySqlCommand(@"SELECT p.codigo_barras AS CodigoBarras, p.valor_unitario AS ValorUnitario, c.qtd AS Qtd
FROM carrinho c
INNER JOIN produtos p ON p.id = c.id_prod
WHERE c.id_user = @userId", conexao))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using var reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        var codigo = reader["CodigoBarras"] == DBNull.Value ?0L : Convert.ToInt64(reader["CodigoBarras"]);
                        var valor = reader["ValorUnitario"] == DBNull.Value ?0m : Convert.ToDecimal(reader["ValorUnitario"]);
                        var qtd = reader["Qtd"] == DBNull.Value ?0 : Convert.ToInt32(reader["Qtd"]);

                        itens.Add(new VendaProduto
                        {
                            CodigoBarras = new BigInteger(codigo),
                            ValorItem = valor,
                            Qtd = qtd,
                        });
                    }
                }

                // Se não houver itens, não prossegue
                if (!itens.Any())
                    return;

                // Calcular total
                var total = itens.Sum(i => i.ValorItem * i.Qtd);

                // Calcular frete usando serviço de frete
                decimal frete = _freteService.CalcularFreteAsync(idEnd).GetAwaiter().GetResult();

                // Registrar entrega e obter id
                var entrega = new Entrega
                {
                    IdEndereco = idEnd,
                    DataEntrega = DateTime.Now,
                    ValorFrete = frete,
                    DataPrevista = DateTime.Now.AddDays(3),
                    Status = "Pendente"
                };

                int idEntrega = _vendaRepositorio.RegistrarEntrega(entrega);

                var venda = new Venda
                {
                    IdUser = userId,
                    DataVenda = DateTime.Now,
                    ValorTotal = total + frete, // adiciona o valor do frete ao total
                    NF =0,
                    IdEntrega = idEntrega
                };

                // Registrar venda via repositório de vendas
                _vendaRepositorio.RegistrarVenda(venda, itens);

                // Limpar carrinho do usuário
                using(var cmdClear = new MySqlCommand("DELETE FROM carrinho WHERE id_user = @userId", conexao))
                {
                    cmdClear.Parameters.AddWithValue("@userId", userId);
                    cmdClear.ExecuteNonQuery();
                }
            }
        }

        private string GetCepByEnderecoId(int enderecoId)
        {
            using var conn = _dataBase.GetConnection();
            using var cmd = new MySqlCommand("SELECT cep FROM endereco WHERE id_endereco = @id", conn);
            cmd.Parameters.AddWithValue("@id", enderecoId);
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? string.Empty : result.ToString();
        }
        public void LimparCarrinho(int userId)
        {
            using(var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using(var cmd = new MySqlCommand("DELETE FROM carrinho WHERE id_user = @userId", conexao))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void RemoverItem(int produtoId, int userId)
        {
            using(var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using(var cmd = new MySqlCommand("DELETE FROM carrinho WHERE id_user = @userId AND id_prod = @produtoId", conexao))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@produtoId", produtoId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
