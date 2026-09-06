using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Moviles2.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Moviles2.ViewModels
{
    /* 
     * DIAGRAMA DE FLUJO DE NAVEGACIÓN
     * 
     * sequenceDiagram
     *     participant UI as MainPage.xaml
     *     participant VM as ProfileViewModel
     *     participant Shell as AppShell
     *     participant DestVM as ProfileDetailsViewModel
     *     
     *     UI->>VM: Usuario presiona "Guardar" (SaveCommand)
     *     VM->>VM: Validación de campos locales
     *     VM->>UI: Mostrar Snackbar de éxito
     *     VM->>Shell: Shell.Current.GoToAsync("ProfileDetailsPage", Parametros)
     *     Shell->>DestVM: Pasa el diccionario con "MiUsuario"
     *     DestVM->>DestVM: [QueryProperty] asigna el modelo y lo valida
     */
    public class ProfileViewModel : INotifyPropertyChanged
    {
        private UserProfile _userProfile;
        private string _statusMessage = string.Empty;
        private Color _statusColor = Colors.Transparent;

        public ProfileViewModel()
        {
            _userProfile = new UserProfile
            {
                Nombre = "Marcos Parano",
                Edad = 30,
                Descripcion = "Estudiante de Sistemas.",
                ImagenPerfil = "https://static.wikia.nocookie.net/esstarwars/images/5/58/BobaFettMain2.jpg/revision/latest?cb=20120126225714"
            };

            SaveCommand = new Command(async () => await ExecuteSaveAsync());
        }

        public string Nombre
        {
            get => _userProfile.Nombre;
            set { if (_userProfile.Nombre != value) { _userProfile.Nombre = value; OnPropertyChanged(); } }
        }

        public int Edad
        {
            get => _userProfile.Edad;
            set { if (_userProfile.Edad != value) { _userProfile.Edad = value; OnPropertyChanged(); } }
        }

        public string Descripcion
        {
            get => _userProfile.Descripcion;
            set { if (_userProfile.Descripcion != value) { _userProfile.Descripcion = value; OnPropertyChanged(); } }
        }

        public string ImagenPerfil
        {
            get => _userProfile.ImagenPerfil;
            set { if (_userProfile.ImagenPerfil != value) { _userProfile.ImagenPerfil = value; OnPropertyChanged(); } }
        }

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

        private async Task ExecuteSaveAsync()
        {
            // 1. Validaciones locales
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                StatusColor = Colors.Red;
                StatusMessage = "Error: El nombre no puede estar vacío.";
                return;
            }

            if (Edad <= 0)
            {
                StatusColor = Colors.Red;
                StatusMessage = "Error: Ingresa una edad válida.";
                return;
            }

            StatusColor = Colors.Green;
            StatusMessage = "Preparando navegación...";

            // 2. Notificación Visual (Snackbar adaptado para evitar crash en Windows)
            try
            {
                var snackbar = Snackbar.Make($"Guardando perfil de {Nombre}...", null, "OK", TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
            catch (Exception)
            {
                // Silencio la excepción COM en Windows para permitir que continúe la navegación
            }

            // 3. Preparación de Parámetros (Diccionario)
            var navigationParameter = new Dictionary<string, object>
            {
                { "MiUsuario", _userProfile }
            };

            // 4. Navegación MVVM Centralizada
            await Shell.Current.GoToAsync(nameof(Views.ProfileDetailsPage), navigationParameter);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}