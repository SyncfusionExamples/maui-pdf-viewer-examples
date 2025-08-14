# How to disable the loading indicator?

This project demonstrate how to disable the loading indicator in [SfPdfViewer](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html)

## Prerequisites

1. A .NET MAUI project set up.
2. The [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer) package is installed.

## Disable the loading indicator.

To disable the loading indicator while loading the pdf and rendering the pages, by set the color of the loading indicator as Transparent in the `App.Xaml` file using the [Themes key for .NET MAUI PdfViewer](https://help.syncfusion.com/maui/themes/keys#sfpdfviewer). Refer the code example below for disable the loading indicator.

**Xaml**

```xaml
        <ResourceDictionary>
            <x:String x:Key="SfPdfViewerTheme">Custom Theme</x:String>
            <Color x:Key="SfPdfViewerLoadingIndicatorColor">Transparent</Color>
        </ResourceDictionary>
```

## Run the App

1. Build and run the application on all platforms.
2. Scroll through the pages.





