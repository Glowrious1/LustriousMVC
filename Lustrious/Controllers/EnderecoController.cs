using Lustrious.Repositorio;
using Microsoft.AspNetCore.Mvc;
using Lustrious.Autenticacao;
using Lustrious.Models;
using System.Linq;

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
 if (!ModelState.IsValid)
 return View(model);

 var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
 if (userId == null)
 return RedirectToAction("Login","Auth");
 model.IdUser = userId.Value;
 _enderecoRepositorio.CadastrarEndereco(model);
 TempData["Ok"] = "Endereço cadastrado";
 return RedirectToAction(nameof(Index));
 }

 [HttpGet]
 public IActionResult Editar(int id)
 {
 var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
 if (userId == null)
 return RedirectToAction("Login", "Auth");
 var lista = _enderecoRepositorio.ListarEnderecosPorUsuario(userId.Value);
 var model = lista.FirstOrDefault(e => e.IdEndereco == id);
 if (model == null)
 return NotFound();
 return View(model);
 }

 [HttpPost, ValidateAntiForgeryToken]
 public IActionResult Editar(Endereco model)
 {
 if (!ModelState.IsValid)
 return View(model);

 var userId = HttpContext.Session.GetInt32(SessionsKeys.UserId);
 if (userId == null)
 return RedirectToAction("Login","Auth");
 model.IdUser = userId.Value;

 // Validate if the address belongs to the user
 var existingEndereco = _enderecoRepositorio.ObterEnderecoPorId(model.IdEndereco);
 if (existingEndereco == null || existingEndereco.IdUser != userId.Value)
 {
 return Forbid(); // or handle unauthorized access accordingly
 }

 _enderecoRepositorio.AtualizarEndereco(model);
 TempData["Ok"] = "Endereço atualizado";
 return RedirectToAction(nameof(Index));
 }
 }
}
