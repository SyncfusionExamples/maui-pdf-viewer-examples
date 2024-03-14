using Syncfusion.Maui.PdfViewer;
using System.Reflection;

namespace Flatten;

public partial class MainPage : ContentPage
{
    // Initializes the MainPage by loading the PDF document and sets up the PdfViewer and flattenOptions.
    public MainPage()
    {
        InitializeComponent();
        // Loads the PDF document from the embedded resource "Flatten.AnnotationsFormfields.pdf".
        Stream loadedStream = typeof(MainPage).GetTypeInfo().Assembly.GetManifestResourceStream("Flatten.AnnotationsFormfields.pdf");
        pdfViewer.LoadDocument(loadedStream);
        // Associates the PdfViewer with flattenOptions for further processing.
        flattenOptions.PdfViewer = pdfViewer;
    }

    // Event handler for Button click event, toggles the visibility of flattenOptions.
    private void Button_Clicked(object sender, EventArgs e)
    {
        // Toggles the visibility of flattenOptions.
        if (flattenOptions.IsVisible)
            flattenOptions.IsVisible = false; // Hides flattenOptions if it's currently visible.
        else
            flattenOptions.IsVisible = true; // Shows flattenOptions if it's currently hidden.
    }
}
