using Syncfusion.Maui.PdfViewer;
using System.Reflection;

namespace AddImagesToPDF
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            // Load the PDF document from embedded resources
            pdfViewer.DocumentSource = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("AddImagesToPDF.Assets.FormFillingDocument.pdf");
        }

        /// <summary>
        /// Event handler for the signature panel button click.
        /// Opens the signature modal view for adding signatures to the PDF.
        /// </summary>
        private void OpenSignaturePanel_Clicked(object sender, EventArgs e)
        {
            // Open the signature modal view programmatically
            pdfViewer.AnnotationMode = Syncfusion.Maui.PdfViewer.AnnotationMode.Signature;
        }

        /// <summary>
        /// Event handler for adding an image as a custom stamp to the PDF.
        /// Calculates the position and adds an "Approved" stamp to the document.
        /// </summary>
        private void AddImageusingCustomStamp_Clicked(object sender, EventArgs e)
        {
            int pageNumber = 1;

            RectF bounds;

            double stampWidth = 200;
            double stampHeight = 50;

            // Calculate the center of the view port to place the stamp. if size information is available.
            if (pdfViewer.Width > 0 && pdfViewer.Height > 0)
            {
                Point centerPoint = pdfViewer.ConvertClientPointToPagePoint(new Point((pdfViewer.Width - stampWidth) / 2, (pdfViewer.Height - stampHeight) / 2), pageNumber);
                bounds = new RectF((float)centerPoint.X, (float)centerPoint.Y, (float)stampWidth, (float)stampHeight);
            }
            else // Define the position and size for the stamp to be placed in the PDF page.
                bounds = new RectF(50, 50, (float)stampWidth, (float)stampHeight);

            // Create image stream from the image to be used as stamp.
            Stream? imageStream = this.GetType().Assembly.GetManifestResourceStream("AddImagesToPDF.Assets." + "Approved.png");

            if (imageStream != null)
            {
                // Create a custom stamp annotation using the image stream.
                StampAnnotation customStamp = new StampAnnotation(imageStream, pageNumber, bounds);

                // // Check if flattening option is selected and enable flattening to make the stamp permanent when saving
                if (FlattenOnSave.IsChecked)
                    customStamp.FlattenOnSave = true;

                // Add the stamp annotation to the PDF document
                pdfViewer.AddAnnotation(customStamp);
            }
        }
    }
}