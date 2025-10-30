using Lustrious.Data;
using Lustrious.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace Lustrious.Repositorio
{
    public class FuncionarioRepositorio : IFuncionarioRepositorio
    {
        private readonly DataBase _dataBase;
        public FuncionarioRepositorio(DataBase dataBase)
        {
            _dataBase = dataBase;
        }
        public void CadastrarFuncionario(Funcionario funcionario)
        {
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("insertFuncionario", conexao))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vNome", funcionario.Nome);
                    cmd.Parameters.AddWithValue("vEmail", funcionario.Email);
                    cmd.Parameters.AddWithValue("vSenha", funcionario.Senha);
                    cmd.ExecuteNonQuery();
                }
            }

        }
        public Funcionario AcharFuncionario(int id)
        {
            Funcionario funcionario = new Funcionario();
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("obterFuncionario", conexao))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vId", id);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    conexao.Close();

                    foreach (DataRow dr in dt.Rows)
                    {
                        funcionario = new Funcionario()
                        {
                            IdFun = Convert.ToInt32(dr["IdFuncionario"]),
                            Nome = (string)dr["Nome"],
                            Email = (string)dr["Email"],
                            Senha = (string)dr["Senha"]
                        };
                    }
                }
            }
            return funcionario;
        }
        public IEnumerable<Funcionario> ListarFuncionarios()
        {
            List<Funcionario> funcionarios = new List<Funcionario>();
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("selectFuncionario", conexao))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    conexao.Close();

                    foreach (DataRow dr in dt.Rows)
                    {
                        funcionarios.Add(new Funcionario
                        {
                            IdFun = Convert.ToInt32(dr["IdFuncionario"]),
                            Nome = (string)dr["Nome"],
                            Email = (string)dr["Email"],
                            Senha = (string)dr["Senha"]
                        }
                        );
                    }
                }
            }
            return funcionarios;
        }
        public void AlterarFuncionario(Funcionario funcionario)
        {
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("updateFuncionario", conexao))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vNome", funcionario.Nome);
                    cmd.Parameters.AddWithValue("vEmail", funcionario.Email);
                    cmd.Parameters.AddWithValue("vSenha", funcionario.Senha);
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
                using (var cmd = new MySqlCommand("DeleteFuncionario", conexao))
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
}
