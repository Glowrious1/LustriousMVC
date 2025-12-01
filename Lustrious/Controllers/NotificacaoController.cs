using Microsoft.AspNetCore.Mvc;
using Lustrious.Repositorio;
using Lustrious.Autenticacao;

namespace Lustrious.Controllers
{
 [SessionAuthorize]
 public class NotificacaoController : Controller
 {
 private readonly IVendaRepositorio _vendaRepositorio;
 public NotificacaoController(IVendaRepositorio vendaRepositorio)
 {
 _vendaRepositorio = vendaRepositorio;
 }

 public IActionResult Index()
 {
 var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
 if (userId == null) return RedirectToAction("Login", "Auth");
 var nots = _vendaRepositorio.ListarNotificacoes(userId.Value);
 return View(nots);
 }

 [HttpPost]
 [ValidateAntiForgeryToken]
 public IActionResult MarcarUltimasComoLidas([FromBody] dynamic body)
 {
 var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
 if (userId == null) return Unauthorized();
 int max =5;
 try { max = (int)body.max; } catch { }
 _vendaRepositorio.MarcarUltimasNotificacoesComoLidas(userId.Value, max);
 return Ok();
 }
 }
}
