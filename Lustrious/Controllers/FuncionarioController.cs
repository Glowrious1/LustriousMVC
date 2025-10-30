using Lustrious.Data;
using Lustrious.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Lustrious.Repositorio;

namespace Lustrious.Controllers
{
    public class FuncionarioController : Controller
    {
        private IFuncionarioRepositorio _funcionarioRepositorio;
        public FuncionarioController(IFuncionarioRepositorio funcionarioRepositorio)
        {
            _funcionarioRepositorio = funcionarioRepositorio;
        }
        public readonly DataBase db = new DataBase();
        public IActionResult Index()
        {
            return View(_funcionarioRepositorio.ListarFuncionarios());
        }
        public IActionResult CriarFuncionario()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CriarFuncionario(Funcionario funcionario)
        {
            _funcionarioRepositorio.CadastrarFuncionario(funcionario);
            TempData["ok"] = "Funcionario Cadastrado!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult EditarFuncionario(int id)
        {
            return View(_funcionarioRepositorio.AcharFuncionario(id));
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult EditarFuncionario(Funcionario model)
        {
            _funcionarioRepositorio.AlterarFuncionario(model);
            TempData["ok"] = "Funcionario Atualizado!";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ExcluirFuncionario(int id)
        {
            _funcionarioRepositorio.ExcluirFuncionario(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
