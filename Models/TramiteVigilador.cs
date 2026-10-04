using System;

namespace Moviles2.Models
{
    public class TramiteVigilador
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }
        public string NombreSolicitante { get; set; } = string.Empty;

        public bool TieneDNI { get; set; }
        public bool TieneBiometricos { get; set; }
        public bool TieneSecundario { get; set; }
        public bool TieneCurso { get; set; }
        public bool TieneReincidencia { get; set; }

        public DateTime FechaBiometricos { get; set; } = DateTime.Today;
        public DateTime FechaCurso { get; set; } = DateTime.Today;
        public DateTime FechaReincidencia { get; set; } = DateTime.Today;

        public bool EstaPresentado { get; set; }
        public bool EstaHabilitado { get; set; }
        public DateTime? FechaVencimiento { get; set; }

        public bool RequisitosCompletos()
        {
            return TieneDNI && TieneBiometricos && TieneSecundario && TieneCurso && TieneReincidencia;
        }

        public void AprobarHabilitacion()
        {
            EstaHabilitado = true;
            FechaVencimiento = DateTime.Now.AddYears(1);
        }
    }
}