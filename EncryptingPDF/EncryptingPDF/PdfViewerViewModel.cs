using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Security;
using System.ComponentModel;
using System.Reflection;

namespace EncryptingPDF
{
    class PdfViewerViewModel
    {
        private Stream? _inputPdfDocument;
        private Stream? pdfDocumentStream;
        private FileStream outputStream;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the stream of the currently loaded PDF document.
        /// </summary>
        public Stream? PdfDocumentStream
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
            //Accessing the PDF document that is added as embedded resource as stream.
            _inputPdfDocument = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("EncryptingPDF.Assets.PDF_Succinctly.pdf");

            //Load existing PDF document.
            PdfLoadedDocument document = new PdfLoadedDocument(_inputPdfDocument);

            //Create a document security.
            PdfSecurity security = document.Security;

            //Set encryption algorithm.
            security.Algorithm = PdfEncryptionAlgorithm.AES;

            //Set key size.
            security.KeySize = PdfEncryptionKeySize.Key256Bit;

            //Set user password for the document.
            security.UserPassword = "Sample@123";

            // Create a file stream to save the PDF document. Here a file named "EncryptedPdf.pdf" is created in the application's data directory.
            string filepath = Path.Combine(FileSystem.Current.AppDataDirectory, "EncryptedPdf.pdf");
            outputStream = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite);

            // Save the PDF document.
            document.Save(outputStream);

            //Assign the output file to property to use in the Document source for loading the PDF 
            PdfDocumentStream = outputStream;
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

