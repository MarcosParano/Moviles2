using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Networking;
using Moviles2.Interfaces;
using Moviles2.Models;

namespace Moviles2.ViewModels
{
    public class ListaPostulantesViewModel : BindableObject
    {
        private readonly HttpClient _httpClient;
        private readonly ITramiteRepository _tramiteRepository;
        private string _mensajeEstado = "Cargando datos locales...";
        private bool _estaCargando;

        private List<TramiteVigilador> _todosLosPostulantes = new();
        public ObservableCollection<TramiteVigilador> Postulantes { get; } = new();

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

        public ListaPostulantesViewModel(HttpClient httpClient, ITramiteRepository tramiteRepository)
        {
            _httpClient = httpClient;
            _tramiteRepository = tramiteRepository;

            CargarDatosCommand = new Command(async () => await ObtenerPostulantesAsync());
            SeleccionarPostulanteCommand = new Command<TramiteVigilador>(async (p) => await IrADetalleAsync(p));

            FiltrarPresentadosCommand = new Command(() => AplicarFiltro("Presentado"));
            FiltrarEnTramiteCommand = new Command(() => AplicarFiltro("En Trámite"));
            MostrarTodosCommand = new Command(() => AplicarFiltro("Todos"));
        }

        // Se ejecuta automáticamente al entrar a la pantalla
        public async Task CargarLocalesAsync()
        {
            Postulantes.Clear();
            _todosLosPostulantes.Clear();

            try
            {
                var tramiteLocal = await _tramiteRepository.ObtenerTramiteAsync();

                if (tramiteLocal != null && !string.IsNullOrWhiteSpace(tramiteLocal.NombreSolicitante))
                {
                    _todosLosPostulantes.Add(tramiteLocal);
                    Postulantes.Add(tramiteLocal);
                    MensajeEstado = "Mostrando trámite local. Presioná 'Cargar Datos API' para sumar los de internet.";
                }
                else
                {
                    MensajeEstado = "No hay trámites locales guardados. Presioná 'Cargar Datos API'.";
                }
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error al leer SQLite: {ex.Message}";
            }
        }

        // Se ejecuta al tocar el botón "Cargar Datos API"
        private async Task ObtenerPostulantesAsync()
        {
            EstaCargando = true;

            // Recargamos locales primero para evitar duplicados si apretás el botón dos veces
            await CargarLocalesAsync();

            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                MensajeEstado = "Sin conexión a Internet. Mostrando solo datos locales.";
                EstaCargando = false;
                return;
            }

            MensajeEstado = "Acoplando datos desde la API...";

            try
            {
                var response = await _httpClient.GetAsync("https://jsonplaceholder.typicode.com/users");

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
                var listaApi = JsonSerializer.Deserialize<List<PostulanteApi>>(json, opciones);

                if (listaApi != null)
                {
                    var rnd = new Random();
                    foreach (var item in listaApi)
                    {
                        var tramite = new TramiteVigilador
                        {
                            NombreSolicitante = item.Name,
                            TieneDNI = true,
                            TieneBiometricos = rnd.Next(2) == 0,
                            TieneSecundario = rnd.Next(2) == 0,
                            TieneCurso = rnd.Next(2) == 0,
                            TieneReincidencia = rnd.Next(2) == 0
                        };

                        if (tramite.RequisitosCompletos())
                        {
                            tramite.EstaPresentado = true;
                            tramite.AprobarHabilitacion();
                        }

                        _todosLosPostulantes.Add(tramite);
                        Postulantes.Add(tramite);
                    }

                    MensajeEstado = $"Éxito: {_todosLosPostulantes.Count} trámites totales (Local + API).";
                }
            }
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
                bool coincide = estado == "Todos" ||
                                (estado == "Presentado" && p.EstaPresentado) ||
                                (estado == "En Trámite" && !p.EstaPresentado);

                if (coincide)
                {
                    Postulantes.Add(p);
                }
            }
            MensajeEstado = $"Mostrando filtro: {estado} ({Postulantes.Count} resultados)";
        }

        private async Task IrADetalleAsync(TramiteVigilador postulanteSeleccionado)
        {
            if (postulanteSeleccionado == null) return;
            var parametros = new Dictionary<string, object> { { "PostulanteElegido", postulanteSeleccionado } };
            await Shell.Current.GoToAsync("DetalleApiPage", parametros);
        }
    }
}