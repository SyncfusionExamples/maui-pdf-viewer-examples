using System.ComponentModel;
using System.Reflection;

namespace OpenPDFFromBase64String.ViewModel
{
    public class PdfViewerViewModel : INotifyPropertyChanged
    {
        private byte[]? m_pdfDocumentByteArray;

        /// <summary>
        /// An event to detect the change in the value of a property.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// The PDF document byte array that is loaded into the instance of the PDF viewer. 
        /// </summary>
        public byte[]? PdfDocumentByteArray
        {
            get
            {
                return m_pdfDocumentByteArray;
            }
            set
            {
                m_pdfDocumentByteArray = value;
                OnPropertyChanged("PdfDocumentByteArray");
            }
        }

        /// <summary>
        /// Constructor of the view model class
        /// </summary>
        public PdfViewerViewModel()
        {
            SetPdfDocumentByteArray();
        }

        /// <summary>
        /// Gets and sets the document byte array from the given base64 string converted PDF file. 
        /// </summary>
        private void SetPdfDocumentByteArray()
        {
            // Assign the returned byte array to the PdfDocumentByteArray variable
            PdfDocumentByteArray = GetByteArrayFromBase64String();
        }

        // Method to return a MemoryStream containing a base64 string converted PDF file
        public byte[]? GetByteArrayFromBase64String()
        {
            // Get the current executing assembly
            var assembly = Assembly.GetExecutingAssembly();

            // Open a StreamReader to read the embedded resource named "OpenPDFFromBase64String.PDFEncodedBase64String.txt" which has base64 string content.
            using var reader = new StreamReader(
                assembly.GetManifestResourceStream("OpenPDFFromBase64String.Assets.PDFEncodedBase64String.txt")!);

            // Read the entire Base64 string content from the embedded text file.
            string base64Text = reader.ReadToEnd();

            // Decode the Base64 string into a byte array.
            byte[] decodedBytes = System.Convert.FromBase64String(base64Text);

            // return the byte array.
            return decodedBytes;
        }
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}