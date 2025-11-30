using Lustrious.Data;
using Lustrious.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;

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
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("insertUsuario", conexao))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vNome", funcionario.Nome);
                    cmd.Parameters.AddWithValue("vEmail", funcionario.Email);
                    cmd.Parameters.AddWithValue("vCPF", funcionario.CPF);
                    cmd.Parameters.AddWithValue("vSenha", funcionario.Senha);
                    cmd.Parameters.AddWithValue("vRole", funcionario.Role);
                    cmd.Parameters.AddWithValue("vSexo", funcionario.Sexo);
                    cmd.Parameters.AddWithValue("vFoto", funcionario.Foto ?? string.Empty);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public Usuario AcharFuncionario(int id)
        {
            Usuario funcionario = new Usuario();
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("SELECT IdUser, Nome, Email, Senha, Sexo, CPF, Role, Foto FROM Usuario WHERE IdUser = @id", conexao))
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
                            Foto = reader["Foto"] == DBNull.Value ? string.Empty : reader["Foto"].ToString()
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
                using (var cmd = new MySqlCommand("SELECT IdUser, Nome, Email, Senha, Sexo, CPF, Role, Foto FROM Usuario WHERE not Role = 'Cliente' ORDER BY Nome", conexao))
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
                            Foto = reader["Foto"] == DBNull.Value ? string.Empty : reader["Foto"].ToString()
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
                    cmd.Parameters.AddWithValue("vSenha", funcionario.Senha);
                    cmd.Parameters.AddWithValue("vSexo", funcionario.Sexo);
                    cmd.Parameters.AddWithValue("vFoto", funcionario.Foto ?? string.Empty);
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
                using (var cmd = new MySqlCommand("DeleteUsuario", conexao))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vIdUser", id);
                    cmd.ExecuteNonQuery();
                    conexao.Close();
                }
            }
        }
    }
}

