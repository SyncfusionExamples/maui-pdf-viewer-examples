using System.Reflection;

namespace InsertToolbarItems
{
    public partial class MainPage : ContentPage
    {
        // Define events for saving the document
        public event EventHandler<DocumentSaveEventArgs>? DocumentSaveInitiated;
        string currentFileName = "PDF_Succinctly.pdf";

        public MainPage()
        {
            InitializeComponent();

            //Accessing the PDF document that is added as embedded resource as stream.
            Stream? DocumentStream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("InsertToolbarItems.Assets.PDF_Succinctly.pdf");

            // Assigning stream to "DocumentSource" property.
            pdfViewer.DocumentSource = DocumentStream;

            // Subscribe to DocumentSaveInitiated event
            this.DocumentSaveInitiated += MainPage_DocumentSaveInitiated;

            // Calling "AddFileOperationsToolbarItems" for to add save and open options toolbar item in toolbar
            AddFileOperationsToolbarItems();
        }
        void AddFileOperationsToolbarItems()
        {
            // Create new open button
            Button fileOpenButton = new Button
            {
                Text = "\ue712", // Set button text
                FontSize = 24, // Set button text font size
                FontFamily = "MauiMaterialAssets", // Set button text font family
                BackgroundColor = Colors.Transparent, // Set background for the button
                BorderColor = Colors.Transparent, // Set border color for the button
                TextColor = Color.FromArgb("#49454F"), // Set button text color
                CornerRadius = 5 // Set corner radius of the button
            };

            //Subscription of click event for the open file button
            fileOpenButton.Clicked += FileOpenButton_Clicked;

            // Set the tooltip text on hover
            ToolTipProperties.SetText(fileOpenButton, "Open File");

            // Create new save button
            Button fileSaveButton = new Button
            {
                Text = "\ue75f", // Set button text
                FontSize = 24, // Set button text font size
                FontFamily = "MauiMaterialAssets", // Set button text font family
                BackgroundColor = Colors.Transparent, // Set background for the button
                BorderColor = Colors.Transparent, // Set border color for the button
                TextColor = Color.FromArgb("#49454F"), // Set button text color
                CornerRadius = 5 // Set corner radius of the button
            };

            // Subscription of click event for the save button
            fileSaveButton.Clicked += FileSaveButton_Clicked;

            // Set the tooltip text on hover
            ToolTipProperties.SetText(fileSaveButton, "Save");

#if !WINDOWS && !MACCATALYST
            // Inserting open file option button as toolbar item in the top toolbar for the mobile platform.
            pdfViewer?.Toolbars?.GetByName("TopToolbar")?.Items?.Insert(0, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileOpenButton, "FileOpenButton"));
            // Inserting save file option button as toolbar item in the top toolbar for the mobile platform.
            pdfViewer?.Toolbars?.GetByName("TopToolbar")?.Items?.Insert(1, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileSaveButton, "FileSaveButton"));
#else
            // Inserting open file option button as toolbar item in the primary toolbar for the desktop platform.
            pdfViewer?.Toolbars?.GetByName("PrimaryToolbar")?.Items?.Insert(0, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileOpenButton, "FileOpenButton"));
            // Inserting save file option button as toolbar item in the top toolbar for the desktop platform.
            pdfViewer?.Toolbars?.GetByName("PrimaryToolbar")?.Items?.Insert(1, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileSaveButton, "FileSaveButton"));
#endif
        }
 
        private async void FileSaveButton_Clicked(object? sender, EventArgs e)
        {
            // Create a new memory stream to hold the saved PDF document
            Stream savedStream = new MemoryStream();

            // Asynchronously save the current document content into the memory stream
            await pdfViewer.SaveDocumentAsync(savedStream);

            // Trigger the DocumentSaveInitiated event
            DocumentSaveInitiated?.Invoke(this, new DocumentSaveEventArgs(savedStream));
        }

        private void MainPage_DocumentSaveInitiated(object? sender, DocumentSaveEventArgs e)
        {
            // Calling "SavePDFDocument" by passing the saved stream as parameter to save the pdf in local storage.
            SavePDFDocument(e.SaveStream);
        }

        private async void FileOpenButton_Clicked(object? sender, EventArgs e)
        {
            // Choose the pdf file using file picker option and convert the selected pdf to stream.
            PdfFileData? fileData = await FileService.OpenFileRequest("pdf");
            if (fileData != null)
            {
                currentFileName = fileData.FileName;

                // Passing the stream in the "LoadDocumentAsync" method to load the pdf.
                await pdfViewer.LoadDocumentAsync(fileData.Stream);
            }
        }
        private async void SavePDFDocument(Stream saveStream)
        {
            if (!string.IsNullOrEmpty(currentFileName))
            {
                try
                {
                    saveStream.Position = 0;

                    // Open the file explorer using the file picker, select a location to save the PDF, save the file to the chosen path, and retrieve the saved file location for display.
                    string ? filePath = await FileService.SaveAsAsync(currentFileName, saveStream);

                    // Safely storing the main page of the application.
                    var mainPage = Application.Current?.Windows[0].Page;

                    if(mainPage != null)
                        // Display the saved file path.
                        await mainPage.DisplayAlert("File saved", $"The file is saved to {filePath}", "OK");
                }
                catch (Exception exception)
                {
                    // Safely storing the main page of the application.
                    var mainPage = Application.Current?.Windows[0].Page;

                    if(mainPage != null)
                        // Display the error message when the file is not saved.
                        await mainPage.DisplayAlert("Error", $"The file is not saved. {exception.Message}", "OK");
                }
            }
        }
    }

    // Custom event arguments class to pass save stream
    public class DocumentSaveEventArgs : EventArgs
    {
        public Stream SaveStream { get; }

        public DocumentSaveEventArgs(Stream saveStream)
        {
            SaveStream = saveStream;
        }
    }
}
