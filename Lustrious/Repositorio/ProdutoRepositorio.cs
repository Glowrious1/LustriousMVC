﻿using MySql.Data.MySqlClient;
using Lustrious.Models;
using Lustrious.Data;
using System.Data;

namespace Lustrious.Repositorio
{
    //Os : indicam que o RepositorioProduto está heradndo as funcionalidades da interface IProdutoRepositorio.
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
                                qtd = (int)dr["Email"],
                                Descricao = (string)dr["Senha"],
                                ValorUnitario = (decimal)dr["Senha"],
                            };
                        }
                    }
                }
                return Produto;
            }
            public IEnumerable<Produto> ListarProdutos()
            {
                List<Produto> Produtos = new List<Produto>();
                using (var conexao = _dataBase.GetConnection())
                {
                    conexao.Open();
                    using (var cmd = new MySqlCommand("selectProduto", conexao))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                        DataTable dt = new DataTable();

                        da.Fill(dt);

                        conexao.Close();

                        foreach (DataRow dr in dt.Rows)
                        {
                            Produtos.Add(new Produto
                            {
                                CodigoBarras = Convert.ToInt32(dr["IdProduto"]),
                                NomeProd = (string)dr["Nome"],
                                qtd = (int)dr["Email"],
                                Descricao = (string)dr["Senha"],
                                ValorUnitario = (decimal)dr["Senha"],
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
