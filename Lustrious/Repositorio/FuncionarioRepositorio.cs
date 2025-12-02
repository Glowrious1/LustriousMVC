using System.Collections.Generic;
using System.Data;
using Lustrious.Data;
using Lustrious.Models;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;

namespace Lustrious.Repositorio
{
    public class FuncionarioRepositorio : IFuncionarioRepositorio
    {
        private readonly DataBase _dataBase;

        public FuncionarioRepositorio(DataBase dataBase)
        {
            _dataBase = dataBase;
        }
        public void CadastrarFuncionario(Usuario funcionario)
        {

           
             using var conexao = _dataBase.GetConnection();
            
                conexao.Open();
                using var cmd = new MySqlCommand("insertUsuario", conexao);
                var senhaHash = BCrypt.Net.BCrypt.HashPassword(funcionario.Senha, workFactor: 12);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vNome", funcionario.Nome);
                    cmd.Parameters.AddWithValue("vEmail", funcionario.Email);
                    cmd.Parameters.AddWithValue("vCPF", funcionario.CPF);
                    cmd.Parameters.AddWithValue("vSenha", senhaHash);
                    cmd.Parameters.AddWithValue("vRole", string.IsNullOrWhiteSpace(funcionario.Role)? "Funcionario" : funcionario.Role);
                    cmd.Parameters.AddWithValue("vSexo", funcionario.Sexo);
                    cmd.Parameters.AddWithValue("vFoto", (object?)funcionario.Foto ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                
            
        }
        public Usuario AcharFuncionario(int id)
        {
            Usuario funcionario = new Usuario();
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("SELECT IdUser, Nome, Email, Senha, Sexo, CPF, Role, Foto, Ativo FROM Usuario WHERE IdUser = @id", conexao))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        funcionario = new Usuario()
                        {
                            IdUser = reader["IdUser"] == DBNull.Value ?0 : Convert.ToInt32(reader["IdUser"]),
                            Nome = reader["Nome"] == DBNull.Value ? string.Empty : reader["Nome"].ToString(),
                            Email = reader["Email"] == DBNull.Value ? string.Empty : reader["Email"].ToString(),
                            Senha = reader["Senha"] == DBNull.Value ? string.Empty : reader["Senha"].ToString(),
                            Sexo = reader["Sexo"] == DBNull.Value ? string.Empty : reader["Sexo"].ToString(),
                            CPF = reader["CPF"] == DBNull.Value ? string.Empty : reader["CPF"].ToString(),
                            Role = reader["Role"] == DBNull.Value ? string.Empty : reader["Role"].ToString(),
                            Foto = reader["Foto"] == DBNull.Value ? string.Empty : reader["Foto"].ToString(),
                            Ativo = reader["Ativo"] == DBNull.Value ? "1" : reader["Ativo"].ToString()
                        };
                    }
                }
            }
            return funcionario;
        }
        public IEnumerable<Usuario> ListarFuncionario()
        {
            var funcionarios = new List<Usuario>();
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("SELECT IdUser, Nome, Email, Senha, Sexo, CPF, Role, Foto, Ativo FROM Usuario WHERE not Role = 'Cliente' ORDER BY Nome", conexao))
                {
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        funcionarios.Add(new Usuario
                        {
                            IdUser = reader["IdUser"] == DBNull.Value ?0 : Convert.ToInt32(reader["IdUser"]),
                            Nome = reader["Nome"] == DBNull.Value ? string.Empty : reader["Nome"].ToString(),
                            Email = reader["Email"] == DBNull.Value ? string.Empty : reader["Email"].ToString(),
                            Senha = reader["Senha"] == DBNull.Value ? string.Empty : reader["Senha"].ToString(),
                            Sexo = reader["Sexo"] == DBNull.Value ? string.Empty : reader["Sexo"].ToString(),
                            CPF = reader["CPF"] == DBNull.Value ? string.Empty : reader["CPF"].ToString(),
                            Role = reader["Role"] == DBNull.Value ? string.Empty : reader["Role"].ToString(),
                            Foto = reader["Foto"] == DBNull.Value ? string.Empty : reader["Foto"].ToString(),
                            Ativo = reader["Ativo"] == DBNull.Value ? "1" : reader["Ativo"].ToString()
                        });
                    }
                }
                conexao.Close();
            }
            return funcionarios;
        }


        public void AlterarFuncionario(Usuario funcionario)
        {
           
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("updateUsuario", conexao))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vIdUser", funcionario.IdUser);
                    cmd.Parameters.AddWithValue("vNome", funcionario.Nome);
                    cmd.Parameters.AddWithValue("vEmail", funcionario.Email);
                    cmd.Parameters.AddWithValue("vCPF", funcionario.CPF);
                    // Hash password if provided
                    var senhaHash = funcionario.Senha;
                    if (!string.IsNullOrWhiteSpace(funcionario.Senha))
                    {
                        senhaHash = BCrypt.Net.BCrypt.HashPassword(funcionario.Senha, workFactor:12);
                    }
                    cmd.Parameters.AddWithValue("vSenha", senhaHash);
                    cmd.Parameters.AddWithValue("vSexo", funcionario.Sexo);
                    cmd.Parameters.AddWithValue("vFoto", (object?)funcionario.Foto ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                    conexao.Close();
                }
            }
        }
        public void ExcluirFuncionario(int id)
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
    }
}

