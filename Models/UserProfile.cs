using System;
using System.Collections.Generic;
using System.Text;

namespace Moviles2.Models
{
    public class UserProfile
    {
        public string Nombre { get; set; } = string.Empty;
        public int Edad { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string ImagenPerfil { get; set; } = string.Empty;
    }
}
