using Lustrious.Data;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Lustrious.Autenticacao;
using Org.BouncyCastle.Crypto.Generators;

namespace Lustrious.Controllers
{
    public class AuthController : Controller
    {
        private readonly DataBase _db;

        public AuthController(DataBase db)
        {
            _db = db;
        }

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

            using var conexao = _db.GetConnection();
            conexao.Open(); // <<-- ensure connection is open before executing commands

            using var cmd = new MySqlCommand("ObterUsuarioEmail", conexao);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("p_email", email);

            using var reader = cmd.ExecuteReader();
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
            
            bool credentialsOk = false;

            // If stored password is a bcrypt hash it typically starts with "$2" (e.g. $2a$, $2b$, $2y$
            var isBcrypt = !string.IsNullOrEmpty(senhaHash) && senhaHash.StartsWith("$2");

            try
            {
                if (isBcrypt)
                {
                    credentialsOk = BCrypt.Net.BCrypt.Verify(senha, senhaHash);
                }
                else
                {
                    // Legacy plain-text password in DB: compare directly
                    if (senha == senhaHash)
                    {
                        credentialsOk = true;
                        // upgrade to bcrypt: hash and update DB
                        var newHash = BCrypt.Net.BCrypt.HashPassword(senha, workFactor:12);
                        reader.Close(); // close reader before executing another command on same connection
                        using var upd = new MySqlCommand("UPDATE Usuario SET Senha = @senha WHERE IdUser = @id", conexao);
                        upd.Parameters.AddWithValue("@senha", newHash);
                        upd.Parameters.AddWithValue("@id", id);
                        upd.ExecuteNonQuery();
                        senhaHash = newHash;
                    }
                    else
                    {
                        credentialsOk = false;
                    }
                }
            }
            catch
            {
                credentialsOk = false;
            }

            if (!credentialsOk)
            {
                ViewBag.Erro = "Email ou senha inválidos.";
                return View();
            }

            HttpContext.Session.SetInt32(SessionsKeys.UserId, id);
            HttpContext.Session.SetString(SessionsKeys.UserName, nome);
            HttpContext.Session.SetString(SessionsKeys.UserEmail, email);
            HttpContext.Session.SetString(SessionsKeys.UserRole, role);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // Redirect based on role
            var roleLower = (role ?? string.Empty).ToLower();
            if (roleLower == "cliente")
            {
                return RedirectToAction("Index", "Produto");
            }
            else
            {
                // Admin or Funcionario -> dashboard
                return RedirectToAction("Index", "Funcionario");
            }
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
