using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Networking;
using Moviles2.Models;
using System.Collections.Generic;

namespace Moviles2.ViewModels
{
    public class ListaPostulantesViewModel : BindableObject
    {
        private readonly HttpClient _httpClient;
        private string _mensajeEstado = "Presiona 'Cargar Datos API' para obtener postulantes.";
        private bool _estaCargando;

        public ObservableCollection<PostulanteApi> Postulantes { get; } = new();

        public string MensajeEstado
        {
            get => _mensajeEstado;
            set { _mensajeEstado = value; OnPropertyChanged(); }
        }

        public bool EstaCargando
        {
            get => _estaCargando;
            set { _estaCargando = value; OnPropertyChanged(); }
        }

        public ICommand CargarDatosCommand { get; }
        public ICommand SeleccionarPostulanteCommand { get; }

        public ListaPostulantesViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            CargarDatosCommand = new Command(async () => await ObtenerPostulantesAsync());
            SeleccionarPostulanteCommand = new Command<PostulanteApi>(async (p) => await IrADetalleAsync(p));
        }

        private async Task ObtenerPostulantesAsync()
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                MensajeEstado = "Error: Sin conexión a Internet. Verificá tu red.";
                return;
            }

            EstaCargando = true;
            MensajeEstado = "Descargando datos desde la API...";
            Postulantes.Clear();

            try
            {
                var response = await _httpClient.GetAsync("https://jsonplaceholder.typicode.com/users");

                if (!response.IsSuccessStatusCode)
                {
                    MensajeEstado = $"Error HTTP: El servidor respondió con código {response.StatusCode}";
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var lista = JsonSerializer.Deserialize<List<PostulanteApi>>(json, opciones);

                if (lista != null)
                {
                    foreach (var item in lista)
                        Postulantes.Add(item);

                    MensajeEstado = $"Éxito: Se cargaron {lista.Count} postulantes desde internet.";
                }
            }
            catch (HttpRequestException ex)
            {
                MensajeEstado = "Error de red: No se pudo conectar al servidor.";
                Console.WriteLine(ex.Message);
            }
            catch (JsonException ex)
            {
                MensajeEstado = "Error de formato: Los datos recibidos no son válidos.";
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                MensajeEstado = "Error inesperado en la aplicación.";
                Console.WriteLine(ex.Message);
            }
            finally
            {
                EstaCargando = false;
            }
        }

        private async Task IrADetalleAsync(PostulanteApi postulanteSeleccionado)
        {
            if (postulanteSeleccionado == null) return;

            var parametros = new Dictionary<string, object>
            {
                { "PostulanteElegido", postulanteSeleccionado }
            };

            await Shell.Current.GoToAsync("DetalleApiPage", parametros);
        }
    }
}