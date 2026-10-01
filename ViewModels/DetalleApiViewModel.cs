using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Moviles2.Models;

namespace Moviles2.ViewModels
{
    public class DetalleApiViewModel : BindableObject, IQueryAttributable
    {
        private PostulanteApi? _postulante;

        public PostulanteApi? Postulante
        {
            get => _postulante;
            set { _postulante = value; OnPropertyChanged(); }
        }

        public ICommand VolverCommand { get; }

        public DetalleApiViewModel()
        {
            VolverCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("PostulanteElegido"))
            {
                Postulante = query["PostulanteElegido"] as PostulanteApi;
            }
        }
    }
}