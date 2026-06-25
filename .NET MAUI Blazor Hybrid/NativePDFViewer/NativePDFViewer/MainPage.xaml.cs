using MauiApp5.PdfViewer;

namespace MauiApp5
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Opens a PDF file in the native MAUI PDF viewer.
        /// Called from Blazor pages via JS interop or dependency injection.
        /// </summary>
        public async Task OpenPdfAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                await DisplayAlertAsync("Error", "PDF file not found.", "OK");
                return;
            }

            var pdfPage = new PdfViewerPage(filePath);
            await Navigation.PushAsync(pdfPage);
        }

        /// <summary>
        /// Opens a PDF file picker and displays the selected PDF in the native viewer.
        /// </summary>
        public async Task PickAndOpenPdfAsync()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a PDF document",
                    FileTypes = FilePickerFileType.Pdf
                });

                if (result != null)
                {
                    await OpenPdfAsync(result.FullPath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Failed to pick PDF: {ex.Message}", "OK");
            }
        }
    }
}
