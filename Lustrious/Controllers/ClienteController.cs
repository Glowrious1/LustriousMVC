using Microsoft.AspNetCore.Mvc;
using Lustrious.Data;
using Lustrious.Models;
using MySql.Data.MySqlClient;

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
                    CPF = rd.GetInt32("CPF"),
                    Senha = rd.GetString("Senha")
                });
            }
            return View(lista);
        }
    }
}
