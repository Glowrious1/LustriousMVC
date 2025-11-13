using Lustrious.Models;
namespace Lustrious.Repositorio
{
    public interface IFuncionarioRepositorio
    {
        public void CadastrarFuncionario(Usuario funcionario);
        public Usuario AcharFuncionario(int id);
        public IEnumerable<Usuario> ListarFuncionario();
        public void AlterarFuncionario(Usuario funcionario);
        public void ExcluirFuncionario(int id);
    }
}
