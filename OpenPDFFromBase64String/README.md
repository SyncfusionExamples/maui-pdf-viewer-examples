# How to open a PDF document from a base64 string?
This project demonstrates how to open a PDF document from a given base64 string text file by converting it to a byte[] and assigning the obtained byte[] to the DocumentSource property.

## Prerequisites
1. A .NET MAUI project set up.
2. The [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer) package is installed.

Just by making a few changes to the *PdfViewerViewModel.cs* shared in the getting started example, you can easily open a document from a given base64 string text file. Refer to the following code snippet:

```csharp
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
             SetPdfDocumentStream();
        }

        /// <summary>
        /// Gets and sets the document byte array from the given base64 string converted PDF file. 
        /// </summary>
        private void SetPdfDocumentStream()
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
            byte[] decodedBytes = Convert.FromBase64String(base64Text);

            // return the byte array.
            return decodedBytes;
        }
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
```

## Run the App
Build and run the application on all platforms.



