using Lustrious.Data;
using Lustrious.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Lustrious.Controllers
{
    public class FuncionarioController : Controller
    {
        public readonly DataBase db = new DataBase();
        public IActionResult Index()

        {
            var lista = new List<Funcionario>();
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("selectFuncionario", conn) { CommandType = System.Data.CommandType.StoredProcedure };
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Funcionario
                {
                    IdFun = rd.GetInt32("IdFun"),
                    Nome = rd.GetString("nome"),
                    Email = rd.GetString("email"),
                    Senha = rd.GetString("Senha")
                });
            }
            return View(lista);
        }
        public IActionResult CriarFuncionario()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CriarFuncionario(Funcionario funcionario)
        {
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("insertFuncionario", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("vNome", funcionario.Nome);
            cmd.Parameters.AddWithValue("vEmail", funcionario.Email);
            cmd.Parameters.AddWithValue("vSenha", funcionario.Senha);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Funcionario Cadastrado!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult EditarFuncionario(int id)
        {
            using var conn = db.GetConnection();

            Funcionario? funcionario = null;
            using (var cmd = new MySqlCommand("obterFuncionario", conn)
            { CommandType = System.Data.CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("vId", id);
                using var rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    funcionario = new Funcionario
                    {
                        IdFun = rd.GetInt32("IdFun"),
                        Nome = rd.GetString("Nome"),
                        Email = rd.GetString("Email"),
                        Senha = rd.GetString("Senha"),

                    };
                }
            }
            return View(funcionario);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult EditarFuncionario(Funcionario model)
        {
            if (model.IdFun <= 0) return NotFound();
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                ModelState.AddModelError("", "Informe seu Nome");
            }
            using var conn2 = db.GetConnection();
            using var cmd = new MySqlCommand("UpdateFuncionario", conn2) { CommandType = System.Data.CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("vIdFun", model.IdFun);
            cmd.Parameters.AddWithValue("vNome", model.Nome);
            cmd.Parameters.AddWithValue("vEmail", model.Email);
            cmd.Parameters.AddWithValue("vSenha", model.Senha);
            cmd.ExecuteNonQuery();

            TempData["ok"] = "Funcionario Atualizado!";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ExcluirFuncionario(int id)
        {
            using var conn = db.GetConnection();
            try
            {
                using var cmd = new MySqlCommand("DeleteFuncionario", conn) { CommandType = System.Data.CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("vIdFun", id);
                cmd.ExecuteNonQuery();
                TempData["ok"] = "Funcionario Excluido!";
            }
            catch (MySqlException ex)
            {
                TempData["ok"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
