using Lustrious.Data;
using Lustrious.Models;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Data;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Collections.Generic;

namespace Lustrious.Repositorio
{
    public class ClienteRepositorio : IClienteRepositorio
    {
        private readonly DataBase _dataBase;
        public ClienteRepositorio(DataBase dataBase)
        {
            _dataBase = dataBase;
        }
        public void CadastrarCliente(Usuario cliente, IFormFile? foto)
        {


            string? relPath = null;

            if(foto != null && foto.Length >0)
            {
                var ext = Path.GetExtension(foto.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var saveDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fotosUsuario");
                Directory.CreateDirectory(saveDir);
                var absPath = Path.Combine(saveDir, fileName);
                using var fs = new FileStream(absPath, FileMode.Create);
                foto.CopyTo(fs);
                relPath = Path.Combine("fotosUsuario", fileName).Replace("\\", "/");

                // Assign the relative path to the cliente object so it's available for later use
                cliente.Foto = relPath;
            }

            using var conexao = _dataBase.GetConnection();
            
                conexao.Open();
               using var cmd = new MySqlCommand("insertUsuario", conexao);
                
                    var senhaHash = BCrypt.Net.BCrypt.HashPassword(cliente.Senha, workFactor:12);

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vNome", cliente.Nome);
                    cmd.Parameters.AddWithValue("vEmail", cliente.Email);
                    cmd.Parameters.AddWithValue("vCPF", cliente.CPF);
                    cmd.Parameters.AddWithValue("vSenha", senhaHash);
                    cmd.Parameters.AddWithValue("vRole", string.IsNullOrWhiteSpace(cliente.Role) ? "Cliente" : cliente.Role);
                    cmd.Parameters.AddWithValue("vSexo", cliente.Sexo);
                    cmd.Parameters.AddWithValue("vFoto", (object?)relPath ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                
            
        }
        public Usuario AcharCliente(int id)
        {
            Usuario cliente = new Usuario();
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("obterUsuario", conexao))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vId", id);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    conexao.Close();

                    foreach (DataRow dr in dt.Rows)
                    {
                        cliente = new Usuario()
                        {
                            IdUser = Convert.ToInt32(dr["IdUser"]),
                            Nome = (string)dr["Nome"],
                            Email = (string)dr["Email"],
                            Senha = (string)dr["Senha"],
                            Sexo = (string)dr["Sexo"],
                            CPF = (string)dr["CPF"],
                            Foto = dr.Table.Columns.Contains("Foto") && dr["Foto"] != DBNull.Value ? (string)dr["Foto"] : null
                        };
                    }
                }
            }
            return cliente;
        } 
        public IEnumerable<Usuario> ListarClientes()
        {
            List<Usuario> clientes = new List<Usuario>();
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("selectUsuario", conexao))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_role", "Cliente");
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    conexao.Close();

                    foreach (DataRow dr in dt.Rows)
                    {
                        clientes.Add(new Usuario
                        {
                                IdUser = Convert.ToInt32(dr["IdUser"]),
                                Nome = (string)dr["Nome"],
                                Email = (string)dr["Email"],
                                Senha = (string)dr["Senha"],
                                Sexo = (string)dr["Sexo"],
                                CPF = (string)dr["CPF"],
                        });
                    }
                }
            }
            return clientes;
        }
        public void AlterarCliente(Usuario cliente, IFormFile? foto)
        {
            string? relPath = null;

            if (foto != null && foto.Length >0)
            {
                var ext = Path.GetExtension(foto.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var saveDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fotosUsuario");
                Directory.CreateDirectory(saveDir);
                var absPath = Path.Combine(saveDir, fileName);
                using var fs = new FileStream(absPath, FileMode.Create);
                foto.CopyTo(fs);
                relPath = Path.Combine("fotosUsuario", fileName).Replace("\\", "/");
                cliente.Foto = relPath;
            }

            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                // Call stored procedure that updates user fields (expects vIdUser, vNome, vEmail, vSenha, vCPF, vSexo, vFoto)
                using (var cmd = new MySqlCommand("updateUsuario", conexao))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vIdUser", cliente.IdUser);
                    cmd.Parameters.AddWithValue("vNome", cliente.Nome);
                    cmd.Parameters.AddWithValue("vEmail", cliente.Email);
                    cmd.Parameters.AddWithValue("vSenha", cliente.Senha);
                    cmd.Parameters.AddWithValue("vCPF", cliente.CPF);
                    cmd.Parameters.AddWithValue("vSexo", cliente.Sexo);
                    cmd.Parameters.AddWithValue("vFoto", (object?)cliente.Foto ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }

                conexao.Close();
            }
        }
        public void ExcluirCliente(int id)
        {
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("DeleteUsuario", conexao))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vId", id);
                    cmd.ExecuteNonQuery();
                    conexao.Close();
                }
            }
        }

        // Implement interface method without photo for controllers that don't upload files
        public void CadastrarCliente(Usuario cliente)
        {
            CadastrarCliente(cliente, null);
        }
    }
}
