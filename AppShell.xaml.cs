using Microsoft.Maui.Controls;
using Moviles2.Views;

namespace Moviles2
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            
            Routing.RegisterRoute(nameof(ProfileDetailsPage), typeof(ProfileDetailsPage));
        }
    }
}