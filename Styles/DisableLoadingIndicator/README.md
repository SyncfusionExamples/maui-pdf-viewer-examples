# How to disable the loading indicator?

This project demonstrate how to disable the loading indicator in [SfPdfViewer](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html)

## Prerequisites

1. A .NET MAUI project set up.
2. The [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer) package is installed.

## Disable the loading indicator.

The loading indicator is visible in the following scenarios:
- When a PDF document is loaded into the PDF Viewer.
- While rendering individual pages, particularly when scrolling through large or complex PDF documents.

Currently, there is no direct API available to completely disable the loading indicator in the PDF Viewer control. However, as a workaround, you can minimize its visibility by setting the loading indicator color to Transparent in the `App.xaml` file using the [Themes key in .NET MAUI PdfViewer](https://help.syncfusion.com/maui/themes/keys#sfpdfviewer). This effectively hides the indicator without removing its behavior as shown in the following code example.

**Xaml**

```xaml
        <ResourceDictionary>
            <!-- Defines a custom theme label for SfPdfViewer. This signals the control to apply custom styling. -->
            <x:String x:Key="SfPdfViewerTheme">Custom Theme</x:String>
            <!-- Sets the loading indicator color to Transparent to visually hide it -->
            <Color x:Key="SfPdfViewerLoadingIndicatorColor">Transparent</Color>
        </ResourceDictionary>
```

## Run the App

1. Build and run the application on all platforms.
2. Navigate through multiple pages by scrolling.
3. Change the zoom mode and scroll through the PDF document.
4. Test with both large and small PDF files.





