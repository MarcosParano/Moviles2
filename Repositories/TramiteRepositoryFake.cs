using System.Threading.Tasks;
using Moviles2.Models;

namespace Moviles2.Repositories
{
    public class TramiteRepositoryFake
    {
        private TramiteVigilador _tramiteEnMemoria;

        public TramiteRepositoryFake()
        {
            _tramiteEnMemoria = new TramiteVigilador
            {
                NombreSolicitante = "Marcos Parano",
                TieneDNI = true,
                TieneBiometricos = false,
                TieneSecundario = true,
                TieneCurso = false,
                TieneReincidencia = false,
                EstaPresentado = false,
                EstaHabilitado = false
            };
        }

        public Task<TramiteVigilador> ObtenerTramiteAsync()
        {
            return Task.FromResult(_tramiteEnMemoria);
        }

        public Task GuardarTramiteAsync(TramiteVigilador tramite)
        {
            _tramiteEnMemoria = tramite;
            return Task.CompletedTask;
        }
    }
}
