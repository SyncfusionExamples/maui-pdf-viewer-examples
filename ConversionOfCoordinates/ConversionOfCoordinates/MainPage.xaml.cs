using Syncfusion.Maui.PdfViewer.Annotations;
using Syncfusion.Maui.PdfViewer;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
using System.Globalization;
using System.Reflection;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Controls;
using Syncfusion.Pdf;
using Syncfusion.Maui.Core.Internals;
using System.Diagnostics;

namespace ConversionOfCoordinates
{
    public partial class MainPage : ContentPage
    {
        Point endPoint = new Point(0, 0);
        Stream stream;
        Grid myGrid;
        SemitransparentView semitransparentView;
        bool isAnnotationAddedInScrollview = false;

        public MainPage()
        {
            InitializeComponent();
            stream = typeof(MainPage).GetTypeInfo().Assembly.GetManifestResourceStream("ConversionOfCoordinates.Annotations.pdf");
            pdfViewer.LoadDocument(stream);
            semitransparentView = new SemitransparentView();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            isAnnotationAddedInScrollview = true;
            semitransparentView = new SemitransparentView();
            AbsoluteLayout.SetLayoutBounds(semitransparentView, new Rect( 0,0, pdfViewer.ClientRectangle.Width - 15, pdfViewer.ExtentHeight));
            pdfViewer.Children.Add (semitransparentView);
            if (semitransparentView != null)
            {
                semitransparentView.TranslationY = -pdfViewer.VerticalOffset.Value;
                semitransparentView.TranslationX = -pdfViewer.HorizontalOffset.Value;
                semitransparentView.Children.Remove(myGrid);

                PdfLoadedDocument pdfLoadedDocument = new PdfLoadedDocument(stream);
                for (int i = 0; i < pdfLoadedDocument.PageCount; i++)
                {
                    PdfPageBase page = pdfLoadedDocument.Pages[i] as PdfPageBase;
                    if (i == pdfViewer.PageNumber - 1)
                    {
                        foreach (var annotation in page.Annotations)
                        {
                            if (annotation is PdfLoadedRectangleAnnotation squareAnnotation)
                            {
                                Point clientPoint = pdfViewer.ConvertPagePointToScrollPoint(new Point(squareAnnotation.Bounds.X, squareAnnotation.Bounds.Y), i + 1);
                                endPoint = pdfViewer.ConvertPagePointToScrollPoint(new Point(squareAnnotation.Bounds.X + squareAnnotation.Bounds.Width, squareAnnotation.Bounds.Y + squareAnnotation.Bounds.Height), i + 1);

                                clientPoint.Y = (double)(clientPoint.Y - pdfViewer.VerticalOffset);
                                endPoint.Y = (double)(endPoint.Y - pdfViewer.VerticalOffset);
                                clientPoint.X = (double)(clientPoint.X - pdfViewer.HorizontalOffset);
                                endPoint.X = (double)(endPoint.X - pdfViewer.HorizontalOffset);

                                myGrid = new Grid
                                {
                                    Background = Colors.Blue,
                                    WidthRequest = endPoint.X - clientPoint.X,
                                    HeightRequest = endPoint.Y - clientPoint.Y
                                };
                                AbsoluteLayout.SetLayoutBounds(myGrid, new Rect(clientPoint.X + pdfViewer.HorizontalOffset.Value, clientPoint.Y + pdfViewer.VerticalOffset.Value, endPoint.X - clientPoint.X, endPoint.Y - clientPoint.Y));
                                if (!semitransparentView.Children.Contains(myGrid) && !pdfViewer.Children.Contains(myGrid))
                                    semitransparentView.Children.Add(myGrid);
                            }
                        }

                    }
                }
            }
        }
    }

}
