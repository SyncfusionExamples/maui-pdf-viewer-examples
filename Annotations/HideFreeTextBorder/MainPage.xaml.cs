using Syncfusion.Maui.PdfViewer;

namespace HideFreeTextBorder
{
    public partial class MainPage : ContentPage
    {       
        public MainPage()
        {
            // Initialize the components on this page
            InitializeComponent();

            // Customize the default settings for free text annotations
            CustomizeDefaultFreeTextSettings();
        }

        // Method to customize default free text annotation settings
        void CustomizeDefaultFreeTextSettings()
        {
            // Obtain the default settings for free text annotations
            FreeTextAnnotationSettings freeTextSettings = pdfViewer.AnnotationSettings.FreeText;

            // Set the border width to 0 to hide the border around free text annotations
            freeTextSettings.BorderWidth = 0;
        }
    }
}