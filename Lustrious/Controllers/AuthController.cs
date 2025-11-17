using Lustrious.Data;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Lustrious.Autenticacao;
using Org.BouncyCastle.Crypto.Generators;

namespace Lustrious.Controllers
{
    public class AuthController : Controller
    {
        private readonly DataBase db = new DataBase();
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Login(string email, string senha, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                ViewBag.Error = "Por favor, preencha email e senha.";
                return View();
            }


            using var conexao = db.GetConnection();
            using var cmd = new MySqlCommand("ObterUsuarioEmail", conexao);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("p_email", email);
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                ViewBag.Erro = "Email ou senha inválidos.";
                return View();
            }
            var id = reader.GetInt32("IdUser");
            var nome = reader.GetString("Nome");
            var role = reader.GetString("Role");
            var ativo = reader.GetBoolean("Ativo");
            var senhaHash = reader["senha"] as string ?? "";

            if (!ativo)
            {
                ViewBag.Erro = "Usuário inativo. Contate o administrador.";
                return View();
            }
            bool ok;
            try
            {
                ok = BCrypt.Net.BCrypt.Verify(senha, senhaHash);
            }
            catch
            { ok = false; }
            if(!ok)
            {                 ViewBag.Erro = "Email ou senha inválidos.";
                return View();
            }
            HttpContext.Session.SetInt32(SessionsKeys.UserId, id);
            HttpContext.Session.SetString(SessionsKeys.UserName, nome);
            HttpContext.Session.SetString(SessionsKeys.UserEmail, email);
            HttpContext.Session.SetString(SessionsKeys.UserRole, role);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");


        }

        [HttpPost,ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult AcessoNegado() => View();
    }
}
