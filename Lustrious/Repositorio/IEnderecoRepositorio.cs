using Lustrious.Models;
using System.Collections.Generic;

namespace Lustrious.Repositorio
{
 public interface IEnderecoRepositorio
 {
 IEnumerable<Endereco> ListarEnderecosPorUsuario(int userId);
 void CadastrarEndereco(Endereco endereco);
 void AtualizarEndereco(Endereco endereco);
 }
}
