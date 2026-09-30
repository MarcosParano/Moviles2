using Microsoft.Maui.Controls;
using Moviles2.ViewModels;

namespace Moviles2.Views
{
    public partial class ResumenTramitePage : ContentPage
    {
        public ResumenTramitePage(ResumenTramiteViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}