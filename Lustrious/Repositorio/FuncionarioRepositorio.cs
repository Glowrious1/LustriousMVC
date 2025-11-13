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
        private readonly DataBase dataBase;

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
                    cmd.Parameters.AddWithValue("vRole", "Funcionario");
                    cmd.Parameters.AddWithValue("vSexo", funcionario.Sexo);
                    cmd.Parameters.AddWithValue("vFoto", funcionario.Foto);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
