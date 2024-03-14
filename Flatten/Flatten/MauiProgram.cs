using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;

namespace Flatten
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Roboto-Medium.ttf", "Roboto");
                    fonts.AddFont("CurlzMT.ttf", "CurlzMt");
                    fonts.AddFont("FORTE.ttf", "Forte");
                    fonts.AddFont("marlett.ttf", "Marlett");
                });

#if DEBUG
		builder.Logging.AddDebug();
#endif
            builder.ConfigureSyncfusionCore();
            return builder.Build();
        }
    }
}