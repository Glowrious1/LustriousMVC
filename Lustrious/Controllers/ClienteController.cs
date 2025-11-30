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
        public IActionResult EditarCliente(Usuario model, IFormFile? foto)
        {
            _clienteRepositorio.AlterarCliente(model, foto);
            TempData["ok"] = "Cliente Atualizado!";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ExcluirCliente(int id)
        {
            try
            {
                _clienteRepositorio.ExcluirCliente(id);

                TempData["ok"] = "Cliente Desativado!";
            }
            catch (MySqlException ex)
            {
                if (ex.Number ==1451)
                {
                    TempData["erro"] = "Erro: Este cliente não pode ser excluído, pois possui pedidos ou dados associados em outras tabelas. Exclua as dependências primeiro.";
                }
                else
                {
                    TempData["erro"] = $"Erro ao excluir no banco de dados: {ex.Message}";
                }
            }
            catch (Exception)
            {
                TempData["erro"] = "Não foi possível excluir o cliente. Ocorreu um erro inesperado.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ReativarCliente(int id)
        {
            try
            {
                _clienteRepositorio.ReativarCliente(id);
                TempData["ok"] = "Cliente reativado!";
            }
            catch (Exception ex)
            {
                TempData["erro"] = "Erro ao reativar cliente: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
