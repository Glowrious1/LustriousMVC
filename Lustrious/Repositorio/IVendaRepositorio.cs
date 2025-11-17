using Lustrious.Models;
using System.Collections.Generic;

namespace Lustrious.Repositorio
{
    public interface IVendaRepositorio
    {
        void RegistrarVenda(Venda venda, IEnumerable<VendaProduto> itens);
        Venda AcharVenda(int id);
        IEnumerable<Venda> ListarVendasPorUsuario(int userId);
        int RegistrarEntrega(Entrega entrega);
    }
}
