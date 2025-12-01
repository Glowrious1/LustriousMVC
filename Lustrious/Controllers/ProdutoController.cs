using Lustrious.Data;
using Microsoft.AspNetCore.Mvc;
using Lustrious.Models;
using Lustrious.Repositorio;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.IO;


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

        public IActionResult Index(string q = null, int codCategoria =0, int codTipoProduto =0, int page =1)
        {
            const int pageSize =10;
            var result = _produtoRepositorio.ListarProdutos(q, codCategoria, codTipoProduto, page, pageSize);
            var items = result.Items.ToList();
            var total = result.TotalCount;
            var totalPages = (int)System.Math.Ceiling(total / (double)pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = total;

            ViewBag.FilterQ = q;
            ViewBag.FilterCategoria = codCategoria;
            ViewBag.FilterTipo = codTipoProduto;

            // supply categories and tipos for filter selects, set selected flags
            ViewBag.Categorias = _produtoRepositorio.GetCategorias(codCategoria).ToList();
            ViewBag.Tipos = _produtoRepositorio.GetTipos(codTipoProduto, codCategoria).ToList();

            return View(items);
        }

        // Nova ação pública para vitrine (clientes)
        [HttpGet]
        public IActionResult Vitrine(string q = null, int codCategoria =0, int codTipoProduto =0, int page =1)
        {
            const int pageSize =12; // mais itens por página na vitrine
            var result = _produtoRepositorio.ListarProdutos(q, codCategoria, codTipoProduto, page, pageSize);
            var items = result.Items.ToList();
            var total = result.TotalCount;
            var totalPages = (int)System.Math.Ceiling(total / (double)pageSize);

            // validate image files exist; if not, clear Foto so view uses placeholder
            foreach(var p in items)
            {
                if (!string.IsNullOrWhiteSpace(p.Foto))
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", p.Foto.TrimStart('/','\\'));
                    if (!System.IO.File.Exists(path))
                    {
                        p.Foto = null;
                    }
                }
            }

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = total;

            ViewBag.FilterQ = q;
            ViewBag.FilterCategoria = codCategoria;
            ViewBag.FilterTipo = codTipoProduto;

            ViewBag.Categorias = _produtoRepositorio.GetCategorias(codCategoria).ToList();
            ViewBag.Tipos = _produtoRepositorio.GetTipos(codTipoProduto, codCategoria).ToList();

            return View(items);
        }

        // Ação para detalhes do produto (vitrine)
        [HttpGet]
        public IActionResult AcharProduto(long id)
        {
            var produto = _produtoRepositorio.AcharProduto(id);
            if (produto == null)
            {
                return NotFound();
            }
            // ensure foto exists
            if (!string.IsNullOrWhiteSpace(produto.Foto))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", produto.Foto.TrimStart('/','\\'));
                if (!System.IO.File.Exists(path)) produto.Foto = null;
            }
            return View(produto);
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
