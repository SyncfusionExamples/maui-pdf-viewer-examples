using Syncfusion.Maui.PdfViewer;
using System.Globalization;
using System.Resources;

namespace Localization
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();       

            SetCulture("en-US"); // Set default culture
        }

        public void SetCulture(string cultureCode)
        {
            CultureInfo culture = new CultureInfo(cultureCode);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            // Additional UI updates if necessary
        }
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}