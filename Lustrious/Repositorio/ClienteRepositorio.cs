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
                relPath = Path.Combine("fotoUsuario", fileName).Replace("\\", "/");
            }

            using (var conexao = _dataBase.GetConnection())
            {
                conexao.Open();
                using(var cmd = new MySqlCommand("insertUsuario", conexao))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vNome", cliente.Nome);
                    cmd.Parameters.AddWithValue("vEmail", cliente.Email);
                    cmd.Parameters.AddWithValue("vCPF", cliente.CPF);
                    cmd.Parameters.AddWithValue("vSenha", cliente.Senha);
                    cmd.Parameters.AddWithValue("vRole", "Cliente");
                    cmd.Parameters.AddWithValue("vSexo", cliente.Sexo);
                    cmd.Parameters.AddWithValue("vFoto", cliente.Foto);
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
                using (var cmd = new MySqlCommand("selectUsuario", conexao))
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
                                IdUser = Convert.ToInt32(dr["IdUser"]),
                                Nome = (string)dr["Nome"],
                                Email = (string)dr["Email"],
                                Senha = (string)dr["Senha"],
                                Sexo = (string)dr["Sexo"],
                                CPF = (string)dr["CPF"],
                                Role = (string)dr["Role"],
                                CEP = (int)dr["CEP"]
                        });
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
                using (var cmd = new MySqlCommand("updateUsuario", conexao))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vNome", cliente.Nome);
                    cmd.Parameters.AddWithValue("vEmail", cliente.Email);
                    cmd.Parameters.AddWithValue("vCPF", cliente.CPF);
                    cmd.Parameters.AddWithValue("vSenha", cliente.Senha);
                    cmd.Parameters.AddWithValue("vSexo", cliente.Sexo);
                    cmd.Parameters.AddWithValue("vCEP", cliente.CEP);
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
                using (var cmd = new MySqlCommand("DeleteUsuario", conexao))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vId", id);
                    cmd.ExecuteNonQuery();
                    conexao.Close();
                }
            }
        }

        public void CadastrarCliente(Usuario cliente)
        {
            throw new NotImplementedException();
        }
    }
}
