using Syncfusion.Maui.PdfViewer;
using System.Reflection;
using System.Resources;

namespace Localization;

public partial class SfPdfViewerPage : ContentPage
{
    public SfPdfViewerPage(string selectedLanguage)
    {
        // Implement logic to set culture based on language
        string cultureCode = selectedLanguage switch
        {
            "German" => "de-DE",
            "Arabic" => "ar-AE",
            _ => "en-US",
        };

        if (Application.Current is App app)
        {
            app.SetCulture(cultureCode);
        }

        SfPdfViewerResources.ResourceManager = new ResourceManager(
            "Localization.Resources.SfPdfViewer",
            typeof(App).Assembly
        );

        InitializeComponent();
        LoadPdfWithLocalization();    
    }
    
    protected async override void OnDisappearing()
    {
        base.OnDisappearing();
        await pdfViewer.UnloadDocumentAsync();
    }
    private async void LoadPdfWithLocalization()
    {        
        // Load the PDF document
        var documentStream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("Localization.Assets.PDF_Succinctly.pdf");
        await pdfViewer.LoadDocumentAsync(documentStream);
    }
}