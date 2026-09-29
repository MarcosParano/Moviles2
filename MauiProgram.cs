using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Moviles2.ViewModels;
using Moviles2.Views;
using Moviles2.Repositories;

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

            // 1. Registro de la Capa de Datos
            builder.Services.AddSingleton<TramiteRepositorySQLite>(); ;

            // 2. Registro del ViewModel
            builder.Services.AddTransient<TramiteViewModel>();

            // 3. Registro de la Vista
            builder.Services.AddTransient<MainPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
