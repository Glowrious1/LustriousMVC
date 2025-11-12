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
        public void CadastrarCliente(Cliente cliente)
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
        public Cliente AcharCliente(int id)
        {
            Cliente cliente = new Cliente();
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
                        cliente = new Cliente()
                        {
                            IdClient = Convert.ToInt32(dr["IdCliente"]),
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
        public IEnumerable<Cliente> ListarClientes()
        {
            List<Cliente> clientes = new List<Cliente>();
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
                        clientes.Add(new Cliente
                            {
                                IdClient = Convert.ToInt32(dr["IdCliente"]),
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
        public void AlterarCliente(Cliente cliente)
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
