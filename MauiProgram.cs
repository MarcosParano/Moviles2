using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Moviles2.ViewModels;
using Moviles2.Views;
using Moviles2.Repositories;
using Moviles2.Interfaces;

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

            // Registro de la Base de Datos usando la Interfaz (Patrón Repository)
            builder.Services.AddSingleton<ITramiteRepository, TramiteRepositorySQLite>();

            // Registro de ViewModels
            builder.Services.AddTransient<TramiteViewModel>();
            builder.Services.AddTransient<ResumenTramiteViewModel>();

            // Registro de Vistas (Pages)
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ResumenTramitePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}