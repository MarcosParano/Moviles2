using Microsoft.Maui.Controls;
using Moviles2.ViewModels;

namespace Moviles2.Views
{
    public partial class ListaPostulantesPage : ContentPage
    {
        public ListaPostulantesPage(ListaPostulantesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}