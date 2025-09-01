using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Maui.PdfViewer;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Presentation;
using Syncfusion.PresentationRenderer;
using Syncfusion.XlsIO;
using Syncfusion.XlsIORenderer;
using Syncfusion.XPS;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Reflection.Metadata;

namespace DocumentViewerDemo
{
    internal class PdfViewerViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Stream>? _pdfDocuments;

        /// <summary>
        /// An event to detect the change in the value of a property.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Constructor of the view model class
        /// </summary>
        public PdfViewerViewModel()
        {
            // Initialize the collection
            PDFDocuments = new ObservableCollection<Stream>();

            //Accessing the PDF document that is added as embedded resource as stream.
            Stream? documentSource = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("DocumentViewerDemo.Assets.PDF_Succinctly.pdf");
            Stream? documentSource1 = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("DocumentViewerDemo.Assets.Input.docx");
            Stream? documentSource2 = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("DocumentViewerDemo.Assets.Autumn Leaves.jpg");
            Stream? documentSource3 = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("DocumentViewerDemo.Assets.Template.pptx");
            Stream? documentSource4 = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("DocumentViewerDemo.Assets.InputTemplate.xlsx");
            Stream? documentSource5 = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("DocumentViewerDemo.Assets.Input.xps");
            if (documentSource != null)
                PDFDocuments.Add(documentSource);
            if (documentSource1 != null)
                PDFDocuments.Add(documentSource1);
            if (documentSource2 != null)
                PDFDocuments.Add(documentSource2);
            if (documentSource3 != null)
                PDFDocuments.Add(documentSource3);
            if (documentSource4 != null)
                PDFDocuments.Add(documentSource4);
            if (documentSource5 != null)
                PDFDocuments.Add(documentSource5);
        }

        /// <summary>
        /// Collection of PDF document streams for the ListView
        /// </summary>
        public ObservableCollection<Stream>? PDFDocuments
        {
            get => _pdfDocuments;
            set
            {
                _pdfDocuments = value;
                OnPropertyChanged(nameof(PDFDocuments));
            }
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        internal Stream? ConvertDocxToPdf(Stream? docxStream)
        {
            // Load the Word document from the input stream in DOCX format
            using WordDocument wordDocument = new WordDocument(docxStream, Syncfusion.DocIO.FormatType.Docx);

            // Initialize the DocIO renderer to convert Word documents to PDF
            DocIORenderer render = new DocIORenderer();

            //Converts Word document into PDF document
            PdfDocument pdfDocument = render.ConvertToPDF(wordDocument);

            //Saves the PDF document to MemoryStream.
            MemoryStream stream = new MemoryStream();
            pdfDocument.Save(stream);
            stream.Position = 0;
            return stream;
        }

        internal Stream? ConvertPptToPdf(Stream? pptStream)
        {
            // Create new memory stream to save the converted PDF
            MemoryStream stream = new MemoryStream();

            //Open the PowerPoint presentation with loaded stream.
            using (IPresentation pptxDoc = Presentation.Open(pptStream))
            {
                //Convert PowerPoint into PDF document. 
                using (PdfDocument pdfDocument = PresentationToPdfConverter.Convert(pptxDoc))
                {
                    // save the PDF document as memory stream.
                    pdfDocument.Save(stream);
                }
            }
            return stream;
        }
        internal Stream? ConvertXlsxToPdf(Stream? xlsxStream)
        {
            using ExcelEngine excelEngine = new ExcelEngine();
            Syncfusion.XlsIO.IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Xlsx;

            // Open the workbook.
            IWorkbook workbook = application.Workbooks.Open(xlsxStream);

            // Instantiate the Excel to PDF renderer.
            XlsIORenderer renderer = new XlsIORenderer();

            // create XlsIORendererSettings and set the LayoutOptions to cover all the columns in one page.
            XlsIORendererSettings settings = new XlsIORendererSettings
            {
                LayoutOptions = Syncfusion.XlsIORenderer.LayoutOptions.FitAllColumnsOnOnePage
            };


            //Convert Excel document into PDF document
            PdfDocument pdfDocument = renderer.ConvertToPDF(workbook, settings);

            //Create the MemoryStream to save the converted PDF.
            MemoryStream stream = new MemoryStream();

            //Save the converted PDF document to MemoryStream.
            pdfDocument.Save(stream);
            stream.Position = 0;
            return stream;
        }

        internal Stream? ConvertXpsToPdf(Stream? xpsStream)
        {
            //Initialize XPS to PDF converter.
            XPSToPdfConverter converter = new XPSToPdfConverter();

            //Convert the XPS to PDF.
            PdfDocument pdfDocument = converter.Convert(xpsStream);

            //Creating the stream object.
            MemoryStream stream = new MemoryStream();

            //Save the document into stream.
            pdfDocument.Save(stream);
            stream.Position = 0;
            return stream;
        }

        internal Stream? ConvertImageToPdf(Stream? imageStream)
        {
            // Load the image from stream
            PdfBitmap image = new PdfBitmap(imageStream);

            //Create a new PDF document
            PdfDocument pdfDocument = new PdfDocument();

            // Set page size to match image size
            PdfPageSettings pageSettings = new PdfPageSettings();
            pageSettings.Size = new Syncfusion.Drawing.SizeF(image.Width, image.Height);

            pdfDocument.PageSettings = pageSettings;

            //Add a page to the document
            PdfPage page = pdfDocument.Pages.Add();

            //Create PDF graphics for the page
            PdfGraphics graphics = page.Graphics;

            //Draw the image
            graphics.DrawImage(image, 0, 0, image.Width, image.Height);

            //Creating the stream object
            MemoryStream stream = new MemoryStream();

            //Save the document as stream
            pdfDocument.Save(stream);

            //If the position is not set to '0' then the PDF will be empty
            stream.Position = 0;
            return stream;
        }
    }
}