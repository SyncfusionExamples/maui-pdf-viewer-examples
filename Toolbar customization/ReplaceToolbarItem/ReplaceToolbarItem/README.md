# How to replace a toolbar item with another toolbar item?
This project demonstrates how to build a .NET MAUI application that replaces the existing toolbar item with a new toolbar item in the index of the removed existing toolbar item. As an example, the toolbar item we consider for replacing is the "MoreOptions" item, which includes "Outline" and "Print" as dropdown options on the top toolbar in both Android and iOS platforms

## Prerequisites
1. A .NET MAUI project set up.
2. The Syncfusion PDF Viewer NuGet package is installed - [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer).

## Steps

### 1.Install Required NuGet Package
To get started, create a new [Maui App](https://dotnet.microsoft.com/en-us/learn/maui/first-app-tutorial/create) and ensure the following package is installed in your .NET MAUI project:

[Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer)

You can install this package using the NuGet Package Manager or the NuGet CLI.

### 2. Initialize the PDF Viewer in XAML

Start by adding the Syncfusion PDF Viewer control to your XAML file.

**a. Add the Syncfusion namespace in your MainPage.xaml:**

Define the XAML namespace to enable access to the PDF Viewer.

**XAML:**

```xaml
xmlns:syncfusion="clr-namespace:Syncfusion.Maui.PdfViewer;assembly=Syncfusion.Maui.PdfViewer"
```

**b. Add the PDF Viewer control to your layout:**

Initialize the SfPdfViewer in the XAML file. This will display the PDF Viewer in your app. You can load any PDF document into this Viewer and wire the DocumentLoaded event for the PdfViewer.

**XAML:**

```xaml
<syncfusion:SfPdfViewer x:Name="pdfViewer" DocumentLoaded="PdfViewerDocumentLoaded"></syncfusion:SfPdfViewer>
```

**c. Load the PDF in the PDF Viewer control:**

In your MainPage.xaml.cs, the PDF Viewer is initialized with a PDF document embedded in your resources.

```csharp
Stream stream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("ReplaceToolbarItem.Assets.PDF_Succinctly.pdf");
pdfViewer.LoadDocument(stream);
```

### 3. Replace the existing item with another item in the top toolbar in Android and IOS platform.

In the wired document loaded event: (The toolbar item we used in below step for replacing is the "MoreOptions" item, which includes "Outline" and "Print" as its dropdown options on the top toolbar in both Android and iOS platforms.)
 1. Get the index value of the toolbar item in the top toolbar using their name .
 2. Get the toolbar item from the top toolbar using their name.
 3. Remove the toolbar item from the top toolbar using their name.
 4. Create a new button.
 5. Replace the toolbar item by the newly created button as a toolbar item using the index value of the toolbar item.

```csharp
private void PdfViewerDocumentLoaded(object sender, EventArgs e)
{

#if ANDROID || IOS
            // Get the index value of the "MoreItem" toolbar item from the top toolbar.
            var index = (int)pdfViewer?.Toolbars?.GetByName("TopToolbar")?.Items?.GetByName("MoreItem")?.Index;

            // Get the "MoreItem" toolbar item from the top toolbar.
            var item = pdfViewer.Toolbars?.GetByName("TopToolbar")?.Items?.GetByName("MoreItem");

            if (item != null)
                // Remove the "MoreItem" toolbar item from the top toolbar.
                pdfViewer.Toolbars?.GetByName("TopToolbar")?.Items?.Remove(item);
            
            // Creating a new print button to replace the "MoreItem" toolbar item.
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

            // Replace the print button at the index of the "MoreItem" toolbar item. 
            pdfViewer.Toolbars?.GetByName("TopToolbar")?.Items?.Insert(index, new Syncfusion.Maui.PdfViewer.ToolbarItem(printButton, "printButton"));
#endif

}
```

## note:

We can replace the existing toolbar item using a new toolbar item in Windows and MAC platform, by using the toolbars and toolbar items names used in the Windows and MAC platform.

Toolbar name: [Toolbar Names](https://help.syncfusion.com/maui/pdf-viewer/toolbar#desktop-toolbar-names)

Toolbar item name: [Toolbar Item Names](https://help.syncfusion.com/maui/pdf-viewer/toolbar#desktop-toolbar-item-names)

## Run the App

Build and run the application on your preferred platform.