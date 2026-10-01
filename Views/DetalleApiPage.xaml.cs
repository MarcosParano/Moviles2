using Microsoft.Maui.Controls;
using Moviles2.ViewModels;

namespace Moviles2.Views
{
    public partial class DetalleApiPage : ContentPage
    {
        public DetalleApiPage(DetalleApiViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}