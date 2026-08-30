using Microsoft.Extensions.Logging;

using MauiApp5.PdfViewer;
using Syncfusion.Maui.Core.Hosting;

namespace MauiApp5
{
    public static class MauiProgram
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.ConfigureSyncfusionCore();

            // Register PDF Viewer services for dependency injection
            builder.Services.AddSingleton<IPdfViewerService, PdfViewerService>();
            builder.Services.AddSingleton<PdfDocumentManager>();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            var mauiApp = builder.Build();
            ServiceProvider = mauiApp.Services;
            return mauiApp;
        }
    }
}
