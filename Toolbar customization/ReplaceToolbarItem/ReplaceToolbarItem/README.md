# How to replace a built-in toolbar item with a custom application-level control in .NET MAUI PDF Viewer?
This project demonstrates how to replace an existing built-in toolbar item with a custom application-level control in the position of the removed toolbar item in the .NET MAUI PDF Viewer. For example, the built-in toolbar item we are replacing is the "MoreOptions" item, which includes "Outline" and "Print" as dropdown options on the top toolbar for both Android and iOS platforms, while the custom application-level control is a button.

## Prerequisites
1. A .NET MAUI project set up.
2. The [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer) package is installed.

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
     <Grid>
        <syncfusion:SfPdfViewer x:Name="pdfViewer" ></syncfusion:SfPdfViewer>
     </Grid>
```

**c. Load the PDF in the PDF Viewer control:**

In your MainPage.xaml.cs, the PDF Viewer is initialized with a PDF document embedded in your resources.

```csharp
    Stream? stream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("ReplaceToolbarItem.Assets.PDF_Succinctly.pdf");
    pdfViewer.LoadDocument(stream);
```

**d. Wire the document loaded event for the pdfViewer control**

The event is triggered when the document loaded.

**XAML:**

```xaml
    <syncfusion:SfPdfViewer x:Name="pdfViewer" DocumentLoaded="PdfViewerDocumentLoaded"></syncfusion:SfPdfViewer>
```

### 3. Replace the existing item with another item in the top toolbar in Android and IOS platform.
In the DocumentLoaded event handler:
 1. Retrieve the toolbar item from the top toolbar by its name.
 2. Determine the index position of the retrieved toolbar item within the top toolbar.
 3. Remove the retrieved toolbar item from the top toolbar using its name.
 4. Create a new button.
 5. Add a click event handler to the newly created button to verify that the actions are executed correctly.
 6. Replace the retrieved toolbar item with the new created button in the toolbar, using the previously obtained retrieved toolbar item index position.

```csharp
private void PdfViewerDocumentLoaded(object sender, EventArgs e)
{

#if ANDROID || IOS

            // Retrieve the toolbar item named "MoreOptions" from the "TopToolbar"
            var item = pdfViewer.Toolbars?.GetByName("TopToolbar")?.Items?.GetByName("MoreItem");

            // Obtain the index value of the retrieved toolbar item
            var index = (item?.Index != null) ? (int)item.Index : -1;

            if (item != null)
                // Remove the "MoreOptions" toolbar item from the top toolbar.
                pdfViewer.Toolbars?.GetByName("TopToolbar")?.Items?.Remove(item);

            // Create a new print button to replace the "MoreOptions" toolbar item.
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

            // Add a click event handler to the newly created button for initiating document printing
            printButton.Clicked += (s, e) =>
            {
                pdfViewer.PrintDocument();
            };

            // Replace the print button at the index of the "MoreOptions" toolbar item. 
            pdfViewer.Toolbars?.GetByName("TopToolbar")?.Items?.Insert(index, new Syncfusion.Maui.PdfViewer.ToolbarItem(printButton, "printButton"));
#endif

}
```

## Note:
We can replace the existing built-in toolbar item with a custom application-level control in Windows and MAC platform by using the [Toolbar Names](https://help.syncfusion.com/maui/pdf-viewer/toolbar#desktop-toolbar-names) and [Toolbar Item Names](https://help.syncfusion.com/maui/pdf-viewer/toolbar#desktop-toolbar-item-names) specified for the Windows and MAC platform.

## Run the App
Build and run the application on your Android and IOS platform.




