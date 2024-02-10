namespace SharePdfDocumentToDefaultMail
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            PdfViewer.DocumentSource = this.GetType().Assembly.GetManifestResourceStream("SharePdfDocumentToDefaultMail.Assets.PdfSuccinctly.pdf");
        }

        private async void SharePdfToDefaultMail_Clicked(object sender, EventArgs e)
        {
            string fileName = Path.Combine(FileSystem.Current.AppDataDirectory, "PDF_FromSfPdfViewer.pdf");

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
                var tempFilePath = Path.Combine(FileSystem.CacheDirectory, "PDF_FromSfPdfViewer.pdf");

                // Copy the PDF content to the temporary file
                File.Copy(fileName, tempFilePath, true);

                // Share the PDF via email
                if (Email.Default.IsComposeSupported)
                {
                    string subject = "PDF document from MAUI SfPdfViewer";
                    string body = "Check out the Pdf document";

                    var message = new EmailMessage
                    {
                        Subject = subject,
                        Body = body,
                        BodyFormat = EmailBodyFormat.PlainText,
                        To = new List<string> { "recipient@example.com" }
                    };

                    // Attach the modified PDF file to the email
                    message.Attachments?.Add(new EmailAttachment(tempFilePath));

                    await Email.ComposeAsync(message);
                }
            }
        }
    }

}
