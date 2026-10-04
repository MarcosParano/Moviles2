using Microsoft.Maui.Controls;
using Moviles2.Views;

namespace Moviles2
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("ResumenTramitePage", typeof(ResumenTramitePage));
            Routing.RegisterRoute("DetalleApiPage", typeof(DetalleApiPage));
        }
    }
}