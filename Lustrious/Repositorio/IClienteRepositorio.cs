using Lustrious.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Lustrious.Repositorio
{
    public interface IClienteRepositorio
    {
        public void CadastrarCliente(Usuario cliente, IFormFile? foto);
        public Usuario AcharCliente(int id);
        // Server-side listing with pagination and optional search
        public (IEnumerable<Usuario> Items, int TotalCount) ListarClientes(string? q = null, int page =1, int pageSize =10);
        public void AlterarCliente(Usuario cliente, IFormFile? foto);
        public void ExcluirCliente(int id);
        public void ReativarCliente(int id);
    }
}
