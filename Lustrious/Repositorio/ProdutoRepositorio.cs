using MySql.Data.MySqlClient;
using Lustrious.Models;
using Lustrious.Data;
using System.Data;
using System.Collections.Generic;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Lustrious.Repositorio
{
    //Os : indicam que o RepositorioProduto está heradndo as funcionalidades da interface IProdutoRepositorio.// 
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
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("vCodigoBarras", produto.CodigoBarras);
                        cmd.Parameters.AddWithValue("vNomeProd", produto.NomeProd);
                        cmd.Parameters.AddWithValue("vQtd", produto.Qtd);
                        cmd.Parameters.AddWithValue("vDescricao", produto.Descricao);
                        cmd.Parameters.AddWithValue("vValorUnitario", produto.ValorUnitario);
                        cmd.Parameters.AddWithValue("vFoto", (object?)relPath ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            public Produto AcharProduto(int id)
            {
                Produto produto = new Produto();
                using (var conexao = _dataBase.GetConnection())
                {
                    conexao.Open();
                    using (var cmd = new MySqlCommand("obterProduto", conexao))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("vId", id);

                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                        DataTable dt = new DataTable();

                        da.Fill(dt);

                        conexao.Close();

                        foreach (DataRow dr in dt.Rows)
                        {
                            produto = new Produto()
                            {
                                CodigoBarras = dr.Table.Columns.Contains("IdProduto") && dr["IdProduto"] != DBNull.Value ? Convert.ToInt64(dr["IdProduto"]) :0L,
                                NomeProd = dr.Table.Columns.Contains("Nome") ? dr["Nome"].ToString()! : string.Empty,
                                Qtd = dr.Table.Columns.Contains("qtd") && dr["qtd"] != DBNull.Value ? Convert.ToInt32(dr["qtd"]) :0,
                                Descricao = dr.Table.Columns.Contains("descricao") && dr["descricao"] != DBNull.Value ? dr["descricao"].ToString()! : string.Empty,
                                ValorUnitario = dr.Table.Columns.Contains("valor_unitario") && dr["valor_unitario"] != DBNull.Value ? Convert.ToDecimal(dr["valor_unitario"]) :0m,
                                Foto = dr.Table.Columns.Contains("foto") && dr["foto"] != DBNull.Value ? dr["foto"].ToString() : null
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
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("vCodigoBarras", produto.CodigoBarras);
                        cmd.Parameters.AddWithValue("vNomeProd", produto.NomeProd);
                        cmd.Parameters.AddWithValue("vQtd", produto.Qtd);
                        cmd.Parameters.AddWithValue("vDescricao", produto.Descricao);
                        cmd.Parameters.AddWithValue("vValorUnitario", produto.ValorUnitario);
                        cmd.Parameters.AddWithValue("vFoto", (object?)produto.Foto ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                        conexao.Close();
                    }
                }
            }
            public void ExcluirProduto(int id)
            {
                using (var conexao = _dataBase.GetConnection())
                {
                    conexao.Open();
                    using (var cmd = new MySqlCommand("DeleteProduto", conexao))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("vId", id);
                        cmd.ExecuteNonQuery();
                        conexao.Close();
                    }
                }
            }

        public IEnumerable<Produto> ListarProdutos(int codTipoProduto =0)
        {

            List<Produto> produtos = new List<Produto>();
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("selectProdutos", conexao))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    conexao.Close();

                    foreach (DataRow dr in dt.Rows)
                    {
                        produtos.Add(new Produto
                        {
                            CodigoBarras = dr.Table.Columns.Contains("IdProduto") && dr["IdProduto"] != DBNull.Value ? Convert.ToInt64(dr["IdProduto"]) :0L,
                            NomeProd = dr.Table.Columns.Contains("Nome") ? dr["Nome"].ToString()! : string.Empty,
                            Qtd = dr.Table.Columns.Contains("qtd") && dr["qtd"] != DBNull.Value ? Convert.ToInt32(dr["qtd"]) :0,
                            Descricao = dr.Table.Columns.Contains("descricao") ? dr["descricao"].ToString()! : string.Empty,
                            ValorUnitario = dr.Table.Columns.Contains("valor_unitario") && dr["valor_unitario"] != DBNull.Value ? Convert.ToDecimal(dr["valor_unitario"]) :0m,
                            Foto = dr.Table.Columns.Contains("foto") && dr["foto"] != DBNull.Value ? dr["foto"].ToString() : null
                        }
                        );
                    }
                }
            }
            return produtos;
        }
    }
}
