using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf;
using Microsoft.Maui.Graphics;
namespace CreatePDF
{
    public partial class MainPage : ContentPage
    {
        Stream documentStream;
        public MainPage()
        {
            InitializeComponent();
            //Create a new PDF document.
            PdfDocument document = new PdfDocument();
            //Add a page to the document.
            PdfPage page = document.Pages.Add();
            //Create PDF graphics for the page.
            PdfGraphics graphics = page.Graphics;

            //Set the standard font.
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
            //Draw the text.
            graphics.DrawString("Hello World!!!", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));

            //Creating the stream object.
            MemoryStream stream = new MemoryStream();
            //Save the document into memory stream.
            document.Save(stream);
            //Close the document.
            document.Close(true);
            // Store the stream for later use.
            documentStream = stream;

            // Load the document into the PdfViewer.
            PdfViewer.LoadDocument(documentStream);
        }
    }

}
