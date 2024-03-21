using Syncfusion.Maui.PdfViewer;
using System.Reflection;

namespace ConversionOfCoordinates
{
    public partial class MainPage : ContentPage
    {
        Stream? stream;

        public MainPage()
        {
            InitializeComponent();
            // Load PDF document from embedded resource
            stream = typeof(MainPage).GetTypeInfo().Assembly.GetManifestResourceStream("ConversionOfCoordinates.Annotations.pdf");
            pdfViewer.LoadDocument(stream);
        }

        // Method to handle button click event
        private void BringAnnotationToViewClicked(object sender, EventArgs e)
        {
            int count = 0;

            // Iterate through annotations in PDF viewer
            foreach (var annotation in pdfViewer.Annotations)
            {
                // Check if annotation is a square annotation and it's the first one
                if (annotation is SquareAnnotation squareAnnotation && count == 0)
                {
                    count++;

                    // Convert annotation coordinates to scroll point coordinates
                    Point scrollPoint = pdfViewer.ConvertPagePointToScrollPoint(new Point(squareAnnotation.Bounds.X, squareAnnotation.Bounds.Y), 2);
                    Point endPoint = pdfViewer.ConvertPagePointToScrollPoint(new Point(squareAnnotation.Bounds.X + squareAnnotation.Bounds.Width, squareAnnotation.Bounds.Y + squareAnnotation.Bounds.Height), 2);

                    // Scroll to annotation and select it
                    pdfViewer.ScrollToOffset(scrollPoint.X, scrollPoint.Y);
                    pdfViewer.SelectAnnotation(squareAnnotation);
                }
            }
        }
    }

}
