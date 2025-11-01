using Lustrious.Repositorio;
using Microsoft.AspNetCore.Mvc;

namespace Lustrious.Controllers
{
    public class FavoritosController : Controller
    {
        private readonly IFavoritosRepositorio _favoritosRepositorio;
        private readonly IClienteRepositorio _clienteRepositorio;
        public FavoritosController(IClienteRepositorio clienteRepositorio, IFavoritosRepositorio favoritosRepositorio)
        {
            _clienteRepositorio = clienteRepositorio;
            _favoritosRepositorio = favoritosRepositorio;
        }

        public IActionResult Index()
        {
            int userId = 1; //Depois a gente pega o user logado
            return View(_favoritosRepositorio.AcharFavoritos(userId));
        }

        [HttpPost]
        public IActionResult AdicionarFavorito(int produtoId)
        {
            int userId = 1; //Depois a gente pega o user logado
            _favoritosRepositorio.AdicionarFavorito(produtoId, userId);
            return View();
        }

        [HttpDelete]
        public IActionResult RemoverFavorito(int produtoId)
        {
            int userId = 1; //Depois a gente pega o user logado
            _favoritosRepositorio.RemoverFavorito(produtoId, userId);
            return View();
        }

        public IActionResult RemoverTodosFavoritos()
        {
            int userId = 1; //Depois a gente pega o user logado
            _favoritosRepositorio.LimparFavoritos(userId);
            return View();
        }
    }
}
