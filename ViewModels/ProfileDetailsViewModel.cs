using Moviles2.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Moviles2.ViewModels
{
    
    [QueryProperty(nameof(UsuarioRecibido), "MiUsuario")]
    public class ProfileDetailsViewModel : INotifyPropertyChanged
    {
        private UserProfile _usuarioRecibido;
        private string _mensajeValidacion;
        private Color _colorValidacion;

        public UserProfile UsuarioRecibido
        {
            get => _usuarioRecibido;
            set
            {
                _usuarioRecibido = value;
                OnPropertyChanged();

                
                ValidarParametroRecibido();
            }
        }

        public string MensajeValidacion
        {
            get => _mensajeValidacion;
            set { _mensajeValidacion = value; OnPropertyChanged(); }
        }

        public Color ColorValidacion
        {
            get => _colorValidacion;
            set { _colorValidacion = value; OnPropertyChanged(); }
        }

        private void ValidarParametroRecibido()
        {
            // Documentación: Validamos que el objeto haya llegado correctamente
            if (_usuarioRecibido == null || string.IsNullOrWhiteSpace(_usuarioRecibido.Nombre))
            {
                MensajeValidacion = "Error crítico: Los datos del perfil no se transfirieron correctamente.";
                ColorValidacion = Colors.Red;
            }
            else
            {
                MensajeValidacion = "Datos recibidos y validados con éxito vía Shell Routing.";
                ColorValidacion = Colors.LightGreen;
            }
        }

        // Comando para volver atrás (Pop en el stack de navegación)
        public Command VolverCommand => new Command(async () => await Shell.Current.GoToAsync(".."));

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}