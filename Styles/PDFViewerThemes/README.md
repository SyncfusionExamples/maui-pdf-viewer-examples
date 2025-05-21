# Applying Themes to PDF Viewer using Theme Keys

This directory contains a sample project that demonstrates how to apply themes to the PDF Viewer using theme keys. The sample includes updates for contrast themes where the hover and pressed states of buttons and list views are set to aquatic colors.

## Prerequisites
1. .NET MAUI installed.
2. Syncfusion.Maui.PdfViewer NuGet package.

## Steps to Apply Themes

### Step 1: Create a MAUI Sample and Initialize PDF Viewer

1. Initialize your .NET MAUI project.
2. Add the PDF Viewer control to your project.

### Step 2: Apply Themes using Keys in `App.xaml`

You can customize the appearance of `SfPdfViewer` by merging theme keys directly into application resources, without including the entire common theme resource or control style dictionaries.

```xml
<Application xmlns:SyncTheme="clr-namespace:Syncfusion.Maui.Themes;assembly=Syncfusion.Maui.Core"
             ...>
    <Application.Resources>
        <SyncTheme:SyncfusionThemeDictionary>
            <SyncTheme:SyncfusionThemeDictionary.MergedDictionaries>
                <ResourceDictionary>
                    <!-- Apply dark theme for the PDF Viewer -->
                    <SyncTheme:SyncfusionThemeResourceDictionary VisualTheme="MaterialDark"/>
                    
                    <!-- Apply custom theme for specific elements -->
                    <x:String x:Key="SfPdfViewerTheme">Custom Theme</x:String>
                    <Color x:Key="SfPdfViewerNormalDialogSaveButtonBackgroundColor">Aqua</Color>
                    <Color x:Key="SfPdfViewerNormalScrollHeadBackgroundColor">Red</Color>
                    <Color x:Key="SfPdfViewerNormalScrollHeadBorderColor">Blue</Color>
                    <Color x:Key="SfPdfViewerNormalScrollHeadTextColor">Green</Color>
                    <Color x:Key="SfPdfViewerNormalContextMenuBackGroundColor">Yellow</Color>
                </ResourceDictionary>
            </SyncTheme:SyncfusionThemeDictionary.MergedDictionaries>
        </SyncTheme:SyncfusionThemeDictionary>
    </Application.Resources>
</Application>
```
For more detailed guidance on applying themes, please refer to the [Syncfusion MAUI Themes User Guide](https://help.syncfusion.com/maui/themes/themes).       

### Step 3: Load PDF into the PDF Viewer
Initialize the PDF Viewer control in your .NET MAUI application and bind the `DocumentSource` property of the PDF Viewer to the PdfDocumentStream property defined in the PdfViewerViewModel class.

```xaml
 <syncfusion:SfPdfViewer x:Name="pdfViewer" DocumentSource="{Binding PdfDocumentStream}">
        </syncfusion:SfPdfViewer>
``` 

In your PdfViewerViewModel class, set the PDF stream to the PdfDocumentStream property from the CreatePDF method. 

```csharp
public class PdfViewerViewModel
{
    public Stream PdfDocumentStream { get; private set; }

    public PdfViewerViewModel()
    {
        // Load the embedded PDF document stream.
        pdfDocumentStream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("PDFViewerThemes.Assets.PDF_Succinctly.pdf");
    }
}
``` 
## 3.Run the sample to see the PDF Viewer with custom theme applied.
1. Build and deploy your .NET MAUI application on your preferred platform (Android, iOS, Windows, and Mac Catalyst).
2. The PDF Viewer should display your PDF document with the applied custom theme settings.