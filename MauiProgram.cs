using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Moviles2.ViewModels;
using Moviles2.Views;
using Moviles2.Repositories;
using Moviles2.Interfaces;
using Moviles2.Services;
using System.Net.Http;

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

            // Servicios base y repositorios
            builder.Services.AddSingleton<ITramiteRepository, TramiteRepositorySQLite>();
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddTransient<TramiteService>();

            // Parte A: Trámite Local (SQLite)
            builder.Services.AddTransient<TramiteViewModel>();
            builder.Services.AddTransient<ResumenTramiteViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ResumenTramitePage>();

            // Parte B: Consumo API REST
            builder.Services.AddTransient<ListaPostulantesViewModel>();
            builder.Services.AddTransient<DetalleApiViewModel>();
            builder.Services.AddTransient<ListaPostulantesPage>();
            builder.Services.AddTransient<DetalleApiPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}