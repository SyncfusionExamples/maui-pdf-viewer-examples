using Syncfusion.Maui.PdfViewer;

namespace SharePDF
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            PdfViewer.DocumentSource = this.GetType().Assembly.GetManifestResourceStream("SharePDF.Assets.PdfSuccinctly.pdf");
        }

        private async void SharePdf_Clicked(object sender, EventArgs e)
        {
            try
            {
                string fileName = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "ModifiedDocument.pdf");

                // Save the PDF document to the specified file
                using (FileStream pdfStream = File.Create(fileName))
                {
                    // Assuming pdfViewer is an instance of a class that can save the PDF document
                    PdfViewer.SaveDocument(pdfStream);
                }

                // Check if the file exists
                if (File.Exists(fileName))
                {
                    // Create a temporary file with the PDF content
                    var tempFilePath = System.IO.Path.Combine(FileSystem.CacheDirectory, "ModifiedDocument.pdf");

                    // Copy the PDF content to the temporary file
                    File.Copy(fileName, tempFilePath, true);

                    // Create a ShareFileRequest with the file path
                    var request = new ShareFileRequest
                    {
                        Title = "Share PDF File",
                        File = new ShareFile(tempFilePath)
                    };

                    // Call the Share API
                    await Microsoft.Maui.ApplicationModel.DataTransfer.Share.RequestAsync(request);
                }
                else
                {
                    await DisplayAlert("Error", "PDF file not found.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

}
