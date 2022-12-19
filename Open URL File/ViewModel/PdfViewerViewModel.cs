using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;
using Syncfusion.Maui.PdfViewer;

namespace OpenURLFile.ViewModel
{
    public class PdfViewerViewModel : INotifyPropertyChanged
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
             SetPdfDocumentStream("https://www.syncfusion.com/downloads/support/directtrac/general/pd/PDF_Succinctly1928776572");
        }

        /// <summary>
        /// Gets and sets the document stream from the given URL. 
        /// </summary>
        /// <param name="URL">Document URL</param>
        private async void SetPdfDocumentStream(string URL)
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(URL);

            PdfDocumentStream = await response.Content.ReadAsStreamAsync();
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}