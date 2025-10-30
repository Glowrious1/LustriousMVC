using Lustrious.Models;
namespace Lustrious.Repositorio
{
    public interface IClienteRepositorio
    {
        public void CadastrarCliente(Cliente cliente);
        public Cliente AcharCliente(int id);
        public IEnumerable<Cliente> ListarClientes();
        public void AlterarCliente(Cliente cliente);
        public void ExcluirCliente(int id);
    }
}
