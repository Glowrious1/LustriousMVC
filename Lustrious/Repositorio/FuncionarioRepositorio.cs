    using Lustrious.Data;
using Lustrious.Models;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Data;
using System.Net.Sockets;

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
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vNome", funcionario.Nome);
                    cmd.Parameters.AddWithValue("vEmail", funcionario.Email);
                    cmd.Parameters.AddWithValue("vCPF", funcionario.CPF);
                    cmd.Parameters.AddWithValue("vSenha", funcionario.Senha);
                    cmd.Parameters.AddWithValue("vRole", funcionario.Role);
                    cmd.Parameters.AddWithValue("vSexo", funcionario.Sexo);
                    cmd.Parameters.AddWithValue("vFoto", funcionario.Foto);
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
                        funcionario = new Usuario()
                        {
                            IdUser = Convert.ToInt32(dr["IdUser"]),
                            Nome = (string)dr["Nome"],
                            Email = (string)dr["Email"],
                            Senha = (string)dr["Senha"],
                            Sexo = (string)dr["Sexo"],
                            CPF = (string)dr["CPF"]
                        };
                    }
                }
            }
            return funcionario;
        }
        public IEnumerable<Usuario> ListarFuncionario()
        {
            List<Usuario> funcionario = new List<Usuario>();
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("selectUsuario", conexao))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    conexao.Close();

                    foreach (DataRow dr in dt.Rows)
                    {
                        funcionario.Add(new Usuario
                        {
                            IdUser = Convert.ToInt32(dr["IdUser"]),
                            Nome = (string)dr["Nome"],
                            Email = (string)dr["Email"],
                            Senha = (string)dr["Senha"],
                            Sexo = (string)dr["Sexo"],
                            CPF = (string)dr["CPF"],
                            Role = (string)dr["Role"]
                        }
                        );
                    }
                }
            }
            return funcionario;
        }


        public void AlterarFuncionario(Usuario funcionario)
        {
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("updateUsuario", conexao))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vNome", funcionario.Nome);
                    cmd.Parameters.AddWithValue("vEmail", funcionario.Email);
                    cmd.Parameters.AddWithValue("vCPF", funcionario.CPF);
                    cmd.Parameters.AddWithValue("vSenha", funcionario.Senha);
                    cmd.Parameters.AddWithValue("vSexo", funcionario.Sexo);
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
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vId", id);
                    cmd.ExecuteNonQuery();
                    conexao.Close();
                }
            }
        }
    }
}

