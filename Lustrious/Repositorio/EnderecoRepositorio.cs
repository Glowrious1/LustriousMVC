using Lustrious.Data;
using Lustrious.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System;

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
 Cep = dr.Table.Columns.Contains("Cep") ? dr["Cep"].ToString() : string.Empty,
 logradouro = dr.Table.Columns.Contains("logradouro") ? dr["logradouro"].ToString() : string.Empty,
 numero = dr.Table.Columns.Contains("numero") ? dr["numero"].ToString() : string.Empty,
 complemento = dr.Table.Columns.Contains("complemento") ? dr["complemento"].ToString() : string.Empty,
 Idbairro = dr.Table.Columns.Contains("Idbairro") && dr["Idbairro"] != DBNull.Value ? Convert.ToInt32(dr["Idbairro"]) :0,
 Idcidade = dr.Table.Columns.Contains("Idcidade") && dr["Idcidade"] != DBNull.Value ? Convert.ToInt32(dr["Idcidade"]) :0,
 Idestado = dr.Table.Columns.Contains("Idestado") && dr["Idestado"] != DBNull.Value ? Convert.ToInt32(dr["Idestado"]) :0,
 IdUser = dr.Table.Columns.Contains("IdUser") && dr["IdUser"] != DBNull.Value ? Convert.ToInt32(dr["IdUser"]) :0
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

 public void AtualizarEndereco(Endereco endereco)
 {
 using var conn = _dataBase.GetConnection();
 using var cmd = new MySqlCommand("updateEndereco", conn);
 cmd.CommandType = System.Data.CommandType.StoredProcedure;
 cmd.Parameters.AddWithValue("vIdEndereco", endereco.IdEndereco);
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

 public Endereco ObterEnderecoPorId(int id)
 {
 using var conn = _dataBase.GetConnection();
 using var cmd = new MySqlCommand("SELECT * FROM endereco WHERE id_endereco = @id", conn);
 cmd.Parameters.AddWithValue("@id", id);
 using var reader = cmd.ExecuteReader();
 if (reader.Read())
 {
 return new Endereco
 {
 IdEndereco = reader["id_endereco"] == DBNull.Value ?0 : Convert.ToInt32(reader["id_endereco"]),
 Cep = reader["cep"] == DBNull.Value ? string.Empty : reader["cep"].ToString(),
 logradouro = reader["logradouro"] == DBNull.Value ? string.Empty : reader["logradouro"].ToString(),
 numero = reader["numero"] == DBNull.Value ? string.Empty : reader["numero"].ToString(),
 complemento = reader["complemento"] == DBNull.Value ? string.Empty : reader["complemento"].ToString(),
 Idbairro = reader["id_bairro"] == DBNull.Value ?0 : Convert.ToInt32(reader["id_bairro"]),
 Idcidade = reader["id_cidade"] == DBNull.Value ?0 : Convert.ToInt32(reader["id_cidade"]),
 Idestado = reader["id_estado"] == DBNull.Value ?0 : Convert.ToInt32(reader["id_estado"]),
 IdUser = reader["id_user"] == DBNull.Value ?0 : Convert.ToInt32(reader["id_user"])
 };
 }
 return null;
 }
 }
}
