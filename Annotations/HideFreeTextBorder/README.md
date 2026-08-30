# How to hide the border of free text annotation in MAUI PDF Viewer?

This folder contains a sample project that demonstrates How to hide the border of free text annotation in MAUI PDF Viewer

## Prerequisites
1. .NET MAUI installed.
2. Syncfusion.Maui.PdfViewer NuGet package.

## Steps to achieve borderless free text annotations:

### 1. Install Required NuGet Package

Create a .NET MAUI application and install the `Syncfusion.Maui.PdfViewer` NuGet package using the NuGet Package Manager or the CLI.

### 2. Initialize PDF Viewer in XAML

1. **Add Namespace:**
   Include the Syncfusion namespace in your `MainPage.xaml` file to use the PDF Viewer control.

   ```xml
   xmlns:syncfusion="clr-namespace:Syncfusion.Maui.PdfViewer;assembly=Syncfusion.Maui.PdfViewer"
   ```

2. **Add PDF Viewer Control:**
   Insert the `SfPdfViewer` control into your layout and bind it to a document source for displaying PDF files.

   ```xaml
   <syncfusion:SfPdfViewer x:Name="pdfViewer" DocumentSource="{Binding PdfDocumentStream}"/>
   ```

### 3. Load a PDF Document

Create a `PdfViewerViewModel` class and assign an embedded PDF file stream to the `PdfDocumentStream` property:

```csharp
public class PdfViewerViewModel
{
    public Stream PdfDocumentStream { get; private set; }

    public PdfViewerViewModel()
    {
        // Set the embedded PDF stream
        PdfDocumentStream = this.GetType().Assembly.GetManifestResourceStream("HideFreeTextBorder.Assets.PDF_Succinctly1.pdf");
    }
}
```

### 4. Customize Free Text Annotation

To remove the border from free text annotations, set the `BorderWidth` property of `FreeTextAnnotationSettings` to `0` in your code-behind file:

```csharp
FreeTextAnnotationSettings freeTextSettings = pdfViewer.AnnotationSettings.FreeText;
freeTextSettings.BorderWidth = 0;
```

### 5. Run the Application

Run your .NET MAUI app, load a PDF, and add free text annotations using the toolbar. The annotations will render without visible borders.