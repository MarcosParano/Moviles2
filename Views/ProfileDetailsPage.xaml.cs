using Microsoft.Maui.Controls;
using Moviles2.ViewModels;

namespace Moviles2.Views
{
    public partial class ProfileDetailsPage : ContentPage
    {
        
        public ProfileDetailsPage(ProfileDetailsViewModel viewModel)
        {
            InitializeComponent();

            
            BindingContext = viewModel;
        }
    }
}