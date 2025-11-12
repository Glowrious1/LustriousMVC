using Lustrious.Models;
namespace Lustrious.Repositorio
{
    public interface IClienteRepositorio
    {
        public void CadastrarCliente(Usuario cliente);
        public Usuario AcharCliente(int id);
        public IEnumerable<Usuario> ListarClientes();
        public void AlterarCliente(Usuario cliente);
        public void ExcluirCliente(int id);
    }
}
