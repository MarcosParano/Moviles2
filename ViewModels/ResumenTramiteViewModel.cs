using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Moviles2.Models;

namespace Moviles2.ViewModels
{
    public class ResumenTramiteViewModel : BindableObject, IQueryAttributable
    {
        private TramiteVigilador _tramite = new();

        public TramiteVigilador Tramite
        {
            get => _tramite;
            set
            {
                _tramite = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TextoVencimiento));
            }
        }

        public string TextoVencimiento =>
            Tramite.FechaVencimiento.HasValue
                ? Tramite.FechaVencimiento.Value.ToString("dd/MM/yyyy")
                : "Pendiente de aprobación";

        public ICommand VolverCommand { get; }

        public ResumenTramiteViewModel()
        {
            VolverCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("TramiteActual") && query["TramiteActual"] is TramiteVigilador recibido)
            {
                Tramite = recibido;
            }
        }
    }
}