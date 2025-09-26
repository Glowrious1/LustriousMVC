using Microsoft.AspNetCore.Mvc;

namespace Lustrious.Controllers
{
    public class ProdutoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
