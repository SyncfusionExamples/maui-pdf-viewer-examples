using CommunityToolkit.Maui.Storage;
using Syncfusion.Maui.PdfViewer;
using Syncfusion.Pdf.Security;
using System.Reflection;
namespace DigitalSignature
{
    public partial class MainPage : ContentPage
    {
        string? currentFileName = "form_document.pdf";
        public Stream? CertificateStream { get; set; }
        public string? CertificatePassword { get; set; }
        public string? Location { get; set; }
        public string? Reason { get; set; }
        public string? ContactInfo { get; set; }        
        private const string DefaultCertificatePassword = "password123";
        public MainPage()
        {
            InitializeComponent();
           
            pdfViewer.DigitalSignatureSettings.EnableValidation = true;
            pdfViewer.DigitalSignatureSettings.IsValidationBannerVisible = true;
            pdfViewer.DigitalSignatureSettings.EnableSigning = true;
            pdfViewer.DigitalSignatureModalViewAppearing += PdfViewer_DigitalSignatureModalViewAppearing;
            pdfViewer.SigningFailed += PdfViewer_SigningFailed;
        }

        private async void PdfViewer_SigningFailed(object? sender, SigningFailedEventArgs e)
        {
            string message =
                $"Error Type: {e.ErrorType}\n\n" +
                $"Message: {e.ErrorMessage}";

            if (e.InnerException != null)
            {
                message += $"\n\nException: {e.InnerException.Message}";
            }

            await DisplayAlertAsync("Signing Failed", message, "OK");
        }

        private void PdfViewer_DigitalSignatureModalViewAppearing(object? sender, DigitalSignatureModalViewAppearingEventArgs e)
        {
            // Use the user-selected certificate if one was picked; otherwise fall back
            // to the certificate.pfx bundled with the SampleBrowser.
            Stream? certificateStream = CertificateStream;
            string? certificatePassword = CertificatePassword;

            if (certificateStream == null || string.IsNullOrWhiteSpace(certificatePassword))
            {
                certificateStream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("DigitalSignature.Assets.certificate.pfx");
                certificatePassword = DefaultCertificatePassword;
            }

            e.Options = new SigningOptions
            {
                SignatureField = e.SignatureField,
                CertificateStream = certificateStream,
                CertificatePassword = certificatePassword,
                Reason = Reason,
                LocationInfo = Location,
                ContactInfo = ContactInfo,
                DigestAlgorithm = DigestAlgorithm.SHA256,
                CryptographicStandard = CryptographicStandard.CADES,
                Appearance = new SignatureAppearanceSettings
                {
                    ShowSignerName = true,
                    ShowDate = true,
                    ShowReason = true,
                    ShowLocation = true
                }
            };

        }

        /// <summary>
        /// Handles when leaving the current page
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            pdfViewer?.UnloadDocument();
            pdfViewer?.Handler?.DisconnectHandler();
        }

        private async void OnOpenClicked(object? sender, EventArgs? e)
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
                PickerTitle = "Choose a PDF file",
                FileTypes = pdfFileType,
            };

            // Launch the file picker and wait for user selection.
            var result = await FilePicker.Default.PickAsync(options);

            // Check if a file was selected.
            if (result != null)
            {
                // Ensure the file has a name.
                if (result.FileName != null)
                {
                    // Validate the file extension (case-insensitive).
                    if (result.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        currentFileName = result.FileName;
                        // Get the stream.
                        Stream pdfStream = await result.OpenReadAsync();

                        // Load the Pdf in the PdfViewer control using LoadDocument method
                        pdfViewer.LoadDocument(pdfStream);
                    }
                }
            }

        }

        private void OnDigitalSignatureClicked(object? sender, EventArgs? e)
        {
            SignatureDialogControl.IsVisible = true;
        }

        private async void OnSaveClicked(object? sender, EventArgs? e)
        {
            // Create a new memory stream to hold the saved PDF document
            Stream saveDocumentStream = new MemoryStream();

            // Asynchronously save the current document content into the memory stream
            await pdfViewer.SaveDocumentAsync(saveDocumentStream);
            saveDocumentStream.Position = 0;

            // Save the pdf in the local storage using SaveAsync method in the `FileSaver` class present in the CommunityToolkit.Maui source.

            if (!string.IsNullOrEmpty(currentFileName))
            {
                var fileSaverResult = await FileSaver.Default.SaveAsync(currentFileName,saveDocumentStream);

                if (fileSaverResult.IsSuccessful)
                {
                    string filePath = string.IsNullOrEmpty(fileSaverResult.FilePath)
                        ? "Path information is not available."
                        : fileSaverResult.FilePath;

                    await DisplayAlertAsync(
                        "File Saved",
                        $"The file is saved to:\n{filePath}",
                        "OK");
                }
                else
                {
                    await DisplayAlertAsync(
                        "Error",
                        fileSaverResult.Exception?.Message ?? "Save failed.",
                        "OK");
                }
            }

        }

        private void OnSignaturePanelClicked(object? sender, EventArgs? e)
        {
            pdfViewer.DigitalSignatureSettings.IsSignaturePanelVisible = !pdfViewer.DigitalSignatureSettings.IsSignaturePanelVisible;        
        }

        private void OnSignatureApplied(object? sender, SignatureDialogEventArgs? e)
        {
            if (e == null)
            {
                return;
            }
            if (e.CertificateStream == null || string.IsNullOrEmpty(e.CertificatePassword))
            {
                Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", "Please select a certificate and enter the password.", "OK");
                return;
            }
            CertificateStream = e.CertificateStream;
            CertificatePassword = e.CertificatePassword;
            Location = e.Location;
            Reason = e.Reason;
            ContactInfo = e.ContactInfo;
        }
    }
}
