using Lustrious.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Lustrious.Repositorio
{
    public interface IProdutoRepostorio
    {
        public void CadastrarProduto(Produto produto, IFormFile? foto);
        public Produto AcharProduto(int id);
        // Ajustado para permitir filtro por tipo de produto; padrão0 retorna todos dependendo da procedure
        public IEnumerable<Produto> ListarProdutos(int codTipoProduto =0);
        public void AlterarProduto(Produto produto, IFormFile? foto);
        public void ExcluirProduto(int id);
    }
}
