# [Android, iOS] How to customize the more options drop down?
This project demonstrates how to build a .NET MAUI application that customize the more options drop down in mobile platform, by replacing the more option item with a new item in the index of the more option item in Android and IOS platform.

# Prerequisites
1. A .NET MAUI project set up.
2. The Syncfusion PDF Viewer NuGet package is installed.

# Steps
## 1.Install Required NuGet Package
To get started, create a new https://dotnet.microsoft.com/en-us/learn/maui/first-app-tutorial/create and ensure the following package is installed in your .NET MAUI project:
https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer
You can install this package using the NuGet Package Manager or the NuGet CLI.

## 2. Initialize the PDF Viewer in XAML
Start by adding the Syncfusion PDF Viewer control to your XAML file.
a. Add the Syncfusion namespace in your MainPage.xaml:
Define the XAML namespace to enable access to the PDF Viewer.
XAML: xmlns:syncfusion="clr-namespace:Syncfusion.Maui.PdfViewer;assembly=Syncfusion.Maui.PdfViewer"

b. Add the PDF Viewer control to your layout:
Initialize the SfPdfViewer in the XAML file. This will display the PDF Viewer in your app. You can load any PDF document into this Viewer.
<syncfusion:SfPdfViewer x:Name="pdfViewer" DocumentLoaded="PdfViewerDocumentLoaded"></syncfusion:SfPdfViewer>

c. Load the PDF in the PDF Viewer control:
In your MainPage.xaml.cs, the PDF Viewer is initialized with a PDF document embedded in your resources.

```csharp
Stream stream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("Toolabarissue.Assets.PDF_Succinctly.pdf");
pdfViewer.LoadDocument(stream);
```

## 3. Replace the existing item with another item in the top toolbar in Android and IOS platform.
In the document loaded event:
 1. Get the index value of the more items in the top toolbar using the more item name.
 2. Get the more items value in the top toolbar using the more item name.
 3. Remove the more item using the more item name.
 4. Creating a new button.
 5. Insert the new created button as item in the index of the more items which is removed from the top toolbar.

```csharp
private void PdfViewerDocumentLoaded(object sender, EventArgs e)
{

#if ANDROID || IOS
            // Getting the index value of the more item toolbar item from the top toolbar.
            var index = (int)pdfViewer?.Toolbars?.GetByName("TopToolbar")?.Items?.GetByName("MoreItem")?.Index;

            // Get the more item from the top toolbar.
            var item = pdfViewer.Toolbars?.GetByName("TopToolbar")?.Items?.GetByName("MoreItem");

            if (item != null)
                // Remove the more item from the top toolbar.
                pdfViewer.Toolbars?.GetByName("TopToolbar")?.Items?.Remove(item);
            
            // Creating a new print button to replace the more item.
            Button printButton = new Button
            {
                Text = "\ue77f",
                FontSize = 24,
                FontFamily = "MauiMaterialAssets",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = Colors.Transparent,
                TextColor = Color.FromArgb("#49454F"),
                IsEnabled = true,
                Padding = 10,

            };

            // replacing the print button in the index of the more item. 
            pdfViewer.Toolbars?.GetByName("TopToolbar")?.Items?.Insert(index, new Syncfusion.Maui.PdfViewer.ToolbarItem(printButton, "printButton"));
#endif

}
```

# Run the App
1. Build and run the application on your preferred platform.
2. Load the PDF in the viewer.