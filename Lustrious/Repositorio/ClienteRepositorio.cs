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

        // Helper to safely get string values from a DataRow (returns null if DBNull or column missing)
        private static string? GetStringFromRow(DataRow dr, string column)
        {
            if (dr == null) return null;
            if (!dr.Table.Columns.Contains(column)) return null;
            var val = dr[column];
            return val == DBNull.Value ? null : val.ToString();
        }

        // Helper to safely get int values from a DataRow (returns 0 if DBNull or column missing)
        private static int GetIntFromRow(DataRow dr, string column)
        {
            if (dr == null) return 0;
            if (!dr.Table.Columns.Contains(column)) return 0;
            var val = dr[column];
            return val == DBNull.Value ? 0 : Convert.ToInt32(val);
        }

        public void CadastrarCliente(Usuario cliente, IFormFile? foto)
        {
            string? relPath = null;

            if (foto != null && foto.Length > 0)
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
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(cliente.Senha, workFactor: 12);

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
                            IdUser = GetIntFromRow(dr, "IdUser"),
                            Nome = GetStringFromRow(dr, "Nome") ?? string.Empty,
                            Email = GetStringFromRow(dr, "Email") ?? string.Empty,
                            Senha = GetStringFromRow(dr, "Senha") ?? string.Empty,
                            Sexo = GetStringFromRow(dr, "Sexo") ?? string.Empty,
                            CPF = GetStringFromRow(dr, "CPF") ?? string.Empty,
                            Foto = GetStringFromRow(dr, "Foto")
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

                using (var cmd = new MySqlCommand("SELECT IdUser, Nome, Email, Senha, Sexo, CPF, Role, Foto, Ativo FROM Usuario WHERE Role = 'Cliente' ORDER BY Nome", conexao))
                {
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        clientes.Add(new Usuario
                        {
                            IdUser = reader["IdUser"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdUser"]),
                            Nome = reader["Nome"] == DBNull.Value ? string.Empty : reader["Nome"].ToString(),
                            Email = reader["Email"] == DBNull.Value ? string.Empty : reader["Email"].ToString(),
                            Senha = reader["Senha"] == DBNull.Value ? string.Empty : reader["Senha"].ToString(),
                            Sexo = reader["Sexo"] == DBNull.Value ? string.Empty : reader["Sexo"].ToString(),
                            CPF = reader["CPF"] == DBNull.Value ? string.Empty : reader["CPF"].ToString(),
                            Role = reader["Role"] == DBNull.Value ? string.Empty : reader["Role"].ToString(),
                            Foto = reader["Foto"] == DBNull.Value ? null : reader["Foto"].ToString(),
                            Ativo = reader["Ativo"] == DBNull.Value ? "1" : reader["Ativo"].ToString()
                        });
                    }
                }

                conexao.Close();
            }

            return clientes;
        }

        public void AlterarCliente(Usuario cliente, IFormFile? foto)
        {
            string? relPath = null;

            if (foto != null && foto.Length > 0)
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

                using (var cmd = new MySqlCommand("updateUsuario", conexao))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vIdUser", cliente.IdUser);
                    cmd.Parameters.AddWithValue("vNome", cliente.Nome);
                    cmd.Parameters.AddWithValue("vEmail", cliente.Email);
                    cmd.Parameters.AddWithValue("vCPF", cliente.CPF);
                    cmd.Parameters.AddWithValue("vSenha", cliente.Senha);
                    cmd.Parameters.AddWithValue("vSexo", cliente.Sexo);
                    // If a new photo was uploaded, send it; otherwise send DBNull (or existing value handling in stored proc)
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

                using (var cmd = new MySqlCommand("UPDATE Usuario SET Ativo = '0' WHERE IdUser = @id", conexao))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                conexao.Close();
            }
        }

        public void ReativarCliente(int id)
        {
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();

                using (var cmd = new MySqlCommand("UPDATE Usuario SET Ativo = '1' WHERE IdUser = @id", conexao))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                conexao.Close();
            }
        }

        // Implement interface method without photo for controllers that don't upload files
        public void CadastrarCliente(Usuario cliente)
        {
            CadastrarCliente(cliente, null);
        }
    }
}
