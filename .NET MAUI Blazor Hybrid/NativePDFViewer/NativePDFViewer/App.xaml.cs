namespace MauiApp5
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Use a NavigationPage to enable proper back navigation
            // when navigating from Blazor to native MAUI pages like PdfViewerPage.
            // The BlazorWebView (MainPage) sits at the root of the navigation stack,
            // and native pages like PDF viewer are pushed on top of it.
            return new Window(new NavigationPage(new MainPage())) { Title = "MauiApp5" };
        }
    }
}
