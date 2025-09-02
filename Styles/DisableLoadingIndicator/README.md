# How to disable the loading indicator?

This project demonstrate how to disable the loading indicator in [SfPdfViewer](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html)

## Prerequisites

1. A .NET MAUI project set up.
2. The [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer) package is installed.

## Disable the loading indicator.

The loading indicator is visible in the following scenarios:
- Initial PDF loading process.
- Delay in the page rendering process when page navigation and bookmark navigation on large documents.

Currently, there is no direct API available to completely disable the loading indicator in the PDF Viewer control. However, as a workaround, you can alter its visibility by setting the loading indicator color to Transparent using the [Themes key available for .NET MAUI PdfViewer](https://help.syncfusion.com/maui/themes/keys#sfpdfviewer). This effectively hides the loading indicator.

To apply this workaround, you need to merge the `SfPdfViewerTheme` key into your application resources along with the color resource for the loading indicator and set the value as transparent to hide the loading indicator using the [Themes key in .NET MAUI PdfViewer](https://help.syncfusion.com/maui/themes/keys#sfpdfviewer) in the `App.xaml` file as shown in the below code example which is applied for both dark and light theme.

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
2. During initial PDF loading, ensure loading indicator is disabled.
2. Navigate through multiple pages by scrolling and confirm that the loading indicator remains hidden throughout page rendering. 





