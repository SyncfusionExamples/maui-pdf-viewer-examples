using System.ComponentModel;
using System.Reflection;

namespace PdfViewerInStackLayout
{
    class PdfViewerViewModel : INotifyPropertyChanged
    {
        private Stream? m_pdfDocumentStream;

        /// <summary>
        /// An event to detect the change in the value of a property.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// The PDF document stream that is loaded into the instance of the PDF viewer. 
        /// </summary>
        public Stream PdfDocumentStream
        {
            get
            {
                return m_pdfDocumentStream;
            }
            set
            {
                m_pdfDocumentStream = value;
                OnPropertyChanged("PdfDocumentStream");
            }
        }

        /// <summary>
        /// Constructor of the view model class
        /// </summary>
        public PdfViewerViewModel()
        {
            //Accessing the PDF document that is added as embedded resource as stream.
            m_pdfDocumentStream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("PdfViewerInStackLayout.Assets.Barcode.pdf");
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
