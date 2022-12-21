using System.ComponentModel;
using System.Reflection;

namespace PageNavigation
{
    internal class PdfViewerViewModel:INotifyPropertyChanged
    {
        private Stream? pdfDocumentStream;
        private int pageNumber;
        private int? pageCount;
        private bool canGoToPreviousPage = true;
        private bool canGoToNextPage = true;
        private bool canGoToFirstPage = true;
        private bool canGoToLastPage = true;

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
                return pdfDocumentStream;
            }
            set
            {
                pdfDocumentStream = value;
                OnPropertyChanged("PdfDocumentStream");
            }
        }

        /// <summary>
        /// Returns the total number of pages in a document.
        /// </summary>
        public int? TotalPageCount
        {
            get => pageCount;
            set
            {
                pageCount = value;
                OnPropertyChanged("TotalPageCount");
            }
        }

        /// <summary>
        /// Returns the current page number.
        /// </summary>
        public int CurrentPageNumber
        {
            get
            {
                return pageNumber;
            }
            set
            {
                pageNumber = value;
                OnPropertyChanged("CurrentPageNumber");
                ValidatePageNumber();
            }
        }

        /// <summary>
        /// Constructor of the view model class
        /// </summary>
        public PdfViewerViewModel()
        {
            // Accessing the PDF document that is added as embedded resource as stream.
            pdfDocumentStream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("PageNavigation.Assets.PDF_Succinctly.pdf");
        }

        /// <summary>
        ///  Determines whether the go to previous page can execute in its current state.
        /// </summary>
        public bool CanGoToPreviousPage
        {
            get
            {
                return canGoToPreviousPage;
            }
            set
            {
                canGoToPreviousPage = value;
                OnPropertyChanged("CanGoToPreviousPage");
            }
        }

        /// <summary>
        ///  Determines whether the go to next page can execute in its current state.
        /// </summary>
        public bool CanGoToNextPage
        {
            get
            {
                return canGoToNextPage;
            }
            set
            {
                canGoToNextPage = value;
                OnPropertyChanged("CanGoToNextPage");
            }
        }

        /// <summary>
        ///  Determines whether the go to first page can execute in its current state.
        /// </summary>
        public bool CanGoToFirstPage
        {
            get
            {
                return canGoToFirstPage;
            }
            set
            {
                canGoToFirstPage = value;
                OnPropertyChanged("CanGoToFirstPage");
            }
        }

        /// <summary>
        ///  Determines whether the go to last page can execute in its current state.
        /// </summary>
        public bool CanGoToLastPage
        {
            get
            {
                return canGoToLastPage;
            }
            set
            {
                canGoToLastPage = value;
                OnPropertyChanged("CanGoToLastPage");
            }
        }
        
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Validates the current page number and determines whether the go to page state can execute.
        /// </summary>
        public void ValidatePageNumber()
        {            
            if (pageNumber <= 1 || pageNumber > TotalPageCount)
            {
                CanGoToPreviousPage = false;
                CanGoToFirstPage = false;
            }
            else
            {
                CanGoToPreviousPage = true;
                CanGoToFirstPage = true;
            }
            if (pageNumber >= TotalPageCount || pageNumber < 1)
            {
                CanGoToNextPage = false;
                CanGoToLastPage = false;
            }
            else
            {
                CanGoToNextPage = true;
                CanGoToLastPage = true;
            }
        }
    }
}
