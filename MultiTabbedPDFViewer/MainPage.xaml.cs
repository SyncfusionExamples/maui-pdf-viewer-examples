using Syncfusion.Maui.PdfViewer;

namespace MultiTabbedPDFViewer
{
    public partial class MainPage : ContentPage
    {
        // Create two instances of SfPdfViewer for displaying PDF documents
        SfPdfViewer pdfViewer = new SfPdfViewer();
        SfPdfViewer pdfViewer1 = new SfPdfViewer();

        public MainPage()
        {
            InitializeComponent();

            // Set the zoom mode of both PDF viewers to fit the width of the container
            pdfViewer.ZoomMode = ZoomMode.FitToWidth;
            pdfViewer1.ZoomMode = ZoomMode.FitToWidth;
        }

        async void SfTabView_Loaded(System.Object sender, System.EventArgs e)
        {
            // Ensure the viewModel is not null before proceeding
            if (viewModel != null)
            {
                // Check if the first tab's header matches "doc 1"
                if (tab1.Header.Equals("doc 1"))
                {
                    await Task.Delay(80); // Slight delay to ensure UI is ready
                    pdfViewer.DocumentSource = viewModel.PDFDocumentSource; // Assign the stream to the "DocumentSource" property of the PdfViewer control
                    tab1.Content = pdfViewer; // Set the content of tab1 to the pdfViewer.
                }

                // Check if the first tab's header matches "doc 2"
                if (tab2.Header.Equals("doc 2"))
                {
                    await Task.Delay(80); // Slight delay to ensure UI is ready
                    pdfViewer1.DocumentSource = viewModel.PDFDocumentSource2; // Assign the stream to the "DocumentSource" property of the PdfViewer control
                    tab2.Content = pdfViewer1; // Set the content of tab1 to the pdfViewer1.
                }
            }

        }
    }
}
