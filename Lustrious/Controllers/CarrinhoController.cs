using Lustrious.Repositorio;
using Microsoft.AspNetCore.Mvc;
using Lustrious.Autenticacao;

namespace Lustrious.Controllers
{
    public class CarrinhoController : Controller
    {
        private ICarrinhoRepositorio _carrinhoRepositorio;
        private IClienteRepositorio _clienteRepositorio;
        public CarrinhoController(ICarrinhoRepositorio carrinhoRepositorio, IClienteRepositorio clienteRepostorio)
        {
            _clienteRepositorio = clienteRepostorio;
            _carrinhoRepositorio = carrinhoRepositorio;
        }

        [SessionAuthorize]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
            var carrinho = _carrinhoRepositorio.AcharCarrinho(userId.Value);
            return View(carrinho);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdicionarItem(int produtoId)
        {
            var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
            var referer = Request.Headers["Referer"].ToString();
            if (userId == null)
            {
                // Inform user that login is required and redirect to login page with returnUrl
                TempData["Erro"] = "Você precisa entrar para adicionar produtos ao carrinho.";
                var returnUrl = string.IsNullOrEmpty(referer) ? Url.Action("Vitrine", "Produto") : referer;
                return RedirectToAction("Login", "Auth", new { returnUrl });
            }

            var added = _carrinhoRepositorio.AdicionarItem(produtoId, userId.Value);
            if (!added)
            {
                TempData["Erro"] = "Produto sem estoque ou indisponível.";
            }
            else
            {
                TempData["Ok"] = "Produto adicionado ao carrinho.";
            }

            // Prefer returning to the page the user came from (vitrine or details)
            if (!string.IsNullOrEmpty(referer))
                return Redirect(referer);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize]
        public IActionResult RemoverItem(int produtoId)
        {
            var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
            _carrinhoRepositorio.RemoverItem(produtoId, userId.Value);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize]
        public IActionResult RemoverTodosItens()
        {
            var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
            _carrinhoRepositorio.LimparCarrinho(userId.Value);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize]
        public IActionResult IncrementarItem(int produtoId)
        {
            var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
            _carrinhoRepositorio.IncrementarItem(produtoId, userId.Value);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize]
        public IActionResult DecrementarItem(int produtoId)
        {
            var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
            _carrinhoRepositorio.DecrementarItem(produtoId, userId.Value);
            return RedirectToAction(nameof(Index));
        }

        [SessionAuthorize]
        public IActionResult Checkout()
        {
            var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
            var carrinho = _carrinhoRepositorio.AcharCarrinho(userId.Value);
            return View(carrinho);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize]
        public IActionResult Checkout(int idEnd)
        {
            var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
            var notified = _carrinhoRepositorio.FinalizarCompra(idEnd, userId.Value);
            if (notified)
            {
                TempData["Ok"] = "Compra finalizada e notificação enviada.";
            }
            else
            {
                TempData["Ok"] = "Compra finalizada.";
            }
            return RedirectToAction("Index", "Venda");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdicionarItemAjax(int produtoId)
        {
            var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
            if (userId == null)
            {
                var referer = Request.Headers["Referer"].ToString();
                var returnUrl = string.IsNullOrEmpty(referer) ? Url.Action("Vitrine", "Produto") : referer;
                return Json(new { authenticated = false, redirectUrl = Url.Action("Login", "Auth", new { returnUrl }) });
            }

            var added = _carrinhoRepositorio.AdicionarItem(produtoId, userId.Value);
            var carrinho = _carrinhoRepositorio.AcharCarrinho(userId.Value);
            if (!added)
                return Json(new { success = false, message = "Produto sem estoque ou indisponível.", cartCount = carrinho?.Qtd ?? 0 });

            return Json(new { success = true, message = "Produto adicionado ao carrinho.", cartCount = carrinho?.Qtd ?? 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize]
        public IActionResult CheckoutAjax(int idEnd)
        {
            var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
            if (userId == null)
                return Json(new { success = false, message = "Usuário não autenticado." });

            var notified = _carrinhoRepositorio.FinalizarCompra(idEnd, userId.Value);
            return Json(new { success = true, notified = notified });
        }
    }
}
