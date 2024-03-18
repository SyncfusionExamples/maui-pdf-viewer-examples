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
            stream = typeof(MainPage).GetTypeInfo().Assembly.GetManifestResourceStream("ConversionOfCoordinates.Annotations.pdf");
            pdfViewer.LoadDocument(stream);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            int count = 0;
            foreach (var annotation in pdfViewer.Annotations)
            {
                if (annotation is SquareAnnotation squareAnnotation && count == 0)
                {
                    count++;
                    Point clientPoint = pdfViewer.ConvertPagePointToScrollPoint(new Point(squareAnnotation.Bounds.X, squareAnnotation.Bounds.Y), 2);
                    Point endPoint = pdfViewer.ConvertPagePointToScrollPoint(new Point(squareAnnotation.Bounds.X + squareAnnotation.Bounds.Width, squareAnnotation.Bounds.Y + squareAnnotation.Bounds.Height), 2);
                    pdfViewer.ScrollToOffset(clientPoint.X, clientPoint.Y);
                    pdfViewer.SelectAnnotation(squareAnnotation);
                }
            }
        }
    }

}
