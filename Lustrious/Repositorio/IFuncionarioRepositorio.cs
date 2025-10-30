using Lustrious.Models;

namespace Lustrious.Repositorio
{
    public interface IFuncionarioRepositorio
    {
        public void CadastrarFuncionario(Funcionario funcionario);
        public Funcionario AcharFuncionario(int id);
        public IEnumerable<Funcionario> ListarFuncionarios();
        public void AlterarFuncionario(Funcionario funcionario);
        public void ExcluirFuncionario(int id);
    }
}
