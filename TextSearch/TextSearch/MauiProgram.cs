using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;

namespace TextSearch
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
                });

            builder.ConfigureSyncfusionCore();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MDAxQDMyMzgyZTMwMmUzMGFMV081MVlpVnJ5aHJyQzUzMFFBOUJwZ1B0YWkxNC80WnA3SHdvbmdqNzA9");

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
