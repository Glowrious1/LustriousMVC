using Lustrious.Data;
using Lustrious.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace Lustrious.Repositorio
{
 public class EnderecoRepositorio : IEnderecoRepositorio
 {
 private readonly DataBase _dataBase;
 public EnderecoRepositorio(DataBase dataBase)
 {
 _dataBase = dataBase;
 }

 public IEnumerable<Endereco> ListarEnderecosPorUsuario(int userId)
 {
 var lista = new List<Endereco>();
 using var conn = _dataBase.GetConnection();
 using var cmd = new MySqlCommand("obterEnderecosPorUsuario", conn);
 cmd.CommandType = System.Data.CommandType.StoredProcedure;
 cmd.Parameters.AddWithValue("vUserId", userId);
 var da = new MySqlDataAdapter(cmd);
 var dt = new DataTable();
 da.Fill(dt);
 foreach (DataRow dr in dt.Rows)
 {
 lista.Add(new Endereco
 {
 IdEndereco = Convert.ToInt32(dr["IdEndereco"]),
 Cep = dr["Cep"].ToString(),
 logradouro = dr["logradouro"].ToString(),
 numero = dr["numero"].ToString(),
 complemento = dr["complemento"].ToString(),
 Idbairro = Convert.ToInt32(dr["Idbairro"]),
 Idcidade = Convert.ToInt32(dr["Idcidade"]),
 Idestado = Convert.ToInt32(dr["Idestado"]),
 IdUser = Convert.ToInt32(dr["IdUser"]) 
 });
 }
 return lista;
 }

 public void CadastrarEndereco(Endereco endereco)
 {
 using var conn = _dataBase.GetConnection();
 using var cmd = new MySqlCommand("insertEndereco", conn);
 cmd.CommandType = System.Data.CommandType.StoredProcedure;
 cmd.Parameters.AddWithValue("vCep", endereco.Cep);
 cmd.Parameters.AddWithValue("vLogradouro", endereco.logradouro);
 cmd.Parameters.AddWithValue("vNumero", endereco.numero);
 cmd.Parameters.AddWithValue("vComplemento", endereco.complemento);
 cmd.Parameters.AddWithValue("vIdBairro", endereco.Idbairro);
 cmd.Parameters.AddWithValue("vIdCidade", endereco.Idcidade);
 cmd.Parameters.AddWithValue("vIdEstado", endereco.Idestado);
 cmd.Parameters.AddWithValue("vIdUser", endereco.IdUser);
 cmd.ExecuteNonQuery();
 }
 }
}
