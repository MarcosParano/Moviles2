using System;
using System.Threading.Tasks;
using Moviles2.Interfaces;
using Moviles2.Models;

namespace Moviles2.Services
{
    public class TramiteService
    {
        private readonly ITramiteRepository _repository;

        public TramiteService(ITramiteRepository repository)
        {
            _repository = repository;
        }

        public async Task<TramiteVigilador?> ObtenerTramiteGuardadoAsync()
        {
            return await _repository.ObtenerTramiteAsync();
        }

        public async Task GuardarProgresoAsync(TramiteVigilador tramite)
        {
            if (tramite == null)
                throw new ArgumentNullException(nameof(tramite));

            if (string.IsNullOrWhiteSpace(tramite.NombreSolicitante) ||
                tramite.NombreSolicitante.Trim().Length < 3)
            {
                throw new ArgumentException("El nombre del solicitante debe tener al menos 3 caracteres.");
            }

            // Si el trámite está incompleto, nunca puede quedar como presentado ni habilitado
            if (!tramite.RequisitosCompletos())
            {
                tramite.EstaPresentado = false;
                tramite.EstaHabilitado = false;
                tramite.FechaVencimiento = null;
            }

            await _repository.GuardarTramiteAsync(tramite);
        }

        public async Task<bool> PresentarTramiteAsync(TramiteVigilador tramite)
        {
            if (tramite == null)
                throw new ArgumentNullException(nameof(tramite));

            // Requisito obligatorio: nombre válido Y los 5 requisitos completos
            if (string.IsNullOrWhiteSpace(tramite.NombreSolicitante) ||
                tramite.NombreSolicitante.Trim().Length < 3 ||
                !tramite.RequisitosCompletos())
            {
                tramite.EstaPresentado = false;
                tramite.EstaHabilitado = false;
                tramite.FechaVencimiento = null;
                return false;
            }

            tramite.EstaPresentado = true;
            tramite.AprobarHabilitacion();

            await _repository.GuardarTramiteAsync(tramite);
            return true;
        }
    }
}