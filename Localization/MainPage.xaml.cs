using Syncfusion.Maui.PdfViewer;
using System.Globalization;
using System.Resources;

namespace Localization
{
    public partial class MainPage : ContentPage
    {
        CultureInfo initialCulture;

        public MainPage()
        {
            InitializeComponent();

            // Backup the device's current culture before setting the new culture to demonstrate RTL.
            initialCulture = CultureInfo.CurrentUICulture;

            // Set the device's culture to Arabic to demonstrate RTL.
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ar-AE");

            string basePath = "Localization.Localization";
            SfPdfViewerResources.ResourceManager = new ResourceManager($"{basePath}.SfPdfViewer", Application.Current!.GetType().Assembly);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Restore the device's original culture while navigating out of the sample.
            CultureInfo.DefaultThreadCurrentUICulture = initialCulture;
        }
    }
}
