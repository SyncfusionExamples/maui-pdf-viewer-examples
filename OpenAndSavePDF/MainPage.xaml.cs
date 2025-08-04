namespace OpenAndSavePDF
{
    public partial class MainPage : ContentPage
    {
        // Define events for saving the document
        public event EventHandler<DocumentSaveEventArgs>? DocumentSaveInitiated;
        public MainPage()
        {
            InitializeComponent();

            // Subscribe to DocumentSaveInitiated event
            this.DocumentSaveInitiated += MainPage_DocumentSaveInitiated;

            AddSaveToolbarItem();
            AddOpenFileToolbarItem();
        }

        void AddSaveToolbarItem()
        {
            // Create new save button
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

            // Set tooltip text for the button
            ToolTipProperties.SetText(fileSaveButton, "Save");

            // Subscribe to the Clicked event to handle save logic when clicked
            fileSaveButton.Clicked += FileSaveButton_Clicked;
#if !WINDOWS && !MACCATALYST

            // Insert the button into the "TopToolbar" on Android/iOS platforms
            pdfViewer?.Toolbars?.GetByName("TopToolbar")?.Items?.Insert(0, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileSaveButton, "FileSaveButton"));
#else
            // Insert the button into the "PrimaryToolbar" for Windows/macOS platforms
            pdfViewer?.Toolbars?.GetByName("PrimaryToolbar")?.Items?.Insert(0, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileSaveButton, "FileSaveButton"));
#endif
        }

        void AddOpenFileToolbarItem()
        {
            // Create new open button
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

            // Set tooltip text for the button
            ToolTipProperties.SetText(fileOpenButton, "Open");


            // Subscribe to the Clicked event to handle file open logic when clicked
            fileOpenButton.Clicked += FileOpenButton_Clicked;

#if !WINDOWS && !MACCATALYST

            // Insert the button into the "TopToolbar" on Android/iOS platforms
            pdfViewer?.Toolbars?.GetByName("TopToolbar")?.Items?.Insert(1, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileOpenButton, "FileOpenButton"));
#else
            // Insert the button into the "PrimaryToolbar" for Windows/macOS platforms
            pdfViewer?.Toolbars?.GetByName("PrimaryToolbar")?.Items?.Insert(1, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileOpenButton, "FileOpenButton"));
#endif
        }

        // Handle open file button click
        private void FileOpenButton_Clicked(object? sender, EventArgs e)
        {
            var viewModel = (PdfViewerViewModel)BindingContext;
            viewModel.OpenDocument();
        }

        // This method is called before saving starts
        private void MainPage_DocumentSaveInitiated(object? sender, DocumentSaveEventArgs e)
        {
            var viewModel = (PdfViewerViewModel)BindingContext;

            // Calling the save logic method by passing the saved stream as it parameter.
            viewModel.SavePDFDocument(e.SaveStream);
        }

        // Handle file save button click
        private async void FileSaveButton_Clicked(object? sender, EventArgs e)
        {

            // Create a new memory stream to hold the saved PDF document
            MemoryStream saveStream = new MemoryStream();

            // Asynchronously save the current document content into the memory stream
            await pdfViewer.SaveDocumentAsync(saveStream);

            // Trigger the DocumentSaveInitiated event
            DocumentSaveInitiated?.Invoke(this, new DocumentSaveEventArgs(saveStream));

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
