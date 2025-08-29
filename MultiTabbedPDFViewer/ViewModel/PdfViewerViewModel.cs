using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;

namespace MultiTabbedPDFViewer
{
    public class PdfViewerViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Stream>? _pdfDocuments;

        /// <summary>
        /// Constructor of the view model class
        /// </summary>
        public PdfViewerViewModel()
        {
            // Initialize the collection
            PDFDocuments = new ObservableCollection<Stream>();

            //Accessing the PDF document that is added as embedded resource as stream.
            Stream? documentSource = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("MultiTabbedPDFViewer.Assets.PDF_Succinctly.pdf");
            Stream? documentSource1 = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("MultiTabbedPDFViewer.Assets.rotated_document.pdf");
            Stream? documentSource2 = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("MultiTabbedPDFViewer.Assets.Annotations.pdf");
            // Add the PDF streams to the collection if they are successfully retrieved
            if (documentSource != null)
                PDFDocuments.Add(documentSource);
            if (documentSource1 != null)
                PDFDocuments.Add(documentSource1);
            if (documentSource2 != null)
                PDFDocuments.Add(documentSource2);
        }

        /// <summary>
        /// Collection of PDF document streams.
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

        /// <summary>
        /// An event to detect the change in the value of a property.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
