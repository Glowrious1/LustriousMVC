using Lustrious.Repositorio;
using Microsoft.AspNetCore.Mvc;
using Lustrious.Autenticacao;
using Lustrious.Models;

namespace Lustrious.Controllers
{
 [SessionAuthorize]
 public class VendaController : Controller
 {
 private readonly IVendaRepositorio _vendaRepositorio;

 public VendaController(IVendaRepositorio vendaRepositorio)
 {
 _vendaRepositorio = vendaRepositorio;
 }

 public IActionResult Index()
 {
 var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
 if (userId == null)
 return RedirectToAction("Login", "Auth");

 var vendas = _vendaRepositorio.ListarVendasPorUsuario(userId.Value);
 return View(vendas);
 }

 public IActionResult Details(int id)
 {
 var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
 if (userId == null)
 return RedirectToAction("Login", "Auth");

 var venda = _vendaRepositorio.AcharVenda(id);
 if (venda == null)
 return NotFound();

 if (venda.IdUser != userId.Value)
 return Forbid();

 return View(venda);
 }
 }
}
