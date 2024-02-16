using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Security;
using System.ComponentModel;
using System.Reflection;

namespace EncryptingPdf
{
    public class PdfViewerViewModel : INotifyPropertyChanged
    {
        //fields 
        private Stream? _inputPdfDocument;
        private Stream? _outputPdfDocument;
        private FileStream outputStream;

        //An event to detect the change in the value of a property.
        public event PropertyChangedEventHandler? PropertyChanged;

        //The PDF document stream that is loaded into the instance of the PDF viewer. 
        public Stream OutputPdfDocument
        {
            get { return _outputPdfDocument; }
            set
            {
                _outputPdfDocument = value;
                OnPropertyChanged("OutputDocument");
            }
        }

        //Constructor of the view model class
        public PdfViewerViewModel()
        {
            //Accessing the PDF document that is added as embedded resource as stream.
            _inputPdfDocument = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("EncryptingPdf.Assets.PDF_Succinctly.pdf");

            //Load existing PDF document.
            PdfLoadedDocument document = new PdfLoadedDocument(_inputPdfDocument);

            //Create a document security.
            PdfSecurity security = document.Security;

            //Set encryption algorithm.
            security.Algorithm = PdfEncryptionAlgorithm.AES;

            //Set key size.
            security.KeySize = PdfEncryptionKeySize.Key256Bit;

            //Set user password for the document.
            security.UserPassword = "sample123";

            // Create a file stream to save the PDF document. Here a file named "EncryptedPdf.pdf" is created in the application's data directory.
            string filepath = Path.Combine(FileSystem.Current.AppDataDirectory, "EncryptedPdf.pdf");
            outputStream = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite);
            // Save the PDF document.
            document.Save(outputStream);

            //Assign the output file to property to use in the Document source for loading the PDF 
            OutputPdfDocument = outputStream;

        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
