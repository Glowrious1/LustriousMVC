using Lustrious.Repositorio;
using Microsoft.AspNetCore.Mvc;
using Lustrious.Autenticacao;
using Lustrious.Models;

namespace Lustrious.Controllers
{
 [SessionAuthorize]
 public class EnderecoController : Controller
 {
 private readonly IEnderecoRepositorio _enderecoRepositorio;
 public EnderecoController(IEnderecoRepositorio enderecoRepositorio)
 {
 _enderecoRepositorio = enderecoRepositorio;
 }

 public IActionResult Index()
 {
 var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
 if (userId == null)
 return RedirectToAction("Login", "Auth");
 var enderecos = _enderecoRepositorio.ListarEnderecosPorUsuario(userId.Value);
 return View(enderecos);
 }

 public IActionResult Criar()
 {
 return View();
 }
 [HttpPost, ValidateAntiForgeryToken]
 public IActionResult Criar(Endereco model)
 {
 var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
 if (userId == null)
 return RedirectToAction("Login","Auth");
 model.IdUser = userId.Value;
 _enderecoRepositorio.CadastrarEndereco(model);
 TempData["Ok"] = "Endereço cadastrado";
 return RedirectToAction(nameof(Index));
 }
 }
}
