using Microsoft.Maui.Controls;
using Moviles2.ViewModels;

namespace Moviles2.Views
{
    public partial class MainPage : ContentPage
    {
        
        public MainPage(ProfileViewModel viewModel)
        {
            InitializeComponent();

            
            BindingContext = viewModel;
        }
    }
}