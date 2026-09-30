using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Moviles2.Models;
using Moviles2.Repositories;
using Moviles2.Interfaces;

namespace Moviles2.ViewModels
{
    public class TramiteViewModel : BindableObject
    {
        private readonly ITramiteRepository _repository;
        private TramiteVigilador? _tramite;

        public TramiteViewModel(ITramiteRepository repository)
        {
            _repository = repository;

            LoadCommand = new Command(async () => await CargarTramiteAsync());
            GuardarCommand = new Command(async () => await GuardarAsync());
            PresentarCommand = new Command(Presentar, () => _tramite?.RequisitosCompletos() == true && _tramite?.EstaPresentado == false);
            NuevoTramiteCommand = new Command(async () => await ReiniciarTramiteAsync());

            LoadCommand.Execute(null);
        }

        public string NombreSolicitante
        {
            get => _tramite?.NombreSolicitante ?? string.Empty;
            set { if (_tramite != null) { _tramite.NombreSolicitante = value ?? string.Empty; OnPropertyChanged(nameof(NombreSolicitante)); } }
        }

        public bool TieneDNI
        {
            get => _tramite?.TieneDNI ?? false;
            set { if (_tramite != null) { _tramite.TieneDNI = value; NotificarCambios(nameof(TieneDNI)); } }
        }

        public bool TieneBiometricos
        {
            get => _tramite?.TieneBiometricos ?? false;
            set { if (_tramite != null) { _tramite.TieneBiometricos = value; NotificarCambios(nameof(TieneBiometricos)); } }
        }

        public DateTime FechaBiometricos
        {
            get => _tramite?.FechaBiometricos ?? DateTime.Today;
            set { if (_tramite != null) { _tramite.FechaBiometricos = value; OnPropertyChanged(nameof(FechaBiometricos)); } }
        }

        public bool TieneSecundario
        {
            get => _tramite?.TieneSecundario ?? false;
            set { if (_tramite != null) { _tramite.TieneSecundario = value; NotificarCambios(nameof(TieneSecundario)); } }
        }

        public bool TieneCurso
        {
            get => _tramite?.TieneCurso ?? false;
            set { if (_tramite != null) { _tramite.TieneCurso = value; NotificarCambios(nameof(TieneCurso)); } }
        }

        public DateTime FechaCurso
        {
            get => _tramite?.FechaCurso ?? DateTime.Today;
            set { if (_tramite != null) { _tramite.FechaCurso = value; OnPropertyChanged(nameof(FechaCurso)); } }
        }

        public bool TieneReincidencia
        {
            get => _tramite?.TieneReincidencia ?? false;
            set { if (_tramite != null) { _tramite.TieneReincidencia = value; NotificarCambios(nameof(TieneReincidencia)); } }
        }

        public DateTime FechaReincidencia
        {
            get => _tramite?.FechaReincidencia ?? DateTime.Today;
            set { if (_tramite != null) { _tramite.FechaReincidencia = value; OnPropertyChanged(nameof(FechaReincidencia)); } }
        }

        public string EstadoGeneral
        {
            get
            {
                if (_tramite == null) return "Cargando...";
                if (_tramite.EstaHabilitado) return $"Habilitado hasta: {_tramite.FechaVencimiento?.ToString("dd/MM/yyyy")}";

                var faltantes = new List<string>();
                if (!_tramite.TieneDNI) faltantes.Add("- DNI Vigente");
                if (!_tramite.TieneBiometricos) faltantes.Add("- Datos Biométricos");
                if (!_tramite.TieneSecundario) faltantes.Add("- Título Secundario");
                if (!_tramite.TieneCurso) faltantes.Add("- Curso de Capacitación");
                if (!_tramite.TieneReincidencia) faltantes.Add("- Certificado de Reincidencia");

                if (faltantes.Count == 0) return "¡Todos los requisitos completos!\nListo para presentar.";

                return "En proceso. Faltan los siguientes requisitos:\n" + string.Join("\n", faltantes);
            }
        }

        public ICommand LoadCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand PresentarCommand { get; }
        public ICommand NuevoTramiteCommand { get; }

        private void NotificarCambios(string nombrePropiedad)
        {
            OnPropertyChanged(nombrePropiedad);
            OnPropertyChanged(nameof(EstadoGeneral));
            ((Command)PresentarCommand).ChangeCanExecute();
        }

        private async Task CargarTramiteAsync()
        {
            _tramite = await _repository.ObtenerTramiteAsync();

            OnPropertyChanged(nameof(NombreSolicitante));
            OnPropertyChanged(nameof(TieneDNI));
            OnPropertyChanged(nameof(TieneBiometricos));
            OnPropertyChanged(nameof(FechaBiometricos));
            OnPropertyChanged(nameof(TieneSecundario));
            OnPropertyChanged(nameof(TieneCurso));
            OnPropertyChanged(nameof(FechaCurso));
            OnPropertyChanged(nameof(TieneReincidencia));
            OnPropertyChanged(nameof(FechaReincidencia));
            OnPropertyChanged(nameof(EstadoGeneral));
            ((Command)PresentarCommand).ChangeCanExecute();
        }

        private async Task GuardarAsync()
        {
            if (_tramite != null)
            {
                await _repository.GuardarTramiteAsync(_tramite);
                if (Shell.Current != null) await Shell.Current.DisplayAlertAsync("Guardado", "Avance registrado.", "OK");
            }
        }

        private async void Presentar()
        {
            if (_tramite != null)
            {
                _tramite.EstaPresentado = true;
                _tramite.AprobarHabilitacion();
                OnPropertyChanged(nameof(EstadoGeneral));
                ((Command)PresentarCommand).ChangeCanExecute();

                await GuardarAsync();

                var parametros = new Dictionary<string, object>
                {
                    { "MiTramite", _tramite }
                };
                await Shell.Current.GoToAsync("ResumenTramitePage", parametros);
            }
        }

        private async Task ReiniciarTramiteAsync()
        {
            if (_tramite != null)
            {
                _tramite.NombreSolicitante = string.Empty;
                _tramite.TieneDNI = false;
                _tramite.TieneBiometricos = false;
                _tramite.TieneSecundario = false;
                _tramite.TieneCurso = false;
                _tramite.TieneReincidencia = false;
                _tramite.FechaBiometricos = DateTime.Today;
                _tramite.FechaCurso = DateTime.Today;
                _tramite.FechaReincidencia = DateTime.Today;
                _tramite.EstaPresentado = false;
                _tramite.EstaHabilitado = false;
                _tramite.FechaVencimiento = null;

                await _repository.GuardarTramiteAsync(_tramite);
                await CargarTramiteAsync();
            }
        }
    }
}