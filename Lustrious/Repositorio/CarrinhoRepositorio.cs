using Lustrious.Data;
using Lustrious.Models;
using MySql.Data.MySqlClient;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Data;
using Lustrious.Services;

namespace Lustrious.Repositorio
{
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

        public bool AdicionarItem(int produtoId, int userId)
        {
            using var conexao = _dataBase.GetConnection();
            conexao.Open();


            // verificar estoque do produto (assume-se que Produto.CodigoBarras é chave)
            using (var cmdCheck = new MySqlCommand("SELECT qtd FROM Produto WHERE CodigoBarras = @codigo", conexao))
            {
                cmdCheck.Parameters.AddWithValue("@codigo", produtoId);
                var obj = cmdCheck.ExecuteScalar();
                if (obj == null || obj == DBNull.Value)
                    return false; // produto não encontrado
                var estoque = Convert.ToInt32(obj);
                if (estoque <= 0) return false; // sem estoque
            }

            // Chama procedure insertCarrinho(vUserId, vCodigo, vQtd)
            using (var cmd = new MySqlCommand("insertCarrinho", conexao))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("vUserId", userId);
                cmd.Parameters.AddWithValue("vCodigo", produtoId);
                cmd.Parameters.AddWithValue("vQtd", 1);
                cmd.ExecuteNonQuery();
            }

            return true;
        }

        public bool IncrementarItem(int produtoId, int userId)
        {
            using var conexao = _dataBase.GetConnection();
            conexao.Open();


            // verificar estoque disponível
            int estoque = 0;
            using (var cmdStock = new MySqlCommand("SELECT qtd FROM Produto WHERE CodigoBarras = @codigo", conexao))
            {
                cmdStock.Parameters.AddWithValue("@codigo", produtoId);
                var obj = cmdStock.ExecuteScalar();
                if (obj == null || obj == DBNull.Value) return false;
                estoque = Convert.ToInt32(obj);
            }

            // obter quantidade atual no carrinho
            int qtdAtual = 0;
            using (var cmdGet = new MySqlCommand("SELECT qtd FROM Carrinho WHERE IdUser = @userId AND IdProd = @produtoId", conexao))
            {
                cmdGet.Parameters.AddWithValue("@userId", userId);
                cmdGet.Parameters.AddWithValue("@produtoId", produtoId);
                var obj = cmdGet.ExecuteScalar();
                if (obj != null && obj != DBNull.Value) qtdAtual = Convert.ToInt32(obj);
            }

            if (qtdAtual + 1 > estoque) return false; // não há estoque suficiente

            // Use stored procedure to increment (it will insert or update quantity)
            using (var cmd = new MySqlCommand("insertCarrinho", conexao))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("vUserId", userId);
                cmd.Parameters.AddWithValue("vCodigo", produtoId);
                cmd.Parameters.AddWithValue("vQtd", 1);
                cmd.ExecuteNonQuery();
            }

            return true;
        }

        public bool DecrementarItem(int produtoId, int userId)
        {
            using var conexao = _dataBase.GetConnection();
            conexao.Open();
            using (var cmd = new MySqlCommand("UPDATE Carrinho SET Qtd = Qtd -1 WHERE IdUser = @userId AND IdProd = @produtoId", conexao))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@produtoId", produtoId);
                cmd.ExecuteNonQuery();
            }
            using (var cmdDel = new MySqlCommand("DELETE FROM Carrinho WHERE IdUser = @userId AND IdProd = @produtoId AND Qtd <=0", conexao))
            {
                cmdDel.Parameters.AddWithValue("@userId", userId);
                cmdDel.Parameters.AddWithValue("@produtoId", produtoId);
                cmdDel.ExecuteNonQuery();
            }
            return true;
        }

        public Carrinho AcharCarrinho(int userId)
        {
            var carrinho = new Carrinho();

            using var conexao = _dataBase.GetConnection();
            conexao.Open();

            using var cmd = new MySqlCommand(@"SELECT c.IdCarrinho AS IdCarrinho, c.IdProd AS IdProd, c.Qtd AS Qtd, p.ValorUnitario AS ValorUnitario, p.CodigoBarras AS CodigoBarras, p.NomeProd AS Nome, p.Foto AS Imagem  
            FROM Carrinho c 
            INNER JOIN Produto p ON p.CodigoBarras = c.IdProd
            WHERE c.IdUser = @userId", conexao);
            cmd.Parameters.AddWithValue("@userId", userId);
            using var reader = cmd.ExecuteReader();

            int totalQtd = 0;
            decimal total = 0m;

            while (reader.Read())
            {
                var idProd = reader["IdProd"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdProd"]);
                var qtd = reader["Qtd"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Qtd"]);
                var valorUnit = reader["ValorUnitario"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["ValorUnitario"]);
                var codigo = reader["CodigoBarras"] == DBNull.Value ? 0L : Convert.ToInt64(reader["CodigoBarras"]);
                var nome = reader["Nome"] == DBNull.Value ? string.Empty : reader["Nome"].ToString();
                var imagem = reader["Imagem"] == DBNull.Value ? string.Empty : reader["Imagem"].ToString();

                totalQtd += qtd;
                total += valorUnit * qtd;

                carrinho.Items.Add(new VendaProduto
                {
                    CodigoBarras = new BigInteger(codigo),
                    ValorItem = valorUnit,
                    Qtd = qtd,
                    Nome = nome,
                    Imagem = imagem,
                    ProdutoId = idProd
                });
            }

            carrinho.Qtd = totalQtd;
            carrinho.ValorTotal = total;
            return carrinho;
        }

        public void FinalizarCompra(int idEnd, int userId)
        {
            var itens = new List<VendaProduto>();
            using var conexao = _dataBase.GetConnection();
            conexao.Open();

            using var cmd = new MySqlCommand(@"SELECT p.CodigoBarras AS CodigoBarras, p.ValorUnitario AS ValorUnitario, c.Qtd AS Qtd, p.CodigoBarras AS ProdutoId, p.NomeProd AS Nome, p.Foto AS Imagem
            FROM Carrinho c
            INNER JOIN Produto p ON p.CodigoBarras = c.IdProd
            WHERE c.IdUser = @userId;
            ", conexao);
            cmd.Parameters.AddWithValue("@userId", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var codigo = reader["CodigoBarras"] == DBNull.Value ? 0L : Convert.ToInt64(reader["CodigoBarras"]);
                var valor = reader["ValorUnitario"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["ValorUnitario"]);
                var qtd = reader["Qtd"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Qtd"]);
                var produtoId = reader["ProdutoId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ProdutoId"]);
                var nome = reader["Nome"] == DBNull.Value ? string.Empty : reader["Nome"].ToString();
                var imagem = reader["Imagem"] == DBNull.Value ? string.Empty : reader["Imagem"].ToString();

                itens.Add(new VendaProduto
                {
                    CodigoBarras = new BigInteger(codigo),
                    ValorItem = valor,
                    Qtd = qtd,
                    ProdutoId = produtoId,
                    Nome = nome,
                    Imagem = imagem
                });
            }
            if (!itens.Any()) return;

            var total = itens.Sum(i => i.ValorItem * i.Qtd);
            decimal frete = _freteService.CalcularFreteAsync(idEnd).GetAwaiter().GetResult();

            var entrega = new Entrega
            {
                IdEndereco = idEnd,
                DataEntrega = DateTime.Now,
                ValorFrete = frete,
                DataPrevista = DateTime.Now.AddDays(3),
                Status = "Pedido enviado"
            };

            int idEntrega = _vendaRepositorio.RegistrarEntrega(entrega);

            var venda = new Venda
            {
                IdUser = userId,
                DataVenda = DateTime.Now,
                ValorTotal = total + frete,
                NF = 1,
                IdEntrega = idEntrega
            };

            _vendaRepositorio.RegistrarVenda(venda, itens);

            LimparCarrinho(userId);
        }

        public void LimparCarrinho(int userId)
        {
            using var conexao = _dataBase.GetConnection();
            conexao.Open();
            using var cmd = new MySqlCommand("DELETE FROM Carrinho WHERE IdUser = @userId", conexao);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.ExecuteNonQuery();
        }

        public void RemoverItem(int produtoId, int userId)
        {
            using var conexao = _dataBase.GetConnection();
            conexao.Open();
            using var cmd = new MySqlCommand("DELETE FROM Carrinho WHERE IdUser = @userId AND IdProd = @produtoId", conexao);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@produtoId", produtoId);
            cmd.ExecuteNonQuery();
        }
    }
}