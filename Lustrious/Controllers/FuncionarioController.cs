using Microsoft.AspNetCore.Mvc;

namespace Lustrious.Controllers
{
    public class FuncionarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
