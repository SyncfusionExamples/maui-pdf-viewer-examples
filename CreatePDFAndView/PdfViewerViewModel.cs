// PdfViewerViewModel.cs
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.ComponentModel;
using System.IO;

namespace CreatePDFAndView
{
    class PdfViewerViewModel : INotifyPropertyChanged
    {
        private Stream pdfDocumentStream;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the stream of the currently loaded PDF document.
        /// </summary>
        public Stream PdfDocumentStream
        {
            get
            {
                return pdfDocumentStream;
            }
            set
            {
                pdfDocumentStream = value;
                OnPropertyChanged(nameof(PdfDocumentStream));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfViewerViewModel"/> class.
        /// </summary>
        public PdfViewerViewModel()
        {
            //Set the created PDF stream to pdfDocumentStream for loading it in PDF Viewer
            pdfDocumentStream = CreatePDF();
        }

        private Stream CreatePDF()
        {
            // Create a new PDF document.
            PdfDocument document = new PdfDocument();

            // Add a page to the document.
            PdfPage page = document.Pages.Add();

            // Create PDF graphics for the page.
            PdfGraphics graphics = page.Graphics;

            // Set the standard font.
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);

            // Define the text to be drawn.
            string text = "This PDF was created using the Syncfusion PDF Library.";

            // Measure the text size.
            Syncfusion.Drawing.SizeF textSize = font.MeasureString(text);

            // Calculate the center position for the text.
            float pageWidth = page.GetClientSize().Width;
            float pageHeight = page.GetClientSize().Height;
            float xPosition = (pageWidth - textSize.Width) / 2;
            float yPosition = (pageHeight - textSize.Height) / 2;

            // Draw the centered text.
            graphics.DrawString(text, font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(xPosition, yPosition));

            // Creating the stream object.
            MemoryStream stream = new MemoryStream();

            // Save the document into memory stream.
            document.Save(stream);

            // Close the document.
            document.Close(true);

            if (stream.CanSeek)
                stream.Position = 0;

            return stream;
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the specified property name.
        /// </summary>
        /// <param name="name">The name of the property that changed.</param>
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}