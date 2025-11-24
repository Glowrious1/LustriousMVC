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
            public void CadastrarProduto(Produto produto)
            {
                using (var conexao = _dataBase.GetConnection())
                {
                    conexao.Open();
                    
                    using (var cmd = new MySqlCommand("insertProduto", conexao))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("vCodigoBarras", produto.CodigoBarras);
                        cmd.Parameters.AddWithValue("vNomeProd", produto.NomeProd);
                        cmd.Parameters.AddWithValue("vQtd", produto.qtd);
                        cmd.Parameters.AddWithValue("vDescricao", produto.Descricao);
                        cmd.Parameters.AddWithValue("vValorUnitario", produto.ValorUnitario);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            public Produto AcharProduto(int id)
            {
                Produto Produto = new Produto();
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
                            Produto = new Produto()
                            {
                                CodigoBarras = Convert.ToInt32(dr["IdProduto"]),
                                NomeProd = (string)dr["Nome"],
                                qtd = dr.Table.Columns.Contains("qtd") && dr["qtd"] != DBNull.Value ? Convert.ToInt32(dr["qtd"]) :0,
                                Descricao = dr.Table.Columns.Contains("descricao") && dr["descricao"] != DBNull.Value ? dr["descricao"].ToString() : string.Empty,
                                ValorUnitario = dr.Table.Columns.Contains("valor_unitario") && dr["valor_unitario"] != DBNull.Value ? Convert.ToDecimal(dr["valor_unitario"]) :0m,
                            };
                        }
                    }
                }
                return Produto;
            }
            public IEnumerable<Produto> ListarProdutos(int codTipoProduto =0)
            {
                List<Produto> Produtos = new List<Produto>();
                using (var conexao = _dataBase.GetConnection())
                {
                    conexao.Open();
                    using (var cmd = new MySqlCommand("selectProdutos", conexao))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        // passa o parâmetro opcional para a procedure, caso ela aceite
                        cmd.Parameters.AddWithValue("vCodTipoProduto", codTipoProduto);

                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                        DataTable dt = new DataTable();

                        da.Fill(dt);

                        conexao.Close();

                        foreach (DataRow dr in dt.Rows)
                        {
                            Produtos.Add(new Produto
                            {
                                CodigoBarras = Convert.ToInt32(dr["IdProduto"]),
                                NomeProd = dr.Table.Columns.Contains("Nome") ? dr["Nome"].ToString() : string.Empty,
                                qtd = dr.Table.Columns.Contains("qtd") && dr["qtd"] != DBNull.Value ? Convert.ToInt32(dr["qtd"]) :0,
                                Descricao = dr.Table.Columns.Contains("descricao") ? dr["descricao"].ToString() : string.Empty,
                                ValorUnitario = dr.Table.Columns.Contains("valor_unitario") && dr["valor_unitario"] != DBNull.Value ? Convert.ToDecimal(dr["valor_unitario"]) :0m,
                            }
                            );
                        }
                    }
                }
                return Produtos;
            }
            public void AlterarProduto(Produto produto)
            {
                using (var conexao = _dataBase.GetConnection())
                {
                    conexao.Open();
                    using (var cmd = new MySqlCommand("updateProduto", conexao))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("vCodigoBarras", produto.CodigoBarras);
                        cmd.Parameters.AddWithValue("vNomeProd", produto.NomeProd);
                        cmd.Parameters.AddWithValue("vQtd", produto.qtd);
                        cmd.Parameters.AddWithValue("vDescricao", produto.Descricao);
                        cmd.Parameters.AddWithValue("vValorUnitario", produto.ValorUnitario);
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
        }
    }
