# How to include Open and Save options in the built-in toolbar and implement support for the DocumentSaveInitiated event?
This project demonstrate how to add the save and open options in the toolbar in [SfPdfViewer](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html) and how to create and handle a custom event This project demonstrates how to integrate Open and Save actions into a PDF viewer's toolbar using Syncfusion's .NET MAUI PDF Viewer. It also showcases how to create and handle a custom DocumentSaveInitiated event.

## Prerequisites
1. A .NET MAUI project set up.
2. The [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer) package is installed.

## To Include open and save option in built-in toolbar

To add an item at a specific index in the toolbar in [SfPdfViewer](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html), first create the UI element you want to include. Then, convert that element into a [ToolbarItem](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.ToolbarItem.html#Syncfusion_Maui_PdfViewer_ToolbarItem__ctor_Microsoft_Maui_Controls_View_System_String) using the ToolbarItem method. Finally, add the newly created ToolbarItem to the toolbar using the [Insert](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.ToolbarItemCollection.html#Syncfusion_Maui_PdfViewer_ToolbarItemCollection_Insert_System_Int32_Syncfusion_Maui_PdfViewer_ToolbarItem_) or [Add](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.ToolbarItemCollection.html#Syncfusion_Maui_PdfViewer_ToolbarItemCollection_Add_Syncfusion_Maui_PdfViewer_ToolbarItem_) method. Here we are using [Insert](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.ToolbarItemCollection.html#Syncfusion_Maui_PdfViewer_ToolbarItemCollection_Insert_System_Int32_Syncfusion_Maui_PdfViewer_ToolbarItem_) method to include the open and save button in the toolbar item.

### Step 1: Create open and save button with subscription of clicked event.

Create "Open" and "Save" buttons, and wire up their click events. In the Save button's click event handler, implement the logic to save the file, while in the Open button's handler, define the logic to open an existing file

**C#**

```csharp

        // Save button
        Button fileSaveButton = new Button
        {
            Text = "\ue75f", // Set button text
            FontSize = 24, // Set the size of the button text
            FontFamily = "MauiMaterialAssets", // Set icon font style for the button text
            BackgroundColor = Colors.Transparent, // Set background for the button
            BorderColor = Colors.Transparent, // Set border color of the button
            TextColor = Color.FromArgb("#49454F"), // Set button text color
            Padding = 10, // Set padding for the button
        };
        // Subscribe to the Clicked event to handle save logic when clicked
        fileSaveButton.Clicked += FileSaveButton_Clicked;

        // Open file button
        Button fileOpenButton = new Button
        {
            Text = "\ue712", // Set button text
            FontSize = 24, // Set the size of the button text
            FontFamily = "MauiMaterialAssets", // Set icon font style for the button text
            BackgroundColor = Colors.Transparent, // Set background for the button 
            BorderColor = Colors.Transparent, // Set border color of the button
            TextColor = Color.FromArgb("#49454F"), // Set text color of the button text
            Padding = 10 // Set padding for the button
        };
        // Subscribe to the Clicked event to handle file open logic when clicked
        fileOpenButton.Clicked += FileOpenButton_Clicked;
```

### Step 2: Include the created save and open button in the toolbar using toolbar names.

The created save and open button is inserted in the specific toolbar by the [Insert](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.ToolbarItemCollection.html#Syncfusion_Maui_PdfViewer_ToolbarItemCollection_Insert_System_Int32_Syncfusion_Maui_PdfViewer_ToolbarItem_) method using the [Mobile Toolbar Names](https://help.syncfusion.com/maui/pdf-viewer/toolbar#mobile-toolbar-names) and [Desktop Toolbar Names](https://help.syncfusion.com/maui/pdf-viewer/toolbar#desktop-toolbar-names).

**C#**

```csharp

        // Including the save button in the toolbar using toolbar name
        #if !WINDOWS && !MACCATALYST

                    // Insert the button into the "TopToolbar" on Android/iOS platforms
                    pdfViewer?.Toolbars?.GetByName("TopToolbar")?.Items?.Insert(0, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileSaveButton, "FileSaveButton"));
        #else
                    // Insert the button into the "PrimaryToolbar" for Windows/macOS platforms
                    pdfViewer?.Toolbars?.GetByName("PrimaryToolbar")?.Items?.Insert(0, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileSaveButton, "FileSaveButton"));
        #endif

        // Including the open button in the toolbar using toolbar name
        #if !WINDOWS && !MACCATALYST

                    // Insert the button into the "TopToolbar" on Android/iOS platforms
                    pdfViewer?.Toolbars?.GetByName("TopToolbar")?.Items?.Insert(1, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileOpenButton, "FileOpenButton"));
        #else
                    // Insert the button into the "PrimaryToolbar" for Windows/macOS platforms
                    pdfViewer?.Toolbars?.GetByName("PrimaryToolbar")?.Items?.Insert(1, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileOpenButton, "FileOpenButton"));
        #endif

```

## To Create and handle custom DocumentSaveInitiated event.

### Step 1: Create a custom event argument class

Construct a `DocumentSaveEventArgs` class having `SaveStream` property, having constructor with one `Stream` parameter used to carry the saved stream. 

**C#**

```csharp
        public class DocumentSaveEventArgs : EventArgs
        {
            public Stream SaveStream { get; }

            public DocumentSaveEventArgs(Stream saveStream)
            {
                SaveStream = saveStream;
            }
        }
```

### Step 2: Subscription of DocumentSaveInitiated event.

Subscribe the `DocumentSaveInitiated` event in the MainPage constructor 

**C#**

```csharp
            // Subscribe to DocumentSaveInitiated event
            this.DocumentSaveInitiated += MainPage_DocumentSaveInitiated;
```

### Step 3: Invoke DocumentSaveInitiated event in the save button event handler.

In the save button click event handler, save the current document as memory stream, construct a `DocumentSaveEventArgs` using that stream, and invoke the `DocumentSaveInitiated` event by passing the newly created argument.

**C#**

```csharp
        private async void FileSaveButton_Clicked(object? sender, EventArgs e)
        {

            // Create a new memory stream to hold the saved PDF document
            MemoryStream saveStream = new MemoryStream();

            // Asynchronously save the current document content into the memory stream
            await pdfViewer.SaveDocumentAsync(saveStream);

            // Trigger the DocumentSaveInitiated event
            DocumentSaveInitiated?.Invoke(this, new DocumentSaveEventArgs(saveStream));

        }
```

### Step 4: Calling save logic method in the DocumentSaveInitiated event handler.

This event `DocumentSaveInitiated` handler designed to respond when a document saving process is initiated in the UI.

**C#**

```csharp
        // This method is called before saving starts
        private void MainPage_DocumentSaveInitiated(object? sender, DocumentSaveEventArgs e)
        {
            var viewModel = (PdfViewerViewModel)BindingContext;

            // Calling the save logic method by passing the saved stream as it parameter.
            viewModel.SavePDFDocument(e.SaveStream);
        }
```

### Step 5: Document save logic.

Set the location path and create a file name to save the pdf document. Create a file in the location path specified, after that copy the provided stream into the file.

**C#**

```csharp
        public async void SavePDFDocument(Stream saveStream)
        {
            try
            {
                string fileName = "SavedPDF.pdf";
                string filePath;
                // Set the file path to save the saved pdf document.
        #if WINDOWS
                filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName); // Define the file path for Windows.
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    saveStream.CopyTo(fileStream); // Copy the stream to the file stream.
                }
        #elif ANDROID
                filePath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads)!.AbsolutePath, fileName);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    saveStream.CopyTo(fileStream); // Copy the stream to the file stream.
                }
        #else
                filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName); // Define the file path for Android, iOS, and Mac Catalyst.
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    saveStream.CopyTo(fileStream); // Copy the stream to the file stream.
                }
        #endif
                await Application.Current!.Windows[0].Page!.DisplayAlert("File Saved", filePath + " is successfully saved", "OK"); // Display a success message.       
            }
            catch (Exception ex)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlert( "File not saved", $"Error saving document: {ex.Message}", "OK"); // Display the error message if the file is not saved.
            }
        }
```

## Run the App

1. Build and run the application on all platforms.
2. Click the save button added in the toolbar.
3. Click the open button added in the toolbar




