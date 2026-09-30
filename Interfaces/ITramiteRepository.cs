using System.Threading.Tasks;
using Moviles2.Models;

namespace Moviles2.Interfaces
{
    public interface ITramiteRepository
    {
        Task<TramiteVigilador> ObtenerTramiteAsync();
        Task GuardarTramiteAsync(TramiteVigilador tramite);
    }
}