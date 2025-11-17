using System.Threading.Tasks;

namespace Lustrious.Services
{
 public interface IFreteService
 {
 Task<decimal> CalcularFreteAsync(int enderecoClienteId);
 }
}
