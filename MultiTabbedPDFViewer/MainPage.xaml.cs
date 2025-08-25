using Syncfusion.Maui.PdfViewer;

namespace MultiTabbedPDFViewer
{
    public partial class MainPage : ContentPage
    {
        // Create three instances of SfPdfViewer for displaying PDF documents
        SfPdfViewer pdfViewer = new SfPdfViewer();
        SfPdfViewer pdfViewer1 = new SfPdfViewer();
        SfPdfViewer pdfViewer2 = new SfPdfViewer();

        public MainPage()
        {
            InitializeComponent();

            // Set the zoom mode of three PDF viewers to fit the width of the container
            pdfViewer.ZoomMode = ZoomMode.FitToWidth;
            pdfViewer1.ZoomMode = ZoomMode.FitToWidth;
            pdfViewer2.ZoomMode = ZoomMode.FitToWidth;
        }

        async void SfTabView_Loaded(System.Object sender, System.EventArgs e)
        {
            // Ensure the viewModel is not null before proceeding
            if (viewModel?.PDFDocuments != null)
            {
                // Check if the first tab's header matches "doc 1"
                if (tab1.Header.Equals("doc 1"))
                {
                    await Task.Delay(80); // Slight delay to ensure UI is ready
                    pdfViewer.DocumentSource = viewModel.PDFDocuments[0]; // Assign the stream to the "DocumentSource" property of the PdfViewer control
                    tab1.Content = pdfViewer; // Set the content of tab1 to the pdfViewer.
                }

                // Check if the second tab's header matches "doc 2"
                if (tab2.Header.Equals("doc 2"))
                {
                    await Task.Delay(80); // Slight delay to ensure UI is ready
                    pdfViewer1.DocumentSource = viewModel.PDFDocuments[1]; // Assign the stream to the "DocumentSource" property of the PdfViewer control
                    tab2.Content = pdfViewer1; // Set the content of tab1 to the pdfViewer1.
                }

                // Check if the third tab's header matches "doc 3"
                if (tab3.Header.Equals("doc 3"))
                {
                    await Task.Delay(80); // Slight delay to ensure UI is ready
                    pdfViewer2.DocumentSource = viewModel.PDFDocuments[2]; // Assign the stream to the "DocumentSource" property of the PdfViewer control
                    tab3.Content = pdfViewer2; // Set the content of tab1 to the pdfViewer1.
                }
            }
        }
    }
}
