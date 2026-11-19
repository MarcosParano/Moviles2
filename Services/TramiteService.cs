using System;
using System.Threading.Tasks;
using Moviles2.Interfaces;
using Moviles2.Models;
using Moviles2.Validators;

namespace Moviles2.Services
{
    public class TramiteService
    {
        private readonly ITramiteRepository _repository;

        public TramiteService(ITramiteRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> GuardarProgresoAsync(TramiteVigilador tramite)
        {
            if (tramite == null || !ValidadorTramite.EsNombreValido(tramite.NombreSolicitante))
                throw new ArgumentException("El nombre del solicitante debe tener al menos 3 caracteres.");

            await _repository.GuardarTramiteAsync(tramite);
            return true;
        }

        public async Task<bool> PresentarTramiteAsync(TramiteVigilador tramite)
        {
            if (!ValidadorTramite.PuedePresentarse(tramite))
                return false;

            tramite.EstaPresentado = true;
            tramite.AprobarHabilitacion();
            await _repository.GuardarTramiteAsync(tramite);
            return true;
        }

        public async Task<TramiteVigilador?> ObtenerTramiteGuardadoAsync()
        {
            return await _repository.ObtenerTramiteAsync();
        }
    }
}