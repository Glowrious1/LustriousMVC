using Lustrious.Repositorio;
using Microsoft.AspNetCore.Mvc;
using Lustrious.Autenticacao;

namespace Lustrious.Controllers
{
    [SessionAuthorize]
    public class CarrinhoController : Controller
    {
        private ICarrinhoRepositorio _carrinhoRepositorio;
        private IClienteRepositorio _clienteRepositorio;
        public CarrinhoController(ICarrinhoRepositorio carrinhoRepositorio, IClienteRepositorio clienteRepostorio)
        {
            _clienteRepositorio = clienteRepostorio;
            _carrinhoRepositorio = carrinhoRepositorio;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var carrinho = _carrinhoRepositorio.AcharCarrinho(userId.Value);
            return View(carrinho);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdicionarItem(int produtoId)
        {
            var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            _carrinhoRepositorio.AdicionarItem(produtoId, userId.Value);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoverItem(int produtoId)
        {
            var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            _carrinhoRepositorio.RemoverItem(produtoId, userId.Value);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoverTodosItens()
        {
            var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            _carrinhoRepositorio.LimparCarrinho(userId.Value);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Checkout()
        {
            var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var carrinho = _carrinhoRepositorio.AcharCarrinho(userId.Value);
            return View(carrinho);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(int idEnd)
        {
            var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            _carrinhoRepositorio.FinalizarCompra(idEnd, userId.Value);
            // após finalizar, redireciona para histórico de vendas ou para confirmação
            return RedirectToAction("Index", "Venda");
        }
    }
}
