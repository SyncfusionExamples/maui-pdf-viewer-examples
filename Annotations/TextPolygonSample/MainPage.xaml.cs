using Syncfusion.Maui.PdfViewer;
using System.Globalization;

namespace TextPolygonSample
{
    public partial class MainPage : ContentPage
    {
        // Define a variable to hold the polygon bounds and thickness for positioning other annotations
        RectF polygonBounds;
        float polygonThickness;
        PolygonAnnotation? polygonAnnotation;
        FreeTextAnnotation? textAnnotation;

        public MainPage()
        {
            InitializeComponent();
            
            // Load a PDF document embedded in the project resources
            Stream? documentStream = this.GetType().Assembly.GetManifestResourceStream("TextPolygonSample.Assets.Annotations.pdf");

            // Load the PDF document into the SfPdfViewer
            PdfViewer.LoadDocument(documentStream);

            // Call method to convert coordinates and add polygon annotations to the PDF
            ConvertAndAddPolygonAnnotation();

            // Create and add free text annotation within the polygon
            CreateAndAddFreeTextAnnotation();
        }

        // Converts coordinate to polygon points and adds polygon and text annotations
        void ConvertAndAddPolygonAnnotation()
        {
            // Example coordinates for a polygon
            var coordinates = "M50 27L111 27L111 63L50 63";

            // Create and add polygon annotation based on coordinate
            CreateAndAddPolygonAnnotation(coordinates);
        }

        // Creates and adds a free text annotation inside the polygon annotation
        private void CreateAndAddFreeTextAnnotation()
        {
            // Text to be displayed within the free text annotation
            string text = "TOMTEST";

            // Calculate position and size of the free text annotation
            float FreeTextX = polygonBounds.X + polygonThickness;
            float FreeTextY = polygonBounds.Y + polygonThickness;
            float FreeTextWidth = polygonBounds.Width - polygonThickness;
            float FreeTextHeight = polygonBounds.Height - polygonThickness;

            // Define bounds for the free text annotation
            RectF freeTextBounds = new RectF(FreeTextX, FreeTextY, FreeTextWidth, FreeTextHeight);

            // Create a free text annotation and add it to the PDF viewer
            textAnnotation = new FreeTextAnnotation(text, 1, freeTextBounds);
            PdfViewer.AddAnnotation(textAnnotation);
        }

        // Converts coordinates to a list of points for the polygon annotation
        public List<PointF> ConvertCoordinatesToPolygonPoints(string coordinatesData)
        {
            // List to hold the parsed points
            var points = new List<PointF>();

            // Split the coordinates data into commands based on the 'M' (move to) and 'L' (line to) commands
            string[] commands = coordinatesData.Split(new char[] { 'M', 'L' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var command in commands)
            {
                // Separate each command into X and Y coordinates
                string[] coords = command.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (coords.Length >= 2)
                {
                    // Parse the coordinates and add to the list as a PointF object
                    float x = float.Parse(coords[0], CultureInfo.InvariantCulture);
                    float y = float.Parse(coords[1], CultureInfo.InvariantCulture);
                    points.Add(new PointF(x, y));
                }
            }
            return points;
        }

        // Creates a polygon annotation from a list of points
        private PolygonAnnotation CreatePolygonAnnotation(List<PointF> polygonPoints)
        {
            // Create the polygon annotation and set its properties
            PolygonAnnotation polygonAnnotation = new PolygonAnnotation(polygonPoints, 1);
            polygonAnnotation.Color = Colors.Black; // Set annotation color
            polygonAnnotation.Opacity = 25; // Set annotation opacity

            // Set the bounds and thickness for use in positioning other annotations
            polygonBounds = polygonAnnotation.Bounds;
            polygonThickness = polygonAnnotation.BorderWidth;

            // Set a solid border style for the polygon
            polygonAnnotation.BorderStyle = BorderStyle.Solid;

            return polygonAnnotation;
        }

        // Parses coordinate, creates a polygon annotation, and adds it to the PDF viewer
        private void CreateAndAddPolygonAnnotation(string coordinate)
        {
            // Convert coordinate to a list of polygon points
            List<PointF> polygonPoints = ConvertCoordinatesToPolygonPoints(coordinate);

            // Create the polygon annotation from the points
            polygonAnnotation = CreatePolygonAnnotation(polygonPoints);

            // Add the polygon annotation to the PDF document
            PdfViewer.AddAnnotation(polygonAnnotation);
        }

        //private void PdfViewer_AnnotationEdited(object sender, AnnotationEventArgs e)
        //{
        //    if (e.Annotation is FreeTextAnnotation annotation && annotation == textAnnotation && polygonAnnotation != null)
        //    {
        //        polygonBounds = annotation.Bounds;
        //    }
        //    if (polygonAnnotation != null)
        //    {
        //        polygonAnnotation.Bounds = polygonBounds;
        //        polygonAnnotation.Points = new List<PointF>() { new PointF(polygonBounds.X, polygonBounds.Y), new PointF(polygonBounds.X + polygonBounds.Width, polygonBounds.Y), new PointF(polygonBounds.X, polygonBounds.Y + polygonBounds.Height), new PointF(polygonBounds.X + polygonBounds.Width, polygonBounds.Y + polygonBounds.Height) };
        //    }
        //}
    }
}
