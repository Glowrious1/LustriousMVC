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
        IEnumerable<Venda> ListarTodasVendas();
        void NotificarClienteVenda(int userId, string mensagem);
        IEnumerable<Notificacao> ListarNotificacoes(int userId);
        int ContarNotificacoesNaoLidas(int userId);
        IEnumerable<Notificacao> ListarUltimasNotificacoes(int userId, int max);
        void MarcarUltimasNotificacoesComoLidas(int userId, int max);
    }
}
