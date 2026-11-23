using System;
using SQLite;

namespace Moviles2.Models
{
    public class TramiteVigilador
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string NombreSolicitante { get; set; } = string.Empty;

        public bool TieneDNI { get; set; }

        public bool TieneBiometricos { get; set; }
        public DateTime FechaBiometricos { get; set; } = DateTime.Today;

        public bool TieneSecundario { get; set; }

        public bool TieneCurso { get; set; }
        public DateTime FechaCurso { get; set; } = DateTime.Today;

        public bool TieneReincidencia { get; set; }
        public DateTime FechaReincidencia { get; set; } = DateTime.Today;

        public bool EstaPresentado { get; set; }
        public bool EstaHabilitado { get; set; }
        public DateTime? FechaVencimiento { get; set; }

        // Propiedad nueva para la foto del DNI / perfil
        public string? RutaFotoDni { get; set; }

        public bool RequisitosCompletos()
        {
            return TieneDNI &&
                   TieneBiometricos &&
                   TieneSecundario &&
                   TieneCurso &&
                   TieneReincidencia;
        }

        public void AprobarHabilitacion()
        {
            if (RequisitosCompletos() && EstaPresentado)
            {
                EstaHabilitado = true;
                FechaVencimiento = DateTime.Now.AddYears(1);
            }
        }

        [Ignore]
        public string EstadoGeneral
        {
            get
            {
                if (EstaHabilitado)
                    return "Presentado - Habilitado";
                if (EstaPresentado)
                    return "Presentado - En revisión";

                return "En Trámite - Faltan requisitos";
            }
        }
    }
}