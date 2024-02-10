using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf;
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
            documentStream = stream;
        }

        private void Preview_Clicked(object sender, EventArgs e)
        {
            PdfViewer.LoadDocument(documentStream);
        }
    }

}
