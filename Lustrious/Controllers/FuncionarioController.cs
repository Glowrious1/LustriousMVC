using Microsoft.AspNetCore.Mvc;
using Lustrious.Data;
using Lustrious.Models;
using MySql.Data.MySqlClient;
using System.Data;
using Microsoft.Net.Http.Headers;
using Lustrious.Repositorio;
using Lustrious.Autenticacao;

namespace Lustrious.Controllers
{
    public class FuncionarioController : Controller
    {
        private IFuncionarioRepositorio _funcionarioRepositorio;
        public FuncionarioController(IFuncionarioRepositorio funcionarioRepositorio)
        {
            _funcionarioRepositorio = funcionarioRepositorio;
        }

        [SessionAuthorize(RoleAnyOf = "Admin,Funcionario")]
        public IActionResult Index()
        {
            return View(_funcionarioRepositorio.ListarFuncionario());
        }

        [SessionAuthorize(RoleAnyOf = "Admin")]
        public IActionResult CriarFuncionario()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Admin")]
        public IActionResult CriarFuncionario(Usuario funcionario)
        {
            _funcionarioRepositorio.CadastrarFuncionario(funcionario);
            TempData["ok"] = "Funcionario Cadastrado!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [SessionAuthorize(RoleAnyOf = "Admin,Funcionario")]
        public IActionResult EditarFuncionario(int id)
        {
            return View(_funcionarioRepositorio.AcharFuncionario(id));
        }
        [HttpPost, ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Admin,Funcionario")]
        public IActionResult EditarFuncionario(Usuario model)
        {
            _funcionarioRepositorio.AlterarFuncionario(model);
            TempData["ok"] = "Funcionario Atualizado!";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost, ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Admin")]
        public IActionResult ExcluirFuncionario(int id)
        {
            _funcionarioRepositorio.ExcluirFuncionario(id);
            TempData["ok"] = "Funcionario Excluído!";
            return RedirectToAction(nameof(Index));
        }
    }
}
