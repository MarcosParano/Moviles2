using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Moviles2.Models;
using Moviles2.Services;

namespace Moviles2.ViewModels
{
    public class TramiteViewModel : BindableObject
    {
        private readonly TramiteService _tramiteService;
        private TramiteVigilador _tramite = new();
        private string _mensajeEstado = "Completá los datos y requisitos del postulante.";

        public TramiteVigilador Tramite
        {
            get => _tramite;
            set { _tramite = value; OnPropertyChanged(); }
        }

        public string MensajeEstado
        {
            get => _mensajeEstado;
            set { _mensajeEstado = value; OnPropertyChanged(); }
        }

        public ICommand GuardarTramiteCommand { get; }
        public ICommand PresentarTramiteCommand { get; }
        public ICommand VerResumenCommand { get; }
        public ICommand MostrarPostulantesCommand { get; }

        public TramiteViewModel(TramiteService tramiteService)
        {
            _tramiteService = tramiteService;

            GuardarTramiteCommand = new Command(async () => await GuardarTramiteAsync());
            PresentarTramiteCommand = new Command(async () => await PresentarTramiteAsync());
            VerResumenCommand = new Command(async () => await IrAResumenAsync());
            MostrarPostulantesCommand = new Command(async () =>
                await Shell.Current.GoToAsync("///ListaPostulantesPage"));

            _ = CargarTramiteLocalAsync();
        }

        private async Task CargarTramiteLocalAsync()
        {
            try
            {
                var guardado = await _tramiteService.ObtenerTramiteGuardadoAsync();
                if (guardado != null)
                {
                    Tramite = guardado;
                    MensajeEstado = $"Cargado desde SQLite: {Tramite.EstadoGeneral}";
                }
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error al leer SQLite: {ex.Message}";
            }
        }

        private async Task GuardarTramiteAsync()
        {
            try
            {
                await _tramiteService.GuardarProgresoAsync(Tramite);
                OnPropertyChanged(nameof(Tramite));
                MensajeEstado = $"Guardado localmente ({Tramite.EstadoGeneral}).";
                await Shell.Current.DisplayAlertAsync("Guardado", "El progreso se guardó en la base local.", "OK");
            }
            catch (ArgumentException ex)
            {
                MensajeEstado = ex.Message;
                await Shell.Current.DisplayAlertAsync("Datos inválidos", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error al guardar: {ex.Message}";
            }
        }

        private async Task PresentarTramiteAsync()
        {
            try
            {
                bool presentado = await _tramiteService.PresentarTramiteAsync(Tramite);

                if (!presentado)
                {
                    MensajeEstado = "Faltan requisitos obligatorios o el nombre es inválido.";
                    await Shell.Current.DisplayAlertAsync("Incompleto", "Ingresá un nombre válido (mín. 3 letras) y marcá los 5 requisitos.", "OK");
                    return;
                }

                OnPropertyChanged(nameof(Tramite));
                MensajeEstado = $"¡Trámite presentado! ({Tramite.EstadoGeneral})";
                await Shell.Current.DisplayAlertAsync("Trámite Presentado", $"Habilitación aprobada para {Tramite.NombreSolicitante}.", "OK");
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error al presentar: {ex.Message}";
            }
        }

        private async Task IrAResumenAsync()
        {
            var parametros = new Dictionary<string, object>
            {
                { "TramiteActual", Tramite }
            };
            await Shell.Current.GoToAsync("ResumenTramitePage", parametros);
        }
    }
}