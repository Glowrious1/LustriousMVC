using Lustrious.Data;
using Lustrious.Models;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Data;
using System.Net.Sockets;

namespace Lustrious.Repositorio
{
    public class ClienteRepositorio : IClienteRepositorio
    {
        private readonly DataBase _dataBase;
        public ClienteRepositorio(DataBase dataBase)
        {
            _dataBase = dataBase;
        }
        public void CadastrarCliente(Usuario cliente)
        {
            using(var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using(var cmd = new MySqlCommand("insertCliente", conexao))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vNome", cliente.Nome);
                    cmd.Parameters.AddWithValue("vEmail", cliente.Email);
                    cmd.Parameters.AddWithValue("vCPF", cliente.CPF);
                    cmd.Parameters.AddWithValue("vSenha", cliente.Senha);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public Usuario AcharCliente(int id)
        {
            Usuario cliente = new Usuario();
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("obterCliente", conexao))
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
                            IdUser = Convert.ToInt32(dr["IdCliente"]),
                            Nome = (string)dr["Nome"],
                            Email = (string)dr["Email"],
                            Senha = (string)dr["Senha"],
                            CPF = (string)dr["CPF"]
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
                using (var cmd = new MySqlCommand("selectCliente", conexao))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    conexao.Close();

                    foreach (DataRow dr in dt.Rows)
                    {
                        clientes.Add(new Usuario
                            {
                                IdUser = Convert.ToInt32(dr["IdCliente"]),
                                Nome = (string)dr["Nome"],
                                Email = (string)dr["Email"],
                                Senha = (string)dr["Senha"],
                                CPF = (string)dr["CPF"]
                            }
                        );
                    }
                }
            }
            return clientes;
        }
        public void AlterarCliente(Usuario cliente)
        {
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("updateCliente", conexao))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vNome", cliente.Nome);
                    cmd.Parameters.AddWithValue("vEmail", cliente.Email);
                    cmd.Parameters.AddWithValue("vCPF", cliente.CPF);
                    cmd.Parameters.AddWithValue("vSenha", cliente.Senha);
                    cmd.ExecuteNonQuery();
                    conexao.Close();
                }
            }
        }
        public void ExcluirCliente(int id)
        {
            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using (var cmd = new MySqlCommand("DeleteCliente", conexao))
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
