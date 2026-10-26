using Microsoft.Extensions.DependencyInjection;

namespace ViewOnlyPdf
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var page = IPlatformApplication.Current?.Services.GetRequiredService<MainPage>()
                ?? new MainPage(new DocumentAccessPolicy());
            var navigation = new NavigationPage(page)
            {
                BarBackgroundColor = Color.FromArgb("#512BD4"),
                BarTextColor = Colors.White
            };
            return new Window(navigation);
        }
    }
}