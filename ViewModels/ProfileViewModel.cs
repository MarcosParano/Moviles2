using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Moviles2.Models;

namespace Moviles2.ViewModels
{

    public class ProfileViewModel : INotifyPropertyChanged
    {
        private UserProfile _userProfile;

        private string _statusMessage = string.Empty;
        private Color _statusColor = Colors.Transparent;

      
        public ProfileViewModel()
        {
            // Inicializamos el modelo con datos por defecto
            _userProfile = new UserProfile
            {
                Nombre = "Marcos Parano",
                Edad = 30,
                Descripcion = "Estudiante de Sistemas.",
                ImagenPerfil = "https://static.wikia.nocookie.net/esstarwars/images/5/58/BobaFettMain2.jpg/revision/latest?cb=20120126225714"
            };

            // Inicializamos el comando que ejecuta el botón
            SaveCommand = new Command(ExecuteSave);
        }

        public string Nombre
        {
            get => _userProfile.Nombre;
            set
            {
                if (_userProfile.Nombre != value)
                {
                    _userProfile.Nombre = value;
                    OnPropertyChanged(); // Avisa a la pantalla que el nombre cambió
                }
            }
        }

        public int Edad
        {
            get => _userProfile.Edad;
            set
            {
                if (_userProfile.Edad != value)
                {
                    _userProfile.Edad = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Descripcion
        {
            get => _userProfile.Descripcion;
            set
            {
                if (_userProfile.Descripcion != value)
                {
                    _userProfile.Descripcion = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ImagenPerfil
        {
            get => _userProfile.ImagenPerfil;
            set
            {
                if (_userProfile.ImagenPerfil != value)
                {
                    _userProfile.ImagenPerfil = value;
                    OnPropertyChanged();
                }
            }
        }

        // Propiedades de estado para los mensajes de error/éxito
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public Color StatusColor
        {
            get => _statusColor;
            set { _statusColor = value; OnPropertyChanged(); }
        }

       
        public Command SaveCommand { get; }

       
        private void ExecuteSave()
        {
            // Validamos que no esté vacío
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                StatusColor = Colors.Red;
                StatusMessage = "Error: El nombre no puede estar vacío.";
                return;
            }

            // Validamos una edad lógica
            if (Edad <= 0)
            {
                StatusColor = Colors.Red;
                StatusMessage = "Error: Ingresa una edad válida.";
                return;
            }

            // Si pasa todo, mostramos mensaje de éxito
            StatusColor = Colors.Green;
            StatusMessage = $"¡Perfil de {Nombre} actualizado correctamente!";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}