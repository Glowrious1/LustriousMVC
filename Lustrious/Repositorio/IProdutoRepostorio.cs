using Lustrious.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lustrious.Repositorio
{
    public interface IProdutoRepostorio
    {
        public void CadastrarProduto(Produto produto, IFormFile? foto);
        public Produto AcharProduto(long id);
        // Server-side filtering and pagination: returns items and total count
        public (IEnumerable<Produto> Items, int TotalCount) ListarProdutos(string? q = null, int codCategoria =0, int codTipoProduto =0, int page =1, int pageSize =10);
        public void AlterarProduto(Produto produto, IFormFile? foto);
        public void ExcluirProduto(long id);

        // Novos métodos para popular selects nas views
        public IEnumerable<SelectListItem> GetCategorias(int? selectedId = null);
        public IEnumerable<SelectListItem> GetTipos(int? selectedId = null, int? codCategoria = null);

        // Conveniência: retornar somente produtos (sem total) para views públicas
        public IEnumerable<Produto> ListarProdutosPublico(string? q = null, int codCategoria =0, int codTipoProduto =0, int page =1, int pageSize =12);
    }
}
