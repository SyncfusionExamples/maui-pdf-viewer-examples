using Syncfusion.Maui.PdfViewer.Annotations;
using Syncfusion.Maui.PdfViewer;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
using System.Globalization;
using System.Reflection;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Controls;
using Syncfusion.Pdf;
using Syncfusion.Maui.Core.Internals;
using System.Diagnostics;
using System.IO;

namespace Flatten;

public partial class MainPage : ContentPage
{
    Stream loadedStream;
    List<string> annotationModes;
    List<string> colorStrings;
    List<Color> colors;
    Annotation annotation;
    FormField formField;
    public MainPage()
    {
        InitializeComponent();
        Stream loadedStream = typeof(MainPage).GetTypeInfo().Assembly.GetManifestResourceStream("Flatten.AnnotationsFormfields.pdf");
        pdfViewer.LoadDocument(loadedStream, flattenOptions: FlattenOptions.Unsupported);
    }

    private void Button_Clicked_4(object sender, EventArgs e)
    {
        foreach (var annotation in pdfViewer.Annotations)
        {
            annotation.FlattenOnSave = true;
        }
    }

    private void Button_Clicked_6(object sender, EventArgs e)
    {
        foreach (var formField in pdfViewer.FormFields)
        {
            formField.FlattenOnSave = true;
        }
    }

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

    private void Button_Clicked(object sender, EventArgs e)
    {
        SaveDocument();
    }
}
