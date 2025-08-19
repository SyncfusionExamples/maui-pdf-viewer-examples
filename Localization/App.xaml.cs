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
            CultureInfo.CurrentUICulture = new CultureInfo("de-DE");
            if(Application.Current != null)
            SfPdfViewerResources.ResourceManager = new ResourceManager("Localization.Resources.SfPdfViewer",
                Application.Current.GetType().Assembly);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}