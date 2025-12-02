using Lustrious.Models;
namespace Lustrious.Repositorio
{
    public interface IFuncionarioRepositorio
    {
        public void CadastrarFuncionario(Usuario funcionario, IFormFile? foto);
        public Usuario AcharFuncionario(int id);
        public IEnumerable<Usuario> ListarFuncionario();
        public void AlterarFuncionario(Usuario funcionario, IFormFile? foto);
        public void ExcluirFuncionario(int id);
    }
}
