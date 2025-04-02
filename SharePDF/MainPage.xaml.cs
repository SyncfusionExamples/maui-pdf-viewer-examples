namespace SharePDF
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            // Load the PDF into the PDF Viewer from embedded resources
            pdfViewer.DocumentSource = this.GetType().Assembly.GetManifestResourceStream("SharePDF.Assets.PDF_Succinctly1.pdf");
        }

        // Event handler for sharing the PDF when the share button is clicked
        private async void SharePdf_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Define the file path in the application's data directory to save the PDF
                string fileName = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "ModifiedDocument.pdf");

                // Save the PDF document to the specified file
                using (FileStream pdfStream = File.Create(fileName))
                {
                    // Save the document from the PDF viewer to the file stream
                    pdfViewer.SaveDocument(pdfStream);
                }

                // Check if the PDF file was saved successfully
                if (File.Exists(fileName))
                {
                    // Define a temporary file path in the cache directory
                    var tempFilePath = System.IO.Path.Combine(FileSystem.CacheDirectory, "ModifiedDocument.pdf");

                    // Copy the saved PDF to the temporary file for sharing
                    File.Copy(fileName, tempFilePath, true);

                    // Create a ShareFileRequest to facilitate sharing the PDF
                    var request = new ShareFileRequest
                    {
                        Title = "Share PDF File",
                        File = new ShareFile(tempFilePath)
                    };

                    // Invoke the Share API to share the PDF with other apps
                    await Microsoft.Maui.ApplicationModel.DataTransfer.Share.RequestAsync(request);
                }
                else
                {
                    // Display an error message if the file does not exist
                    await DisplayAlert("Error", "PDF file not found.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Display an error message for any exceptions that occur during the process
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}