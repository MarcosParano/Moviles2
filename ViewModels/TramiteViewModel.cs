using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using Moviles2.Models;
using Moviles2.Services;

namespace Moviles2.ViewModels
{
    public class TramiteViewModel : BindableObject
    {
        private readonly TramiteService _tramiteService;
        private TramiteVigilador _tramite = new();
        private string _mensajeEstado = "Completá los datos y requisitos del postulante.";
        private string? _rutaFotoDni;

        public TramiteVigilador Tramite
        {
            get => _tramite;
            set
            {
                _tramite = value;
                OnPropertyChanged();
                RutaFotoDni = _tramite?.RutaFotoDni;
            }
        }

        public string MensajeEstado
        {
            get => _mensajeEstado;
            set { _mensajeEstado = value; OnPropertyChanged(); }
        }

        public string? RutaFotoDni
        {
            get => _rutaFotoDni;
            set { _rutaFotoDni = value; OnPropertyChanged(); }
        }

        public ICommand TomarFotoCommand { get; }
        public ICommand GuardarTramiteCommand { get; }
        public ICommand PresentarTramiteCommand { get; }
        public ICommand VerResumenCommand { get; }
        public ICommand MostrarPostulantesCommand { get; }

        public TramiteViewModel(TramiteService tramiteService)
        {
            _tramiteService = tramiteService;

            TomarFotoCommand = new Command(async () => await TomarFotoDniAsync());
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
                    RutaFotoDni = guardado.RutaFotoDni;
                    MensajeEstado = $"Cargado desde SQLite: {Tramite.EstadoGeneral}";
                }
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error al leer SQLite: {ex.Message}";
            }
        }

        private async Task TomarFotoDniAsync()
        {
            try
            {
                // 1. Verificar soporte de cámara
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await Shell.Current.DisplayAlertAsync(
                        "Cámara no disponible",
                        "Este dispositivo no soporta la captura de fotos.",
                        "OK");
                    return;
                }

                // 2. Verificar y solicitar permiso de cámara en tiempo de ejecución
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Camera>();
                }

                if (status != PermissionStatus.Granted)
                {
                    await Shell.Current.DisplayAlertAsync(
                        "Permiso denegado",
                        "Necesitás habilitar el permiso de cámara para sacar la foto del DNI o perfil.",
                        "OK");
                    return;
                }

                // 3. Abrir la cámara y capturar la foto
                FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo == null)
                    return; // El usuario canceló

                // 4. Guardar la imagen en la carpeta interna de la app
                string nombreArchivo = $"dni_{DateTime.Now:yyyyMMddHHmmss}.jpg";
                string rutaLocal = Path.Combine(FileSystem.AppDataDirectory, nombreArchivo);

                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(rutaLocal);
                await sourceStream.CopyToAsync(localFileStream);

                // 5. Guardar la ruta en el modelo y actualizar la vista previa
                Tramite.RutaFotoDni = rutaLocal;
                RutaFotoDni = rutaLocal;
                MensajeEstado = "Foto del DNI/perfil capturada correctamente.";

                // 6. Vibración corta de confirmación
                if (Vibration.Default.IsSupported)
                {
                    Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(200));
                }

                await Shell.Current.DisplayAlertAsync(
                    "Foto guardada",
                    "La foto del DNI/perfil se cargó correctamente.",
                    "OK");
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error de cámara: {ex.Message}";
                await Shell.Current.DisplayAlertAsync(
                    "Error",
                    $"No se pudo capturar la foto: {ex.Message}",
                    "OK");
            }
        }

        private async Task GuardarTramiteAsync()
        {
            try
            {
                await _tramiteService.GuardarProgresoAsync(Tramite);
                OnPropertyChanged(nameof(Tramite));
                MensajeEstado = $"Guardado localmente ({Tramite.EstadoGeneral}).";

                if (Vibration.Default.IsSupported)
                    Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(150));

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

                if (Vibration.Default.IsSupported)
                    Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(300));

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