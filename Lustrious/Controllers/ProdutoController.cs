using Lustrious.Data;
using Microsoft.AspNetCore.Mvc;
using Lustrious.Models;
using MySql.Data.MySqlClient;


namespace Lustrious.Controllers
{
    public class ProdutoController : Controller
    {
        public readonly DataBase db = new DataBase();

        public IActionResult Index()
        {
            var lista = new List<Produto>();
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("selectProduto", conn) { CommandType = System.Data.CommandType.StoredProcedure };
            using var rd = cmd .ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Produto
                {
                    CodigoBarras = rd.GetInt32("CodigoBarras"),
                    NomeProd = rd.GetString("NomeProd"),
                    qtd = rd.GetInt32("qtd"),
                    Descricao = rd.GetString("Descricao"),
                    ValorUnitario = rd.GetDecimal("ValorUnitario")
                });
            }
            
            return View(lista);
        }
        public IActionResult CriarProduto()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CriarProduto(Produto produto)
        {
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand("insertProduto", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("vNomeProd", produto.NomeProd);
            cmd.Parameters.AddWithValue("vqtd", produto.qtd);
            cmd.Parameters.AddWithValue("vDescricao", produto.Descricao);
            cmd.Parameters.AddWithValue("vValorUnitario", produto.ValorUnitario);
            cmd.ExecuteNonQuery();
            TempData["Ok"] = "Produto Cadastrado!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult EditarProduto(int id)
        {
            using var conn = db.GetConnection();
            Produto? produto = null;
            using (var cmd = new MySqlCommand("obterProduto", conn)
            { CommandType = System.Data.CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("vId", id);
                using var rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    produto = new Produto
                    {
                        CodigoBarras = rd.GetInt32("CodigoBarras"),
                        NomeProd = rd.GetString("NomeProd"),
                        qtd = rd.GetInt32("qtd"),
                        Descricao = rd.GetString("Descricao"),
                        ValorUnitario = rd.GetDecimal("ValorUnitario")
                    };
                }
            }
            return View(produto);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult EditarProduto(Produto model)
        {
            if (model.CodigoBarras <= 0) return NotFound();
            if (string.IsNullOrWhiteSpace(model.NomeProd))
            {
                ModelState.AddModelError("", "Informe seu nome!");
            }
            using var conn2 = db.GetConnection();
            using var cmd = new MySqlCommand("updateProduto", conn2)
            { CommandType = System.Data.CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("vCodigoBarra", model.CodigoBarras);
            cmd.Parameters.AddWithValue("vNomeProd", model.NomeProd);
            cmd.Parameters.AddWithValue("vqtd", model.qtd);
            cmd.Parameters.AddWithValue("vDescricao", model.Descricao);
            cmd.Parameters.AddWithValue("vValorUnitario", model.ValorUnitario);
            cmd.ExecuteNonQuery();

            TempData["Ok"] = "Produto atualizado";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ExcluirProduto(int id)
        {
            using var conn = db.GetConnection();
            try
            {
                using var cmd = new MySqlCommand("deleteProduto", conn) { CommandType = System.Data.CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("vCodigoBarras", id);
                cmd.ExecuteNonQuery();
                TempData["Ok"] = "Produto excluido!";
            }
            catch (MySqlException ex)
            {
                TempData["Ok"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
