using Lustrious.Data;
using Microsoft.AspNetCore.Mvc;
using Lustrious.Models;
using MySql.Data.MySqlClient;
using Lustrious.Repositorio;
using Microsoft.AspNetCore.Http;


namespace Lustrious.Controllers
{
    public class ProdutoController : Controller
    {
        //Criando uma variável do tipo IProdutoRepositorio
        private IProdutoRepostorio _produtoRepositorio;

        //Instanciando o repositório através do construtor
        public ProdutoController(IProdutoRepostorio produtoRepostorio)
        {
            //Atribuindo o repositório à variável _produtoRepositorio
            _produtoRepositorio = produtoRepostorio;
        }
        public IActionResult Index()
        {
            return View(_produtoRepositorio.ListarProdutos());
        }
        public IActionResult CriarProduto()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CriarProduto(Produto produto, IFormFile? foto)
        {
            _produtoRepositorio.CadastrarProduto(produto, foto);
            TempData["Ok"] = "Produto Cadastrado!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult EditarProduto(int id)
        {
            Produto produto = _produtoRepositorio.AcharProduto(id);
            return View(produto);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult EditarProduto(Produto model, IFormFile? foto)
        {
            _produtoRepositorio.AlterarProduto(model, foto);
            TempData["Ok"] = "Produto atualizado";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ExcluirProduto(int id)
        {
            _produtoRepositorio.ExcluirProduto(id);
            TempData["Ok"] = "Produto Excluído!";
            return RedirectToAction(nameof(Index));
        }
    }
}
