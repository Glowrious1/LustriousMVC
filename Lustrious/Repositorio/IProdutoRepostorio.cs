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
        // Ajustado para permitir filtro por tipo de produto; padrão0 retorna todos dependendo da procedure
        public IEnumerable<Produto> ListarProdutos(int codTipoProduto =0);
        public void AlterarProduto(Produto produto, IFormFile? foto);
        public void ExcluirProduto(long id);

        // Novos métodos para popular selects nas views
        public IEnumerable<SelectListItem> GetCategorias(int? selectedId = null);
        public IEnumerable<SelectListItem> GetTipos(int? selectedId = null, int? codCategoria = null);
    }
}
