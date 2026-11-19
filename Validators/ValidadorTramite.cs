using Moviles2.Models;

namespace Moviles2.Validators
{
    public static class ValidadorTramite
    {
        public static bool EsNombreValido(string? nombre)
            => !string.IsNullOrWhiteSpace(nombre) && nombre.Trim().Length >= 3;

        public static bool PuedePresentarse(TramiteVigilador? tramite)
            => tramite != null
               && EsNombreValido(tramite.NombreSolicitante)
               && tramite.RequisitosCompletos();
    }
}