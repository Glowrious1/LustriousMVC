using Lustrious.Repositorio;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Lustrious.Controllers
{
    [Authorize]
    public class FavoritosController : Controller
    {
        private readonly IFavoritosRepositorio _favoritosRepositorio;
        private readonly IClienteRepositorio _clienteRepositorio;
        public FavoritosController(IClienteRepositorio clienteRepositorio, IFavoritosRepositorio favoritosRepositorio)
        {
            _clienteRepositorio = clienteRepositorio;
            _favoritosRepositorio = favoritosRepositorio;
        }

        private int? GetCurrentUserId()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            // Tenta pegar claim padrão de NameIdentifier ou uma claim personalizada 'userId'
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("userId");
            if (claim == null)
                return null;

            if (int.TryParse(claim.Value, out var id))
                return id;

            return null;
        }

        public IActionResult Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var lista = _favoritosRepositorio.AcharFavoritos(userId.Value);
            return View(lista);
        }

        [HttpPost]
        public IActionResult AdicionarFavorito(int produtoId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            _favoritosRepositorio.AdicionarFavorito(produtoId, userId.Value);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult RemoverFavorito(int produtoId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            _favoritosRepositorio.RemoverFavorito(produtoId, userId.Value);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult RemoverTodosFavoritos()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            _favoritosRepositorio.LimparFavoritos(userId.Value);
            return RedirectToAction(nameof(Index));
        }
    }
}
