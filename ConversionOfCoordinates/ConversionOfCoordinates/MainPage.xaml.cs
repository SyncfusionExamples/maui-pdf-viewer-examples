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
            pdfViewer.LoadDocument(stream, flattenOptions: FlattenOptions.Unsupported);
            pdfViewer.PropertyChanged += PdfViewer_PropertyChanged;
            semitransparentView = new SemitransparentView();
        }

        private void PdfViewer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "VerticalOffset" || e.PropertyName == "HorizontalOffset" || e.PropertyName == "ZoomFactor") && isAnnotationAddedInScrollview)
            {
                if(semitransparentView != null)
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

        private void Button_Clicked(object sender, EventArgs e)
        {
            isAnnotationAddedInScrollview = true;
            semitransparentView = new SemitransparentView();
            AbsoluteLayout.SetLayoutBounds(semitransparentView, new Rect(0, 0, (double)pdfViewer.ClientRectangle.Width - 16, (double)pdfViewer.ExtentHeight));
            pdfViewer.Children.Add(semitransparentView);
            PdfLoadedDocument pdfLoadedDocument = new PdfLoadedDocument(stream);
            for (int i = 0; i < pdfLoadedDocument.PageCount; i++)
            {
                PdfPageBase page = pdfLoadedDocument.Pages[i] as PdfPageBase;
                foreach (var annotation in page.Annotations)
                {
                    if (annotation is PdfLoadedRectangleAnnotation squareAnnotation)
                    {
                        Point clientPoint = pdfViewer.ConvertPagePointToScrollPoint(new Point(squareAnnotation.Bounds.X, squareAnnotation.Bounds.Y), i + 1);
                        endPoint = pdfViewer.ConvertPagePointToScrollPoint(new Point(endPoint.X, endPoint.Y), i + 1);
                        clientPoint.Y = (double)(clientPoint.Y - pdfViewer.VerticalOffset);
                        endPoint.Y = (double)(endPoint.Y - pdfViewer.VerticalOffset);
                        clientPoint.X = (double)(clientPoint.X - pdfViewer.HorizontalOffset);
                        endPoint.X = (double)(endPoint.X - pdfViewer.HorizontalOffset);
                        semitransparentView.Children.Remove(myGrid);
                        myGrid = new Grid
                        {
                            Background = Colors.Blue,
                            HeightRequest = 100,
                            WidthRequest = 200,
                        };
                        AbsoluteLayout.SetLayoutBounds(myGrid, new Rect(clientPoint.X, clientPoint.Y, 200, 100));
                        semitransparentView.Children.Add(myGrid);
                    }
                }
            }
        }
    }

}
