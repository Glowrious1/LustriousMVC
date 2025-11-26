using Microsoft.AspNetCore.Mvc;
using Lustrious.Data;
using Lustrious.Models;
using MySql.Data.MySqlClient;
using System.Data;
using Microsoft.Net.Http.Headers;
using Lustrious.Repositorio;
using Microsoft.AspNetCore.Http;

namespace Lustrious.Controllers
{
    public class ClienteController : Controller
    {
        private IClienteRepositorio _clienteRepositorio;
        public ClienteController(IClienteRepositorio clienteRepositorio)
        {
            _clienteRepositorio = clienteRepositorio;
        }
        public IActionResult Index()
        {
            return View(_clienteRepositorio.ListarClientes());
        }
        public IActionResult CriarCliente()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CriarCliente(Usuario cliente, IFormFile foto)
        {
            _clienteRepositorio.CadastrarCliente(cliente, foto);
            TempData["ok"] = "Cliente Cadastrado!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult EditarCliente(int id)
        {
            return View(_clienteRepositorio.AcharCliente(id));
        }
        [HttpPost, ValidateAntiForgeryToken]
         public IActionResult EditarCliente(Usuario model)
        {
            _clienteRepositorio.AlterarCliente(model);
            TempData["ok"] = "Cliente Atualizado!";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ExcluirCliente(int id)
        {
            _clienteRepositorio.ExcluirCliente(id);
            TempData["ok"] = "Cliente Excluído!";
            return RedirectToAction(nameof(Index));
        }
    }
}
