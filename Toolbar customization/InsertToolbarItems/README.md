# How to include Open and Save options in the built-in toolbar?

This project demonstrate how to insert the save and open options in the toolbar in [SfPdfViewer](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html) and how to create and it also showcases how to create and handle a custom DocumentSaveInitiated event.

## Prerequisites

1. A .NET MAUI project set up.
2. The [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer) package is installed.

## To Include open and save option in built-in toolbar

To insert an item at a specific index in the toolbar in [SfPdfViewer](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html), begin by creating the desired UI element. Next, convert that element into a [ToolbarItem](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.ToolbarItem.html#Syncfusion_Maui_PdfViewer_ToolbarItem__ctor_Microsoft_Maui_Controls_View_System_String) using the ToolbarItem method. Finally, add the newly created ToolbarItem to the toolbar using the [Insert](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.ToolbarItemCollection.html#Syncfusion_Maui_PdfViewer_ToolbarItemCollection_Insert_System_Int32_Syncfusion_Maui_PdfViewer_ToolbarItem_) or [Add](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.ToolbarItemCollection.html#Syncfusion_Maui_PdfViewer_ToolbarItemCollection_Add_Syncfusion_Maui_PdfViewer_ToolbarItem_) method. Here we are using [Insert](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.ToolbarItemCollection.html#Syncfusion_Maui_PdfViewer_ToolbarItemCollection_Insert_System_Int32_Syncfusion_Maui_PdfViewer_ToolbarItem_) method to include the open and save button in the toolbar item.

### Step 1: Define the methods for Open and Save.

Definition for the open and save logic method.

**C#**

```csharp
async Task<Stream?> OpenFileAsync() {}
async Task SaveFileAsync() {}
```

### Step 2: Implement Open and Save Logic

In the `OpenFileAsync` method, platform-specific file type filters are defined to ensure the file picker displays only compatible PDF files across different operating systems. The file picker is then configured with a custom title and the appropriate file type settings. Once launched, it waits for the user to select a PDF file. After selection, the application opens a read stream from the chosen file, allowing access to its contents. Finally, the method returns this stream for further use.

**C#**

```csharp
        private async Task<Stream?> OpenFileAsync()
        {
            // Define platform-specific file types for the file picker.
            FilePickerFileType pdfFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>{
                        { DevicePlatform.iOS, new[] { "com.adobe.pdf" } },
                        { DevicePlatform.Android, new[] { "application/pdf" } },
                        { DevicePlatform.WinUI, new[] { "pdf" } },
                        { DevicePlatform.MacCatalyst, new[] { "pdf" } },
                    });

            // Configure the file picker options.
            PickOptions options = new()
            {
                PickerTitle = "Please select a PDF file",
                FileTypes = pdfFileType
            };

            // Launch the file picker and wait for user selection.
            var result = await FilePicker.Default.PickAsync(options);

            // Check if a file was selected.
            if (result != null)
            {
                Stream stream = await result.OpenReadAsync();
                return stream;
                // Load stream into viewer or process content
            }
        }
```

In the save file method, get memory stream to save the document and the document is saved in the given stream using the [SaveDocumentAsync](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html#Syncfusion_Maui_PdfViewer_SfPdfViewer_SaveDocumentAsync_System_IO_Stream_System_Threading_CancellationToken_) method in the [SfPdfViewer](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html).

**C#**

```csharp
        private async void SaveFileAsync()
        {
            // Create a new memory stream to hold the saved PDF document
            Stream savedStream = new MemoryStream();

            // Asynchronously save the current document content into the memory stream
            await pdfViewer.SaveDocumentAsync(savedStream);
        }

```


### Step 3: Create open and save button and implement event handlers.

Create "Open" and "Save" buttons, and attach their respective click event handlers.

**Open Button**

### Create new open button.

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

### Event handler of open button.

**C#**

In the open button event handler, get the stream using `OpenFileAsync` and loaded into the viewer by passing the stream in the [LoadDocumentAsync](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html#Syncfusion_Maui_PdfViewer_SfPdfViewer_LoadDocumentAsync_System_IO_Stream_System_String_System_Nullable_Syncfusion_Maui_PdfViewer_FlattenOptions__System_Threading_CancellationTokenSource_) method in the [SfPdfViewer](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html).

```csharp
        private async void FileOpenButton_Clicked(object? sender, EventArgs e)
        {
            // Choose the pdf file using file picker option and convert the selected pdf to stream.
            Stream? fileStream = await FileService.OpenFileAsync();
            if (fileStream != null)
            {
                // Passing the stream in the "LoadDocumentAsync" method to load the pdf.
                await pdfViewer.LoadDocumentAsync(fileStream);
            }
        }
```

**Save Button**

### Create new open button.

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

### Event handler of save button.

**C#**

In the save button event handler, Call the `SaveFileAsync` method

```csharp
        private async void FileOpenButton_Clicked(object? sender, EventArgs e)
        {
            SaveFileAsync();
        }
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





