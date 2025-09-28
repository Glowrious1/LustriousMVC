using Microsoft.AspNetCore.Mvc;
using Lustrious.Data;
using Lustrious.Models;
using MySql.Data.MySqlClient;
using System.Data;
using Microsoft.Net.Http.Headers;

namespace Lustrious.Controllers
{
    public class ClienteController : Controller
    {
        public readonly DataBase db = new DataBase();
        public IActionResult Index()

        {
            var lista = new List<Cliente>();
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("selectCliente", conn) { CommandType = System.Data.CommandType.StoredProcedure };
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Cliente
                {
                    IdClient = rd.GetInt32("IdClient"),
                    Nome = rd.GetString("nome"),
                    Email = rd.GetString("email"),
                    CPF = rd.GetString("CPF"),
                    Senha = rd.GetString("Senha")
                });
            }
            return View(lista);
        }
        public IActionResult CriarCliente()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CriarCliente(Cliente cliente)
        {
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("InsertCliente", conn);
            cmd.Parameters.AddWithValue("vNome", cliente.Nome);
            cmd.Parameters.AddWithValue ("vEmail", cliente.Email);
            cmd.Parameters.AddWithValue("vSenha", cliente.Senha);
            cmd.Parameters.AddWithValue("vCPF", cliente.CPF);
            cmd.ExecuteNonQuery();
            return RedirectToAction("CriarAutor");
        }
        [HttpGet]
        public IActionResult EditarCliente(int id)
        {
             using var conn = db.GetConnection();   
             Cliente? cliente = null;
            using (var cmd = new MySqlCommand("ObterCliente", conn) { CommandType = System.Data.CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("vId", id);
                using var rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    cliente = new Cliente
                    {
                        IdClient = rd.GetInt32("vIdClient"),
                        Nome = rd.GetString("vNome"),
                        Email = rd.GetString("vEmail"),
                        Senha = rd.GetString("vSenha"),
                        CPF = rd.GetString("vCPF")

                    };
                }
            }
            return View(cliente);
        }
        [HttpPost, ValidateAntiForgeryToken]

         public IActionResult EditarCliente(Cliente model)
        {
            if (model.IdClient <= 0) return NotFound();
            if(string.IsNullOrWhiteSpace(model.Email))
              {
                ModelState.AddModelError("", "Informe seu Email");
            }
            using var conn2 = db.GetConnection();
            using var cmd = new MySqlCommand("UpdateCliente", conn2) { CommandType = System.Data.CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("vId",model.IdClient);
            cmd.Parameters.AddWithValue("vNome",model.Nome);
            cmd.Parameters.AddWithValue("vEmail", model.Email);
            cmd.Parameters.AddWithValue("vSenha", model.Senha);
            cmd.Parameters.AddWithValue("vCPF",model.CPF);
            cmd.ExecuteNonQuery();

            TempData["ok"] = "Cliente Atualizado!";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Excluir(int id)
        {
            using var conn = db.GetConnection();
            try
            {
                using var cmd = new MySqlCommand("DeleteCliente", conn) { CommandType = System.Data.CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("vIdClient", id);
                cmd.ExecuteNonQuery();
                TempData["ok"] = "Autor Excluido!";
            }
            catch (MySqlException ex)
            {
                TempData["ok"]=ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
