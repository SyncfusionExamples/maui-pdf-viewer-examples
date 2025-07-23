# Opening a PDF Document from a Base64 String in .NET MAUI PDF Viewer
This project demonstrates how to open a PDF document from a Base64 string by converting it to a byte[] and assigning it to the [DocumentSource](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html#Syncfusion_Maui_PdfViewer_SfPdfViewer_DocumentSource) of .NET MAUI PDF Viewer.

## Prerequisites
1. A .NET MAUI project set up.
2. The [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer) NuGet package installed.

## Code Example
Use the `Convert` class from the `System` namespace to convert the Base64 string to byte array and assign it to the [DocumentSource](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html#Syncfusion_Maui_PdfViewer_SfPdfViewer_DocumentSource).

```csharp
    // Decode the Base64 string into a byte array.
    byte[] decodedBytes = System.Convert.FromBase64String("YourBase64String");
```

## Run the App
Build and run the application.
