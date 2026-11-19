using Microsoft.Maui.Controls;
using Moviles2.ViewModels;

namespace Moviles2.Views
{
    public partial class ListaPostulantesPage : ContentPage
    {
        private readonly ListaPostulantesViewModel _viewModel;

        public ListaPostulantesPage(ListaPostulantesViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarLocalesAsync();
        }
    }
}