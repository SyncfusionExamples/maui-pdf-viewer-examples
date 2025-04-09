# How to create a PDF document using a PDF library and load it in PdfViewer without saving to disk?

This folder contains a sample project that demonstrates how to create a PDF document using the Syncfusion PDF library and load it into the PDF Viewer directly, without saving it to disk. 

# Steps to create a PDF using the Syncfusion PDF library and load it in PdfViewer without saving to disk
1. Create a PDF document using the Syncfusion PDF library.
2. Load the created PDF stream to the PDF Viewer.

# Prerequisites
1. .NET MAUI installed.
2. Syncfusion.Maui.PdfViewer NuGet package.

# Steps
## 1. Create PDF
In this example, we will load the PDF document through MVVM binding. Create a new C# file named PdfViewerViewModel.cs. Create the PDF using the PDF library and save the PDF as a stream. Create the following method in the PdfViewerViewModel class.
Create the PDF using the PDF library and save the PDF as stream. Create the below method in the PdfViewerViewModel class.

```csharp
 private MemoryStream CreatePDF()
        {
            //Create a new PDF document.
            PdfDocument document = new PdfDocument();
            //Add a page to the document.
            PdfPage page = document.Pages.Add();
            //Create PDF graphics for the page.
            PdfGraphics graphics = page.Graphics;

            //Set the standard font.
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
            //Draw the text.
            graphics.DrawString("Hello World!!!", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));

            //Creating the stream object.
            MemoryStream stream = new MemoryStream();
            //Save the document into memory stream.
            document.Save(stream);
            //Close the document.
            document.Close(true);

           return stream;

        }
```
## 2. To load PDF in the PDF Viewer
Initialize the PDF Viewer control in the .NET MAUI application and load the PDF stream into the PDF Viewer by binding the PDF Viewer's `DocumentSource` to the PdfDocumentStream property of the PdfViewerViewModel class.

```xaml
 <syncfusion:SfPdfViewer x:Name="pdfViewer" DocumentSource="{Binding PdfDocumentStream}">
        </syncfusion:SfPdfViewer>
``` 

In your PdfViewerViewModel class, set the created stream to the PdfDocumentStream property from the CreatePDF method. 

```csharp
public class PdfViewerViewModel
{
    public Stream PdfDocumentStream { get; private set; }

    public PdfViewerViewModel()
    {
        PdfDocumentStream = CreatePDF();
    }
}
``` 
## 3.Run the sample to view the PDF in the PDF Viewer.
1. Build and deploy your .NET MAUI application on your preferred platform (Android, iOS, Windows, and Mac Catalyst).
2. The PDF Viewer should display the "This PDF was created using the Syncfusion PDF Library." PDF document directly from the stream.