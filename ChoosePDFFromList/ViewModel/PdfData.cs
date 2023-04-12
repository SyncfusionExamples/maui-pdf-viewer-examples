using System.ComponentModel;
using System.Windows.Input;

namespace ChoosePDFFromList
{
    public class PdfData : INotifyPropertyChanged
    {
        private Stream _documentStream;

        /// <summary>
        /// An event to detect the change in the value of a property.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public IList<string> Files
        {
            get
            {
                return new List<string>
                {
                    "C#_Succinctly",
                    "HTTP Succinctly",
                    "JavaScript Succinctly",
                    "PdfSuccinctly",
                    "SinglePage"
                };
            }
        }

        /// <summary>
        /// The PDF document stream that is loaded into the instance of the PDF viewer. 
        /// </summary>
        public Stream DocumentStream
        {
            get => _documentStream;
            set
            {
                _documentStream = value;
                OnPropertyChanged("DocumentStream");
            }
        }

        /// <summary>
        /// Constructor of the pdf data class
        /// </summary>
        public PdfData()
        {
            UpdateDocumentStream("PdfSuccinctly");
        }

        internal void UpdateDocumentStream(string fileName)
        {
            string basePath = "ChoosePDFFromList.Assets.";
            //Accessing the PDF document that is added as embedded resource as stream.
            DocumentStream = this.GetType().Assembly.GetManifestResourceStream(basePath + fileName + ".pdf");
        }

        /// <summary>
        /// The method that is invoked when the value of a property is changed.
        /// </summary>
        /// <param name="name"></param>
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}