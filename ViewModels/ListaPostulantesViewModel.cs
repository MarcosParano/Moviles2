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
        private string _mensajeEstado = "Presiona 'Cargar Datos API'.";
        private bool _estaCargando;

        private List<PostulanteApi> _todosLosPostulantes = new();

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

        public ICommand FiltrarPresentadosCommand { get; }
        public ICommand FiltrarEnTramiteCommand { get; }
        public ICommand MostrarTodosCommand { get; }

        public ListaPostulantesViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            CargarDatosCommand = new Command(async () => await ObtenerPostulantesAsync());
            SeleccionarPostulanteCommand = new Command<PostulanteApi>(async (p) => await IrADetalleAsync(p));

            FiltrarPresentadosCommand = new Command(() => AplicarFiltro("Presentado"));
            FiltrarEnTramiteCommand = new Command(() => AplicarFiltro("En Trámite"));
            MostrarTodosCommand = new Command(() => AplicarFiltro("Todos"));
        }

        private async Task ObtenerPostulantesAsync()
        {
            // Mantenemos la validación básica inicial
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                MensajeEstado = "Error: Sin conexión a Internet.";
                return;
            }

            EstaCargando = true;
            MensajeEstado = "Descargando datos...";
            Postulantes.Clear();
            _todosLosPostulantes.Clear();

            try
            {
                var response = await _httpClient.GetAsync("https://jsonplaceholder.typicode.com/users");

                // Corrección del profesor: Diferenciar códigos HTTP específicos
                if (!response.IsSuccessStatusCode)
                {
                    MensajeEstado = response.StatusCode switch
                    {
                        System.Net.HttpStatusCode.NotFound => "Error 404: Recurso no encontrado.",
                        System.Net.HttpStatusCode.InternalServerError => "Error 500: Fallo del servidor.",
                        System.Net.HttpStatusCode.ServiceUnavailable => "Error 503: Servidor en mantenimiento.",
                        _ => $"Error HTTP {(int)response.StatusCode}"
                    };
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var lista = JsonSerializer.Deserialize<List<PostulanteApi>>(json, opciones);

                if (lista != null)
                {
                    var rnd = new Random();
                    foreach (var item in lista)
                    {
                        item.EstadoTramite = rnd.Next(2) == 0 ? "Presentado" : "En Trámite";
                        _todosLosPostulantes.Add(item);
                        Postulantes.Add(item);
                    }

                    MensajeEstado = $"Éxito: {lista.Count} postulantes cargados.";
                }
            }
            // Corrección del profesor: Manejo de excepciones granulares
            catch (TaskCanceledException)
            {
                MensajeEstado = "Timeout: El servidor tardó demasiado en responder.";
            }
            catch (HttpRequestException ex)
            {
                MensajeEstado = $"Error de red: {ex.Message}";
            }
            catch (JsonException)
            {
                MensajeEstado = "Error de formato en los datos recibidos.";
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error inesperado: {ex.Message}";
            }
            finally
            {
                EstaCargando = false;
            }
        }

        private void AplicarFiltro(string estado)
        {
            Postulantes.Clear();
            foreach (var p in _todosLosPostulantes)
            {
                if (estado == "Todos" || p.EstadoTramite == estado)
                {
                    Postulantes.Add(p);
                }
            }
            MensajeEstado = $"Mostrando filtro: {estado} ({Postulantes.Count} resultados)";
        }

        private async Task IrADetalleAsync(PostulanteApi postulanteSeleccionado)
        {
            if (postulanteSeleccionado == null) return;
            var parametros = new Dictionary<string, object> { { "PostulanteElegido", postulanteSeleccionado } };
            await Shell.Current.GoToAsync("DetalleApiPage", parametros);
        }
    }
}