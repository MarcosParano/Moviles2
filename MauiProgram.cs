using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Moviles2.ViewModels; 
using Moviles2.Views;     

namespace Moviles2
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            
            // Registramos los ViewModels
            builder.Services.AddSingleton<ProfileViewModel>();
            builder.Services.AddTransient<ProfileDetailsViewModel>(); // Usamos Transient para que la vista de detalles se recargue limpia cada vez

            // Registramos las Vistas 
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<ProfileDetailsPage>();

            return builder.Build();
        }
    }
}
