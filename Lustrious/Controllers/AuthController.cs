using Microsoft.AspNetCore.Mvc;

namespace Lustrious.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
