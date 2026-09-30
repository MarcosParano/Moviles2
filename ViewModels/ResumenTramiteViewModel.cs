using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Moviles2.Models;

namespace Moviles2.ViewModels
{
    // Usamos IQueryAttributable, que es 100% seguro para recibir parámetros
    public class ResumenTramiteViewModel : BindableObject, IQueryAttributable
    {
        private TramiteVigilador? _tramiteRecibido;

        public TramiteVigilador? TramiteRecibido
        {
            get => _tramiteRecibido;
            set
            {
                _tramiteRecibido = value;
                OnPropertyChanged(nameof(TramiteRecibido));
                OnPropertyChanged(nameof(NombrePostulante));
                OnPropertyChanged(nameof(FechaVencimientoStr));
            }
        }

        public string NombrePostulante => TramiteRecibido?.NombreSolicitante ?? "Buscando datos...";
        public string FechaVencimientoStr => TramiteRecibido?.FechaVencimiento?.ToString("dd/MM/yyyy") ?? "Calculando...";

        public ICommand VolverCommand { get; }

        public ResumenTramiteViewModel()
        {
            VolverCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("MiTramite"))
            {
                TramiteRecibido = query["MiTramite"] as TramiteVigilador;
            }
        }
    }
}