using Syncfusion.Maui.PdfViewer;
using System.Reflection;

namespace Flatten;

public partial class MainPage : ContentPage
{
   
    public MainPage()
    {
        InitializeComponent();
        Stream loadedStream = typeof(MainPage).GetTypeInfo().Assembly.GetManifestResourceStream("Flatten.AnnotationsFormfields.pdf");
        pdfViewer.LoadDocument(loadedStream, flattenOptions: FlattenOptions.Unsupported);
    }

    // Sets FlattenOnSave property to true for each annotation in the pdfViewer.
    private void Button_Clicked_4(object sender, EventArgs e)
    {
        foreach (var annotation in pdfViewer.Annotations)
        {
            annotation.FlattenOnSave = true;
        }
    }

    // Sets FlattenOnSave property to true for each form field in the pdfViewer.
    private void Button_Clicked_6(object sender, EventArgs e)
    {
        foreach (var formField in pdfViewer.FormFields)
        {
            formField.FlattenOnSave = true;
        }
    }

    // Saves the modified PDF document to a file.
    void SaveDocument()
    {
        Stream stream = new MemoryStream();
        pdfViewer.SaveDocument(stream);
        string fileName = "SavedPDF.pdf";
#if WINDOWS
       string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
       using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
       {
           stream.CopyTo(fileStream);
       }
#elif ANDROID || IOS || MACCATALYST
        string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            stream.CopyTo(fileStream);
        }
#endif
        DisplayAlert("Information", "Successfully saved", "OK");
    }

    // Event handler for Button click event, calls SaveDocument to save the PDF document.
    private void Button_Clicked(object sender, EventArgs e)
    {
        SaveDocument();
    }
}
