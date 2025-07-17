# How to Localize Texts in Syncfusion PDF Viewer for .NET MAUI

This sample demonstrates how to localize the user interface texts in the Syncfusion PDF Viewer component within a .NET MAUI application.

## Prerequisites

- .NET MAUI setup on your development environment.
- Syncfusion.Maui.PdfViewer NuGet package installed.
- Basic understanding of Resource files and localization concepts in .NET applications.

## Steps to Localize Texts in PDF Viewer

### 1. Install Required Packages

Ensure that you have installed the Syncfusion.Maui.PdfViewer NuGet package in your .NET MAUI project.

### 2. Add Resource Files

Create resource files for each language that you want to support. For example:
- `Resources/SfPdfViewer.resx` for default (e.g., English).
- `Resources/SfPdfViewer.de-DE.resx` for German.

Each resource file contains the key-value pairs where keys correspond to specific text elements in the PDF Viewer.

### 3. Set Up Resource Manager

In your `App.xaml.cs`, configure the Resource Manager to use your resource files:

```csharp
using Syncfusion.Maui.PdfViewer;
using System.Globalization;
using System.Resources;

namespace YourNamespace
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Set the culture you want to use
            CultureInfo.CurrentUICulture = new CultureInfo("de-DE");

            // Configure the ResourceManager for Syncfusion PDF Viewer localization
            SfPdfViewerResources.ResourceManager = new ResourceManager(
                "YourNamespace.Resources.SfPdfViewer",
                typeof(App).Assembly);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage());
        }
    }
}
```
### 4. Initialize the PDF Viewer in XAML

Open the MainPage.xaml file and follow the steps below.

1. Import the control namespace Syncfusion.Maui.PdfViewer, and then add the SfPdfViewer control inside the <ContentPage.Content> tag.
2. Name the PDF viewer control as pdfViewer.

```xaml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:syncfusion="clr-namespace:Syncfusion.Maui.PdfViewer;assembly=Syncfusion.Maui.PdfViewer"
             x:Class="YourNamespace.MainPage"
             x:DataType="local:PdfViewerViewModel">

    <ContentPage.Content>
        <syncfusion:SfPdfViewer x:Name="pdfViewer" DocumentSource="{Binding PdfDocumentStream}" />
    </ContentPage.Content>

</ContentPage>
```
### 5. Load the PDF in PDF Viewer

In your `PdfViewerViewModel.cs` file, load the PDF document stream into the PDF Viewer by setting the embedded PDF stream to the `PdfDocumentStream` property, which is then bound to the `DocumentSource` property of the `SfPdfViewer` control.

``` csharp

using System.Reflection;

public class PdfViewerViewModel
{
    public Stream PdfDocumentStream { get; set; }

    public PdfViewerViewModel()
    {
        // Load the embedded PDF document stream.
        PdfDocumentStream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("Localization.Assets.PDF_Succinctly.pdf");
    }
}

```

### 6. Run the Application
1. Build and run the application.
2. The texts in the PDF Viewer should reflect the language specified by CultureInfo.CurrentUICulture, showing the localized versions.