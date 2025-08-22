using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MultiTabbedPDFViewer
{
    public class PdfViewerViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Constructor of the view model class
        /// </summary>
        public PdfViewerViewModel()
        {
            //Accessing the PDF document that is added as embedded resource as stream.
            _documentSource = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("MultiTabbedPDFViewer.Assets.PDF_Succinctly.pdf");
            _documentSource2 = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("MultiTabbedPDFViewer.Assets.form_document.pdf");
        }

        private Stream? _documentSource;

        /// <summary>
        /// The PDF document stream that is loaded into the instance of the PDF viewer. 
        /// </summary>
        public Stream? PDFDocumentSource
        {
            get => _documentSource;
            set
            {
                _documentSource = value;
                OnPropertyChanged(nameof(PDFDocumentSource));
            }
        }


        private Stream? _documentSource2;

        /// <summary>
        /// The PDF document stream that is loaded into the instance of the PDF viewer. 
        /// </summary>
        public Stream? PDFDocumentSource2
        {
            get => _documentSource2;
            set
            {
                _documentSource2 = value;
                OnPropertyChanged(nameof(PDFDocumentSource2));
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
