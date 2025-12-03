using System.Diagnostics;
using Lustrious.Models;
using Microsoft.AspNetCore.Mvc;
using Lustrious.Repositorio;

namespace Lustrious.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProdutoRepostorio _produtoRepositorio;

        public HomeController(ILogger<HomeController> logger, IProdutoRepostorio produtoRepositorio)
        {
            _logger = logger;
            _produtoRepositorio = produtoRepositorio;
        }

        public IActionResult Index()
        {
            // carregar alguns produtos reais para a home (6 itens)
            var produtos = _produtoRepositorio.ListarProdutosPublico(pageSize:6).ToList();
            return View(produtos);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
