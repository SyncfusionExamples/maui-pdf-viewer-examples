# How to open various type of documents (.docx, .xlsx, images, .xps, .pdf, .pptx) in MAUI PDF Viewer?
This project demonstrates how to open and view multiple document formats—including .docx, .xlsx, images, .xps, .pdf, and .pptx within a .NET MAUI PDF Viewer by converting each file type into a PDF.

## Prerequisites
1. A .NET MAUI project set up.
2. [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer) package.
3. [Syncfusion.Maui.TabView](https://www.nuget.org/packages/Syncfusion.Maui.TabView) package.
4. [Syncfusion.DocIORenderer.NET](https://www.nuget.org/packages/Syncfusion.DocIORenderer.NET) package to convert Word document to PDF.
5. [Syncfusion.Pdf.Imaging.NET](https://www.nuget.org/packages/Syncfusion.Pdf.Imaging.NET) package used to draw the image files to PDF.
6. [Syncfusion.Presentation.NET](https://www.nuget.org/packages/Syncfusion.Presentation.NET) package used to read, write and converting the PowerPoint presentation programmatically.
7. [Syncfusion.PresentationRenderer.NET](https://www.nuget.org/packages/Syncfusion.PresentationRenderer.NET) package to preserve the original appearance of the PowerPoint presentation in the converted PDF document.
8. [Syncfusion.XlsIORenderer.NET](https://www.nuget.org/packages/Syncfusion.XlsIORenderer.NET) package to convert the Excel document to PDF document.
9. [Syncfusion.XpsToPdfConverter.NET](https://www.nuget.org/packages/Syncfusion.XpsToPdfConverter.NET) package used to convert.xps format file to PDF document file.

## Converting word document to PDF document

In .NET MAUI, to convert a word document to a PDF document, refer the [Word to PDF conversion document](https://help.syncfusion.com/document-processing/word/conversions/word-to-pdf/net/convert-word-document-to-pdf-in-maui).

## Converting excel document to PDF document

In .NET MAUI, to convert an excel document to a PDF document, refer the [Excel to PDF conversion document](https://help.syncfusion.com/document-processing/excel/conversions/excel-to-pdf/net/convert-excel-to-pdf-in-maui).

## Converting image to PDF document

In .NET MAUI, to convert an image to a PDF document, refer the [image to PDF conversion document](https://help.syncfusion.com/document-processing/pdf/pdf-library/net/converting-images-to-pdf).

## Converting xps document to PDF document

In .NET MAUI, to convert a xps document to a PDF document, refer the [xps to PDF conversion document](https://help.syncfusion.com/document-processing/pdf/pdf-library/net/converting-xps-to-pdf).

## Converting ppt to PDF document

In .NET MAUI, to convert a PowerPoint presentation to a PDF document, refer the [PowerPoint to PDF conversion document](https://help.syncfusion.com/document-processing/powerpoint/conversions/powerpoint-to-pdf/net/convert-powerpoint-to-pdf-in-maui).

## Loading the converted pdf in the .NET MAUI PDF Viewer

### Conversion of converted pdf into stream

Create a new memory stream and save the converted pdf to it using the [Save](https://help.syncfusion.com/cr/maui/Syncfusion.Pdf.PdfDocument.html#Syncfusion_Pdf_PdfDocument_Save_System_IO_Stream_) method of the `PdfDocument` class in the [.NET MAUI PDF Library](https://help.syncfusion.com/cr/maui/Syncfusion.Pdf.html)

```csharp
        MemoryStream pdfStream = new MemoryStream();
        pdfDocument.Save(pdfStream);
```

### Loading the stream in to the PDFViewer.

Assign the saved PDF stream to the [DocumentSource](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html#Syncfusion_Maui_PdfViewer_SfPdfViewer_DocumentSource) in the [SfPdfViewer](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html).

```csharp
        PdfViewer.DocumentSource = pdfStream;
```

## Run the App

1. Build and run the application on all platforms.
2. Switch to all tabs and check whether all pdf is loading.
3. Ensure all behaviors of PdfViewer in all PdfViewer tabs.