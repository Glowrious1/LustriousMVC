using MySql.Data.MySqlClient;
using Lustrious.Models;
using Lustrious.Data;
using System.Data;
using System.Collections.Generic;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lustrious.Repositorio
{
    public class ProdutoRepositorio : IProdutoRepostorio
    {
        private readonly DataBase _dataBase;
        public ProdutoRepositorio(DataBase dataBase)
        {
            _dataBase = dataBase;
        }

        public void CadastrarProduto(Produto produto, IFormFile? foto)
        {
            string? relPath = null;
            if (foto != null && foto.Length >0)
            {
                var ext = Path.GetExtension(foto.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var saveDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fotosProduto");
                Directory.CreateDirectory(saveDir);
                var absPath = Path.Combine(saveDir, fileName);
                using var fs = new FileStream(absPath, FileMode.Create);
                foto.CopyTo(fs);
                relPath = Path.Combine("fotosProduto", fileName).Replace("\\", "/");
                produto.Foto = relPath;
            }

            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();

                using (var cmd = new MySqlCommand("insertProduto", conexao))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vCodigoBarras", produto.CodigoBarras);
                    cmd.Parameters.AddWithValue("vNomeProd", produto.NomeProd);
                    cmd.Parameters.AddWithValue("vQtd", produto.Qtd);
                    cmd.Parameters.AddWithValue("vDescricao", produto.Descricao ?? string.Empty);
                    cmd.Parameters.AddWithValue("vValorUnitario", produto.ValorUnitario);
                    cmd.Parameters.AddWithValue("vRole", produto.Genero ?? string.Empty);
                    cmd.Parameters.AddWithValue("vCodCategoria", produto.CodCategoria ??0);
                    cmd.Parameters.AddWithValue("vCodTipoProduto", produto.CodTipoProduto ??0);
                    cmd.Parameters.AddWithValue("vFoto", (object?)produto.Foto ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Produto AcharProduto(long id)
        {
            Produto produto = new Produto();
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                // Query directly to include category and tipo names
                using (var cmd = new MySqlCommand(@"SELECT p.CodigoBarras, p.NomeProd, p.qtd, p.Descricao, p.ValorUnitario, p.foto, p.Genero, p.codCategoria, p.codTipoProduto, c.Categoria AS NomeCategoria, t.TipoProduto AS NomeTipoProduto
FROM Produto p
LEFT JOIN Categoria c ON p.codCategoria = c.codCategoria
LEFT JOIN tipoProduto t ON p.codTipoProduto = t.codTipoProduto
WHERE p.CodigoBarras = @codigo", conexao))
                {
                    cmd.Parameters.AddWithValue("@codigo", id);

                    using var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        produto = new Produto()
                        {
                            CodigoBarras = reader["CodigoBarras"] == DBNull.Value ?0L : Convert.ToInt64(reader["CodigoBarras"]),
                            NomeProd = reader["NomeProd"] == DBNull.Value ? string.Empty : reader["NomeProd"].ToString()!,
                            Qtd = reader["qtd"] == DBNull.Value ?0 : Convert.ToInt32(reader["qtd"]),
                            Descricao = reader["Descricao"] == DBNull.Value ? string.Empty : reader["Descricao"].ToString()!,
                            ValorUnitario = reader["ValorUnitario"] == DBNull.Value ?0m : Convert.ToDecimal(reader["ValorUnitario"]),
                            Foto = reader["foto"] == DBNull.Value ? null : reader["foto"].ToString(),
                            Genero = reader["Genero"] == DBNull.Value ? string.Empty : reader["Genero"].ToString()!,
                            CodCategoria = reader["codCategoria"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["codCategoria"]),
                            CodTipoProduto = reader["codTipoProduto"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["codTipoProduto"]),
                            NomeCategoria = reader["NomeCategoria"] == DBNull.Value ? null : reader["NomeCategoria"].ToString(),
                            NomeTipoProduto = reader["NomeTipoProduto"] == DBNull.Value ? null : reader["NomeTipoProduto"].ToString()
                        };
                    }
                }
            }
            return produto;
        }

        public void AlterarProduto(Produto produto, IFormFile? foto)
        {
            string? relPath = null;
            if (foto != null && foto.Length >0)
            {
                var ext = Path.GetExtension(foto.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var saveDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fotosProduto");
                Directory.CreateDirectory(saveDir);
                var absPath = Path.Combine(saveDir, fileName);
                using var fs = new FileStream(absPath, FileMode.Create);
                foto.CopyTo(fs);
                relPath = Path.Combine("fotosProduto", fileName).Replace("\\", "/");
                produto.Foto = relPath;
            }

            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("updateProduto", conexao))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vCodigo", produto.CodigoBarras);
                    cmd.Parameters.AddWithValue("vNome", produto.NomeProd);
                    cmd.Parameters.AddWithValue("vQtd", produto.Qtd);
                    cmd.Parameters.AddWithValue("vDesc", produto.Descricao ?? string.Empty);
                    cmd.Parameters.AddWithValue("vValor", produto.ValorUnitario);
                    cmd.Parameters.AddWithValue("vRole", produto.Genero ?? string.Empty);
                    cmd.Parameters.AddWithValue("vCodCategoria", produto.CodCategoria ??0);
                    cmd.Parameters.AddWithValue("vCodTipoProduto", produto.CodTipoProduto ??0);
                    cmd.Parameters.AddWithValue("vFoto", (object?)produto.Foto ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                    conexao.Close();
                }
            }
        }

        public void ExcluirProduto(long id)
        {
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("deleteProduto", conexao))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vCodigo", id);
                    cmd.ExecuteNonQuery();
                    conexao.Close();
                }
            }
        }

        public (IEnumerable<Produto> Items, int TotalCount) ListarProdutos(string? q = null, int codCategoria =0, int codTipoProduto =0, int page =1, int pageSize =10)
        {
            var produtos = new List<Produto>();
            using var conexao = _dataBase.GetConnection();
            conexao.Open();

            // Build WHERE clauses
            var where = new List<string>();
            if (!string.IsNullOrWhiteSpace(q))
            {
                where.Add("(p.NomeProd LIKE @q OR p.Descricao LIKE @q)");
            }
            if (codCategoria >0)
            {
                where.Add("p.codCategoria = @codCategoria");
            }
            if (codTipoProduto >0)
            {
                where.Add("p.codTipoProduto = @codTipoProduto");
            }

            var whereClause = where.Count >0 ? "WHERE " + string.Join(" AND ", where) : string.Empty;

            // Total count
            using (var cmdCount = new MySqlCommand($"SELECT COUNT(*) FROM Produto p {whereClause}", conexao))
            {
                if (!string.IsNullOrWhiteSpace(q)) cmdCount.Parameters.AddWithValue("@q", "%" + q.Trim() + "%");
                if (codCategoria >0) cmdCount.Parameters.AddWithValue("@codCategoria", codCategoria);
                if (codTipoProduto >0) cmdCount.Parameters.AddWithValue("@codTipoProduto", codTipoProduto);

                var totalObj = cmdCount.ExecuteScalar();
                var total = totalObj == DBNull.Value ?0 : Convert.ToInt32(totalObj);

                // Calculate paging
                if (page <1) page =1;
                if (pageSize <1) pageSize =10;
                var offset = (page -1) * pageSize;

                // Query page of items with joins
                using var cmd = new MySqlCommand($@"SELECT p.CodigoBarras, p.NomeProd, p.qtd, p.Descricao, p.ValorUnitario, p.foto, p.Genero,
c.Categoria AS NomeCategoria, t.TipoProduto AS NomeTipoProduto
FROM Produto p
LEFT JOIN Categoria c ON p.codCategoria = c.codCategoria
LEFT JOIN tipoProduto t ON p.codTipoProduto = t.codTipoProduto
{whereClause}
ORDER BY p.NomeProd
LIMIT @limit OFFSET @offset", conexao);

                if (!string.IsNullOrWhiteSpace(q)) cmd.Parameters.AddWithValue("@q", "%" + q.Trim() + "%");
                if (codCategoria >0) cmd.Parameters.AddWithValue("@codCategoria", codCategoria);
                if (codTipoProduto >0) cmd.Parameters.AddWithValue("@codTipoProduto", codTipoProduto);
                cmd.Parameters.AddWithValue("@limit", pageSize);
                cmd.Parameters.AddWithValue("@offset", offset);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    produtos.Add(new Produto
                    {
                        CodigoBarras = reader["CodigoBarras"] == DBNull.Value ?0L : Convert.ToInt64(reader["CodigoBarras"]),
                        NomeProd = reader["NomeProd"] == DBNull.Value ? string.Empty : reader["NomeProd"].ToString()!,
                        Qtd = reader["qtd"] == DBNull.Value ?0 : Convert.ToInt32(reader["qtd"]),
                        Descricao = reader["Descricao"] == DBNull.Value ? string.Empty : reader["Descricao"].ToString()!,
                        ValorUnitario = reader["ValorUnitario"] == DBNull.Value ?0m : Convert.ToDecimal(reader["ValorUnitario"]),
                        Foto = reader["foto"] == DBNull.Value ? null : reader["foto"].ToString(),
                        Genero = reader["Genero"] == DBNull.Value ? string.Empty : reader["Genero"].ToString()!,
                        NomeCategoria = reader["NomeCategoria"] == DBNull.Value ? null : reader["NomeCategoria"].ToString(),
                        NomeTipoProduto = reader["NomeTipoProduto"] == DBNull.Value ? null : reader["NomeTipoProduto"].ToString()
                    });
                }

                conexao.Close();
                return (produtos, total);
            }
        }

        // Novos métodos para popular selects
        public IEnumerable<SelectListItem> GetCategorias(int? selectedId = null)
        {
            var list = new List<SelectListItem>();
            using var conexao = _dataBase.GetConnection();
            conexao.Open();
            using var cmd = new MySqlCommand("SELECT codCategoria, Categoria FROM Categoria ORDER BY Categoria", conexao);
            using var reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                var id = reader["codCategoria"] == DBNull.Value ?0 : Convert.ToInt32(reader["codCategoria"]);
                var nome = reader["Categoria"] == DBNull.Value ? string.Empty : reader["Categoria"].ToString();
                list.Add(new SelectListItem(nome, id.ToString(), selectedId.HasValue && selectedId.Value == id));
            }
            return list;
        }

        public IEnumerable<SelectListItem> GetTipos(int? selectedId = null, int? codCategoria = null)
        {
            var list = new List<SelectListItem>();
            using var conexao = _dataBase.GetConnection();
            conexao.Open();
            using var cmd = new MySqlCommand("SELECT codTipoProduto, TipoProduto, codCategoria FROM tipoProduto" + (codCategoria.HasValue ? " WHERE codCategoria = @codCategoria" : string.Empty) + " ORDER BY TipoProduto", conexao);
            if (codCategoria.HasValue) cmd.Parameters.AddWithValue("@codCategoria", codCategoria.Value);
            using var reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                var id = reader["codTipoProduto"] == DBNull.Value ?0 : Convert.ToInt32(reader["codTipoProduto"]);
                var nome = reader["TipoProduto"] == DBNull.Value ? string.Empty : reader["TipoProduto"].ToString();
                list.Add(new SelectListItem(nome, id.ToString(), selectedId.HasValue && selectedId.Value == id));
            }
            return list;
        }

        // Conveniência: retornar apenas a lista de produtos para a vitrine
        public IEnumerable<Produto> ListarProdutosPublico(string? q = null, int codCategoria =0, int codTipoProduto =0, int page =1, int pageSize =12)
        {
            var result = ListarProdutos(q, codCategoria, codTipoProduto, page, pageSize);
            return result.Items;
        }
    }
}
