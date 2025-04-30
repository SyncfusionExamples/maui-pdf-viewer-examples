using System.Reflection;

namespace SharePdfThroughEmail
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            // Load the PDF document into the PDF Viewer using the DocumentSource property
            pdfViewer.DocumentSource = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("SharePdfThroughEmail.Assets.PDF_Succinctly.pdf");            
        }

        // Event handler for sharing the PDF through email when the associated button is clicked.
        private async void SharePDFThroughMail_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Define the file name and path for the saved PDF document.
                string fileName = Path.Combine(FileSystem.Current.AppDataDirectory, "ModifiedDocument.pdf");

                // Save the PDF document to the specified file location.
                using (FileStream pdfStream = File.Create(fileName))
                {
                    // Using pdfViewer instance to save the PDF document to a stream.
                    pdfViewer.SaveDocument(pdfStream);
                }

                // Verify if the file exists at the specified path.
                if (File.Exists(fileName))
                {
                    // Define a temporary file path for the PDF content to be used for email attachment.
                    var tempFilePath = Path.Combine(FileSystem.CacheDirectory, "ModifiedDocument.pdf");

                    // Copy the PDF content to the temporary file to prepare for email attachment.
                    File.Copy(fileName, tempFilePath, true);

                    // Check if email composition is supported on the device.
                    if (Email.Default.IsComposeSupported)
                    {
                        // Define the email's subject and body text.
                        string subject = "PDF Document Shared through MAUI SfPdfViewer";
                        string body = "Please find the PDF document attached.";

                        // Create the email message with the subject, body, and recipient.
                        var message = new EmailMessage
                        {
                            Subject = subject,
                            Body = body,
                            BodyFormat = EmailBodyFormat.PlainText,
                            To = new List<string> { "recipient@example.com" } // Placeholder recipient email
                        };

                        // Attach the modified PDF file to the email message.
                        message.Attachments?.Add(new EmailAttachment(tempFilePath));

                        // Open the default mail application to compose the email with the attached PDF.
                        await Email.Default.ComposeAsync(message);
                    }
                    else
                    {
                        // Display an alert if email composition is not supported on the device.
                        await DisplayAlert("Email Not Supported", "This device does not support email composition.", "OK");
                    }
                }
                else
                {
                    // Display an alert if the PDF file does not exist.
                    await DisplayAlert("File Error", "The modified PDF file could not be located.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions and display any error messages to the user.
                await DisplayAlert("An error occurred", ex.Message, "OK");
            }
        }
    }
}