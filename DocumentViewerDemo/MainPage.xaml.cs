using Syncfusion.Maui.PdfViewer;

namespace DocumentViewerDemo
{
    public partial class MainPage : ContentPage
    {
        // Create three instances of SfPdfViewer for displaying PDF documents
        SfPdfViewer pdfViewer = new SfPdfViewer();
        SfPdfViewer pdfViewer1 = new SfPdfViewer();
        SfPdfViewer pdfViewer2 = new SfPdfViewer();
        SfPdfViewer pdfViewer3 = new SfPdfViewer();
        SfPdfViewer pdfViewer4 = new SfPdfViewer();
        SfPdfViewer pdfViewer5 = new SfPdfViewer();
        public MainPage()
        {
            InitializeComponent();

            // Set the zoom mode of three PDF viewers to fit the width of the container
            pdfViewer.ZoomMode = ZoomMode.FitToWidth;
            pdfViewer1.ZoomMode = ZoomMode.FitToWidth;
            pdfViewer2.ZoomMode = ZoomMode.FitToWidth;
            pdfViewer3.ZoomMode = ZoomMode.FitToWidth;
            pdfViewer4.ZoomMode = ZoomMode.FitToWidth;
            pdfViewer5.ZoomMode = ZoomMode.FitToWidth;
        }

        async void SfTabView_Loaded(System.Object sender, System.EventArgs e)
        {
            // Ensure the viewModel is not null before proceeding
            if (viewModel?.PDFDocuments != null)
            {
                // Check if the first tab's header matches "PDF_Succinctly.pdf"
                if (tab1.Header.Equals("PDF_Succinctly.pdf"))
                {
                    await Task.Delay(80); // Slight delay to ensure UI is ready
                    pdfViewer.DocumentSource = viewModel.PDFDocuments[0]; // Assign the stream to the "DocumentSource" property of the PdfViewer control
                    tab1.Content = pdfViewer; // Set the content of tab1 to the pdfViewer.
                }

                // Check if the second tab's header matches "Input.docx"
                if (tab2.Header.Equals("Input.docx"))
                {
                    await Task.Delay(80); // Slight delay to ensure UI is ready
                    pdfViewer1.DocumentSource = viewModel.ConvertDocxToPdf(viewModel.PDFDocuments[1]); // Assign the stream to the "DocumentSource" property of the PdfViewer control
                    tab2.Content = pdfViewer1; // Set the content of tab1 to the pdfViewer1.
                }

                // Check if the third tab's header matches "Autumn Leaves.jpg"
                if (tab3.Header.Equals("Autumn Leaves.jpg"))
                {
                    await Task.Delay(80); // Slight delay to ensure UI is ready
                    pdfViewer2.DocumentSource = viewModel.ConvertImageToPdf(viewModel.PDFDocuments[2]); // Assign the stream to the "DocumentSource" property of the PdfViewer control
                    tab3.Content = pdfViewer2; // Set the content of tab1 to the pdfViewer1.
                }


                // Check if the fourth tab's header matches "InputTemplate.xlsx"
                if (tab4.Header.Equals("InputTemplate.xlsx"))
                {
                    await Task.Delay(80); // Slight delay to ensure UI is ready
                    pdfViewer3.DocumentSource = viewModel.ConvertXlsxToPdf(viewModel.PDFDocuments[3]); // Assign the stream to the "DocumentSource" property of the PdfViewer control
                    tab4.Content = pdfViewer3; // Set the content of tab1 to the pdfViewer1.
                }


                // Check if the fifth tab's header matches "Input.xps"
                if (tab5.Header.Equals("Input.xps"))
                {
                    await Task.Delay(80); // Slight delay to ensure UI is ready
                    pdfViewer4.DocumentSource = viewModel.ConvertXpsToPdf(viewModel.PDFDocuments[4]); // Assign the stream to the "DocumentSource" property of the PdfViewer control
                    tab5.Content = pdfViewer4; // Set the content of tab1 to the pdfViewer1.
                }

                // Check if the sixth tab's header matches "Template.pptx"
                if (tab6.Header.Equals("Template.pptx"))
                {
                    await Task.Delay(80); // Slight delay to ensure UI is ready
                    pdfViewer5.DocumentSource = viewModel.ConvertPptToPdf(viewModel.PDFDocuments[5]); // Assign the stream to the "DocumentSource" property of the PdfViewer control
                    tab6.Content = pdfViewer5; // Set the content of tab1 to the pdfViewer1.
                }
            }
        }
    }
}
