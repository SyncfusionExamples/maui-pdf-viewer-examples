using Syncfusion.Maui.PdfViewer;
using System.Collections.ObjectModel;
using System.Reflection;

namespace PdfViewerAnnotations
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Stream? stream = typeof(MainPage).GetTypeInfo().Assembly.GetManifestResourceStream("PdfViewerAnnotations.Assets.Annotations.pdf");
            PDFViewer.LoadDocument(stream);
        }

        private void AddHighlightAnnotation(object sender, EventArgs e)
        {
            // Create a highlight annotation.
            HighlightAnnotation highlightAnnotation = CreateHighlightAnnotation();

            // Add the highlight annotation to the PDF document using the AddAnnotation method of the `SfPdfViewer` instance.
            PDFViewer.AddAnnotation(highlightAnnotation);
        }

        HighlightAnnotation CreateHighlightAnnotation()
        {
            int pageNumber = 1;

            // Create a list of text bounds that represents a multiple lines of text markups. Here a single line text markup are created.
            List<RectF> textBoundsCollection = new List<RectF> { new RectF(70, 70, 260, 40) };

            // Create an highlight annotation.
            HighlightAnnotation annotation = new HighlightAnnotation(pageNumber, textBoundsCollection);

            // Set the appearance for the highlight annotation
            annotation.Color = Colors.BlueViolet; // set the highlight color
            annotation.Opacity = 0.5f; // set the opacity to 50%

            return annotation;
        }

        private void RemoveAnnotation(object sender, EventArgs e)
        {
            //Obtain the annotation collection.
            ReadOnlyObservableCollection<Annotation> annotations = PDFViewer.Annotations;

            //Obtain the first annotation in the annotation collection.
            Annotation firstAnnotation = annotations[3];

            //Remove the annotation using RemoveAnnotation method of `SfPdfViewer`.
            PDFViewer.RemoveAnnotation(firstAnnotation);
        }

        void EditAnnotation(object sender, EventArgs e)
        {
            // Obtain the annotation collection using
            ReadOnlyObservableCollection<Annotation> annotations = PDFViewer.Annotations;

            // Obtain the first annotation in the annotation collection.
            Annotation annotation = annotations[4];

            // Edit the annotation properties.
            annotation.Color = Colors.Green; //Stroke color.
            annotation.Opacity = 0.75f; // 75% Opacity
        }

        void PerformUndo(object sender, EventArgs e)
        {
            // Undo the last operation using the UndoCommand of `SfPdfViewer` instance.
            PDFViewer.UndoCommand!.Execute(true);
        }

        void PerformRedo(object sender, EventArgs e)
        {
            // Redo the last operation using the RedoCommand of `SfPdfViewer` instance.
            PDFViewer.RedoCommand!.Execute(true);
        }
    }

}
