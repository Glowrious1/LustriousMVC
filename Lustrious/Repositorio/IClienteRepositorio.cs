using Lustrious.Models;
using Microsoft.AspNetCore.Http;
namespace Lustrious.Repositorio
{
    public interface IClienteRepositorio
    {
        public void CadastrarCliente(Usuario cliente, IFormFile? foto);
        public Usuario AcharCliente(int id);
        public IEnumerable<Usuario> ListarClientes();
        public void AlterarCliente(Usuario cliente, IFormFile? foto);
        public void ExcluirCliente(int id);
    }
}
