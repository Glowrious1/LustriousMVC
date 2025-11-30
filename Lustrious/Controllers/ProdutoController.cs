using Lustrious.Data;
using Microsoft.AspNetCore.Mvc;
using Lustrious.Models;
using Lustrious.Repositorio;
using Microsoft.AspNetCore.Http;
using System.Linq;


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
            var model = new Produto();
            model.CategoriaNome = _produtoRepositorio.GetCategorias().ToList();
            model.TipoProdutoNome = _produtoRepositorio.GetTipos().ToList();
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CriarProduto(Produto produto, IFormFile? foto)
        {
            try
            {
                _produtoRepositorio.CadastrarProduto(produto, foto);
                TempData["Ok"] = "Produto Cadastrado!";
            }
            catch (System.Exception ex)
            {
                // Log exception if logging available
                TempData["Erro"] = "Erro ao cadastrar produto: " + ex.Message;
                // Re-populate selects and return view with model
                produto.CategoriaNome = _produtoRepositorio.GetCategorias(produto.CodCategoria).ToList();
                produto.TipoProdutoNome = _produtoRepositorio.GetTipos(produto.CodTipoProduto, produto.CodCategoria).ToList();
                return View(produto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult EditarProduto(long id)
        {
            Produto produto = _produtoRepositorio.AcharProduto(id);
            produto.CategoriaNome = _produtoRepositorio.GetCategorias(produto.CodCategoria).ToList();
            produto.TipoProdutoNome = _produtoRepositorio.GetTipos(produto.CodTipoProduto, produto.CodCategoria).ToList();
            return View(produto);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult EditarProduto(Produto model, IFormFile? foto)
        {
            try
            {
                _produtoRepositorio.AlterarProduto(model, foto);
                TempData["Ok"] = "Produto atualizado";
            }
            catch (System.Exception ex)
            {
                TempData["Erro"] = "Erro ao atualizar produto: " + ex.Message;
                model.CategoriaNome = _produtoRepositorio.GetCategorias(model.CodCategoria).ToList();
                model.TipoProdutoNome = _produtoRepositorio.GetTipos(model.CodTipoProduto, model.CodCategoria).ToList();
                return View(model);
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ExcluirProduto(long id)
        {
            try
            {
                _produtoRepositorio.ExcluirProduto(id);
                TempData["Ok"] = "Produto Excluído!";
            }
            catch (System.Exception ex)
            {
                TempData["Erro"] = "Erro ao excluir produto: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // AJAX endpoint para carregar tipos por categoria
        [HttpGet]
        public IActionResult GetTiposPorCategoria(int codCategoria)
        {
            var tipos = _produtoRepositorio.GetTipos(null, codCategoria)
                .Select(x => new { value = x.Value, text = x.Text })
                .ToList();
            return Json(tipos);
        }
    }
}
