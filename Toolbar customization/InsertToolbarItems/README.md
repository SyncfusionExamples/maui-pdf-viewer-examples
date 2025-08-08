# How to include Open and Save options in the built-in toolbar?

This project demonstrate how to insert the save and open options in the toolbar in [SfPdfViewer](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html) and how to create and it also showcases how to create and handle a custom DocumentSaveInitiated event.

## Prerequisites

1. A .NET MAUI project set up.
2. The [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer) package is installed.

## To Include open and save option in built-in toolbar

To insert an item at a specific index in the toolbar in [SfPdfViewer](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html), begin by creating the desired UI element. Next, convert that element into a [ToolbarItem](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.ToolbarItem.html#Syncfusion_Maui_PdfViewer_ToolbarItem__ctor_Microsoft_Maui_Controls_View_System_String) using the ToolbarItem method. Finally, add the newly created ToolbarItem to the toolbar using the [Insert](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.ToolbarItemCollection.html#Syncfusion_Maui_PdfViewer_ToolbarItemCollection_Insert_System_Int32_Syncfusion_Maui_PdfViewer_ToolbarItem_) or [Add](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.ToolbarItemCollection.html#Syncfusion_Maui_PdfViewer_ToolbarItemCollection_Add_Syncfusion_Maui_PdfViewer_ToolbarItem_) method. Here we are using [Insert](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.ToolbarItemCollection.html#Syncfusion_Maui_PdfViewer_ToolbarItemCollection_Insert_System_Int32_Syncfusion_Maui_PdfViewer_ToolbarItem_) method to include the open and save button in the toolbar item.

### Step 1: Definition and implementation the logic for the event handler of the save and open button.

In the save button click event handler, get memory stream to save the document and the document is saved in the given stream using the [SaveDocumentAsync](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html#Syncfusion_Maui_PdfViewer_SfPdfViewer_SaveDocumentAsync_System_IO_Stream_System_Threading_CancellationToken_) method in the [SfPdfViewer](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html).

**C#**

```csharp
        private async void FileSaveButton_Clicked(object? sender, EventArgs e)
        {
            // Create a new memory stream to hold the saved PDF document
            Stream savedStream = new MemoryStream();

            // Asynchronously save the current document content into the memory stream
            await pdfViewer.SaveDocumentAsync(savedStream);
        }

```

In the open button event handler, we use a file picker to allow users to select and open a PDF file from their device in the .NET MAUI PDF Viewer. The selected PDF is converted into a stream and loaded into the viewer by passing the stream in the [LoadDocumentAsync](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html#Syncfusion_Maui_PdfViewer_SfPdfViewer_LoadDocumentAsync_System_IO_Stream_System_String_System_Nullable_Syncfusion_Maui_PdfViewer_FlattenOptions__System_Threading_CancellationTokenSource_) method in the [SfPdfViewer](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html).

**C#**

```csharp
        private async void FileOpenButton_Clicked(object? sender, EventArgs e)
        {
            // Choose the pdf file using file picker option and convert the selected pdf to stream.
            PdfFileData? fileData = await FileService.OpenFile("pdf");
            if (fileData != null)
            {
                currentFileName = fileData.FileName;

                // Passing the stream in the "LoadDocumentAsync" method to load the pdf.
                await pdfViewer.LoadDocumentAsync(fileData.Stream);
            }
        }
```


### Step 2: Create open and save button and implement event handlers.

Create "Open" and "Save" buttons, and attach their respective click event handlers.

**Open Button**

**C#**

```csharp
        // Create new open button
        Button fileOpenButton = new Button
        {
            Text = "\ue712",
            FontSize = 24, 
            FontFamily = "MauiMaterialAssets",
            BackgroundColor = Colors.Transparent, 
            BorderColor = Colors.Transparent,
            TextColor = Color.FromArgb("#49454F"), 
            CornerRadius = 5 
        };

        //Subscription of click event for the open file button
        fileOpenButton.Clicked += FileOpenButton_Clicked;
```

**Save Button**

**C#**

```csharp
        // Create new save button
        Button fileSaveButton = new Button
        {
            Text = "\ue75f", 
            FontSize = 24, 
            FontFamily = "MauiMaterialAssets", 
            BackgroundColor = Colors.Transparent, 
            BorderColor = Colors.Transparent, 
            TextColor = Color.FromArgb("#49454F"), 
            CornerRadius = 5 
        };

        // Subscription of click event for the save button
        fileSaveButton.Clicked += FileSaveButton_Clicked;
```




### Step 3: Insert the created save and open button in the toolbar using toolbar names.

The created save and open button is inserted in the specific toolbar by the [Insert](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.ToolbarItemCollection.html#Syncfusion_Maui_PdfViewer_ToolbarItemCollection_Insert_System_Int32_Syncfusion_Maui_PdfViewer_ToolbarItem_) method using the [Mobile Toolbar Names](https://help.syncfusion.com/maui/pdf-viewer/toolbar#mobile-toolbar-names) and [Desktop Toolbar Names](https://help.syncfusion.com/maui/pdf-viewer/toolbar#desktop-toolbar-names).

**Desktop platforms**

**C#**

```csharp
            // Inserting open file option button as toolbar item in the primary toolbar for the desktop platform.
            pdfViewer?.Toolbars?.GetByName("PrimaryToolbar")?.Items?.Insert(0, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileOpenButton, "FileOpenButton"));
            // Inserting save file option button as toolbar item in the top toolbar for the desktop platform.
            pdfViewer?.Toolbars?.GetByName("PrimaryToolbar")?.Items?.Insert(1, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileSaveButton, "FileSaveButton"));
```

**Mobile platforms**

**C#**

```csharp
            // Inserting open file option button as toolbar item in the top toolbar for the mobile platform.
            pdfViewer?.Toolbars?.GetByName("TopToolbar")?.Items?.Insert(0, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileOpenButton, "FileOpenButton"));
            // Inserting save file option button as toolbar item in the top toolbar for the mobile platform.
            pdfViewer?.Toolbars?.GetByName("TopToolbar")?.Items?.Insert(1, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileSaveButton, "FileSaveButton"));
```

## To Create and handle custom DocumentSaveInitiated event.

### Step 1: Create a custom event argument class

Create a `DocumentSaveEventArgs` class that inherits from `EventArgs`. This class is intended to carry a stream where the document will be saved.

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
### Step 2: Document save process in the local storage.

Set the location path and create a file name to save the pdf document. Create a file in the location path specified, after that copy the provided stream into the file. Here we are set the location save the file using the `SaveAsAsync` method in `FileService` class, which has logic using the file picker option.

**C#**

```csharp
        private async void SavePDFDocument(Stream saveStream)
        {
            if (!string.IsNullOrEmpty(currentFileName))
            {
                try
                {
                    saveStream.Position = 0;

                    // Open the file explorer using the file picker, select a location to save the PDF, save the file to the chosen path, and retrieve the saved file location for display.
                    string? filePath = await FileService.SaveAsAsync(currentFileName, saveStream);

                    // Display the saved file path.
                    await Application.Current!.Windows[0].Page!.DisplayAlert("File saved", $"The file is saved to {filePath}", "OK");
                }
                catch (Exception exception)
                {
                    // Display the error message when the file is not saved.
                    await Application.Current!.Windows[0].Page!.DisplayAlert("Error", $"The file is not saved. {exception.Message}", "OK");
                }
            }
        }
```

### Step 3: Calling save logic method in the DocumentSaveInitiated event handler.

This `DocumentSaveInitiated` event handler designed to respond when a document saving process is initiated in the UI. Within this event handler, In this example, the PDF is saved by allowing the user to browse their local storage and choose a specific location for saving the file.

**C#**

```csharp
        // This method is called before saving starts
        private void MainPage_DocumentSaveInitiated(object? sender, DocumentSaveEventArgs e)
        {
            // Calling "SavePDFDocument" by passing the saved stream as parameter to save the pdf in local machine.
            SavePDFDocument(e.SaveStream);
        }
```

### Step 4: Subscription of DocumentSaveInitiated event.

Subscribe the `DocumentSaveInitiated` event in the MainPage constructor 

**C#**

```csharp
            // Subscribe to DocumentSaveInitiated event
            this.DocumentSaveInitiated += MainPage_DocumentSaveInitiated;
```

### Step 5: Invoke DocumentSaveInitiated event in the save button event handler.

In the save button click event handler, get memory stream to save the document and the document is saved in the given stream using the [SaveDocumentAsync](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html#Syncfusion_Maui_PdfViewer_SfPdfViewer_SaveDocumentAsync_System_IO_Stream_System_Threading_CancellationToken_) method in the [SfPdfViewer](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html). Construct a `DocumentSaveEventArgs` using the generated stream, and invoke the `DocumentSaveInitiated` event by passing the newly created argument.

**C#**

```csharp
        private async void FileSaveButton_Clicked(object? sender, EventArgs e)
        {
            // Create a new memory stream to hold the saved PDF document
            Stream savedStream = new MemoryStream();

            // Asynchronously save the current document content into the memory stream
            await pdfViewer.SaveDocumentAsync(savedStream);

            // Trigger the DocumentSaveInitiated event
            DocumentSaveInitiated?.Invoke(this, new DocumentSaveEventArgs(savedStream));
        }
```

## Run the App

1. Build and run the application on all platforms.
2. Click the save button added in the toolbar.
3. Click the open button added in the toolbar

## ScreenShots:

### Before changes:

**Default toolbar without save and open file option**

<img src="Images/Before Toolbar Item Insertion.png" Width="600"/>

### After changes:

**After insertion of save and open file option in the toolbar**

<img src="Images/After Toolbar Item Insertion.png" Width="600"/>





