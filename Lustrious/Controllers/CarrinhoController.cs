using Lustrious.Repositorio;
using Microsoft.AspNetCore.Mvc;

namespace Lustrious.Controllers
{
    public class CarrinhoController : Controller
    {
        private ICarrinhoRepositorio _carrinhoRepositorio;
        private IClienteRepositorio _clienteRepositorio;
        public CarrinhoController(ICarrinhoRepositorio carrinhoRepositorio, IClienteRepositorio clienteRepostorio)
        {
            _clienteRepositorio = clienteRepostorio;
            _carrinhoRepositorio = carrinhoRepositorio;
        }

        public IActionResult Index(int userId)
        {
            return View(_carrinhoRepositorio.AcharCarrinho(userId));
        }

        [HttpPost]
        public IActionResult AdicionarItem(int produtoId)
        {
            int userId = 1; //Depois a gente pega o user logado
            _carrinhoRepositorio.AdicionarItem(produtoId, userId);
            return RedirectToAction("Index");
        }

        [HttpDelete]
        public IActionResult RemoverItem(int produtoId)
        {
            int userId = 1;
            _carrinhoRepositorio.RemoverItem(produtoId, userId);
            return RedirectToAction("Index");
        }

        [HttpDelete]
        public IActionResult RemoverTodosItens()
        {
            int userId = 1; //Depois a gente pega o user logado
            _carrinhoRepositorio.LimparCarrinho(userId);
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Checkout(int idEnd)
        {
            //Depois a gente faz algo legal usando a autenticação do usuário para pegar ele no banco
            int userId = 1;
            _carrinhoRepositorio.FinalizarCompra(idEnd, userId);
            return View();
        }
    }
}
