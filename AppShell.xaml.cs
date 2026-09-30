using Microsoft.Maui.Controls;
using Moviles2.Views;

namespace Moviles2
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registramos la ruta para la navegación por parámetros hacia el detalle
            Routing.RegisterRoute("ResumenTramitePage", typeof(ResumenTramitePage));
        }
    }
}