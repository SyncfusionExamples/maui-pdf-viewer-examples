using System.ComponentModel;
using System.Windows.Input;

namespace CustomToolbar
{
    public class PdfViewerViewModel : INotifyPropertyChanged
    {
        private int _pageNumber;
        private ICommand _openFileCommand;
        private ICommand _zoomInCommand;
        private ICommand _zoomOutCommand;
        private double _currentZoom = 1;
        private double _minZoom = 0.25;
        private double _maxZoom = 4;
        private bool _canZoomIn = true;
        private bool _canZoomOut = true;
        private bool _canGoToPreviousPage = true;
        private bool _canGoToNextPage = true;
        private bool _showPasswordDialog = false;
        private Stream _documentStream;
        private int _pageCount;

        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or set the value that determines whether zoom in is allowed.
        /// </summary>
        public bool CanZoomIn
        {
            get => _canZoomIn;
            set
            {
                _canZoomIn = value;
                OnPropertyChanged("CanZoomIn");
            }
        }

        /// <summary>
        /// Gets or sets the value to show or hide the password dialog.
        /// </summary>
        public bool ShowPasswordDialog
        {
            get => _showPasswordDialog;
            set
            {
                _showPasswordDialog = value;
                OnPropertyChanged("ShowPasswordDialog");
            }
        }

        /// <summary>
        /// Gets or sets the value that determines whether zoom out is allowed.
        /// </summary>
        public bool CanZoomOut
        {
            get => _canZoomOut;
            set
            {
                _canZoomOut = value;
                OnPropertyChanged("CanZoomOut");
            }
        }

        /// <summary>
        /// Gets or sets the value that determines whether go to previous page is allowed.
        /// </summary>
        public bool CanGoToPreviousPage
        {
            get => _canGoToPreviousPage;
            set
            {
                _canGoToPreviousPage = value;
                OnPropertyChanged("CanGoToPreviousPage");
            }
        }

        /// <summary>
        /// Gets or sets the value that determines whether go to next page is allowed.
        /// </summary>
        public bool CanGoToNextPage
        {
            get => _canGoToNextPage;
            set
            {
                _canGoToNextPage = value;
                OnPropertyChanged("CanGoToNextPage");
            }
        }

        /// <summary>
        /// Gets or sets the current zoom value.
        /// </summary>
        public double CurrentZoom
        {
            get => _currentZoom;
            set
            {
                _currentZoom = value;
                OnPropertyChanged("CurrentZoom");
                ValidateZoomChange();
            }
        }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int PageNumber
        {
            get => _pageNumber;
            set
            {
                _pageNumber = value;
                OnPropertyChanged("PageNumber");
                ValidatePageNumber();
            }
        }

        /// <summary>
        /// Gets or sets the minimum zoom allowed.
        /// </summary>
        public double MinZoom
        {
            get => _minZoom;
            set
            {
                _minZoom = value;
                ValidateZoomChange();
            }
        }

        /// <summary>
        /// Gets or sets the maximum zoom allowed.
        /// </summary>
        public double MaxZoom
        {
            get => _maxZoom;
            set
            {
                _maxZoom = value;
                ValidateZoomChange();
            }
        }

        /// <summary>
        /// Stores the PDF document stream.
        /// </summary>
        public Stream PdfDocumentStream
        {
            get => _documentStream;
            set
            {
                _documentStream = value;
                OnPropertyChanged("PdfDocumentStream");
            }
        }

        /// <summary>
        /// Stores the total page count information.
        /// </summary>
        public int PageCount
        {
            get => _pageCount;
            set
            {
                _pageCount = value;
                OnPropertyChanged("PageCount");
            }
        }

        /// <summary>
        /// Gets the command to browse file in the disk.
        /// </summary>
        public ICommand OpenFileCommand => _openFileCommand;

        /// <summary>
        /// Gets or sets the command to increase the zoom.
        /// </summary>
        public ICommand ZoomInCommand => _zoomInCommand;

        /// <summary>
        /// Gets or sets the command to decrease the zoom.
        /// </summary>
        public ICommand ZoomOutCommand => _zoomOutCommand;

        public PdfViewerViewModel()
        {
            PdfDocumentStream = this.GetType().Assembly.GetManifestResourceStream("CustomToolbar.Assets." + "PDF_Succinctly.pdf");
            _openFileCommand = new Command<object>(OpenFile);
            _zoomInCommand = new Command<object>(ZoomIn);
            _zoomOutCommand = new Command<object>(ZoomOut);
        }

        /// <summary>
        /// Validates the current zoom with minimum/maximum limit and determines whether further zoom is possible.
        /// </summary>
        public void ValidateZoomChange()
        {
            if (_currentZoom > _minZoom && _currentZoom < _maxZoom)
            {
                CanZoomIn = true;
                CanZoomOut = true;
            }
            else if (_currentZoom <= _minZoom && _currentZoom >= _maxZoom)
            {
                CanZoomIn = false;
                CanZoomOut = false;
            }
            else if (_currentZoom <= _minZoom)
                CanZoomOut = false;
            else if (_currentZoom >= _maxZoom)
                CanZoomIn = false;
        }

        /// <summary>
        /// Validates the current page number with the tolal page count and determines whether further page navigation is possible.
        /// </summary>
        public void ValidatePageNumber()
        {
            if (PageCount <= 1)
            {
                CanGoToPreviousPage = false;
                CanGoToNextPage = false;
            }
            if (_pageNumber <= 1)
                CanGoToPreviousPage = false;
            else
                CanGoToPreviousPage = true;
            if (_pageNumber >= PageCount)
                CanGoToNextPage = false;
            else
                CanGoToNextPage = true;
        }

        /// <summary>
        /// Trigges the property changed event.
        /// </summary>
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Increases the current zoom by 25%.
        /// </summary>
        void ZoomIn(object commandParameter)
        {
            CurrentZoom += 0.25;
        }

        /// <summary>
        /// Decreases the current zoom by 25%.
        /// </summary>
        void ZoomOut(object commandParameter)
        {
            CurrentZoom -= 0.25;
        }

        /// <summary>
        /// Opens the file from the disk and create document stream from it.
        /// </summary>
        async internal void OpenFile(object commandParameter)
        {
            ShowPasswordDialog = false;
            FilePickerFileType pdfFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>{
                        { DevicePlatform.iOS, new[] { "public.pdf" } },
                        { DevicePlatform.Android, new[] { "application/pdf" } },
                        { DevicePlatform.WinUI, new[] { "pdf" } },
                        { DevicePlatform.MacCatalyst, new[] { "pdf" } },
                    });
            PickOptions options = new()
            {
                PickerTitle = "Please select a PDF file",
                FileTypes = pdfFileType,
            };
            await PickAndShow(options);
        }

        /// <summary>
        /// Picks the file from the disc using the given option and creates the stream.
        /// </summary>
        public async Task<FileResult> PickAndShow(PickOptions options)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    PdfDocumentStream = await result.OpenReadAsync();
                }
                return result;
            }
            catch (Exception ex)
            {
                string message;
                if (ex != null && string.IsNullOrEmpty(ex.Message) == false)
                    message = ex.Message;
                else
                    message = "File open failed.";
                _ = Application.Current.MainPage.DisplayAlert("Error", message, "OK");
            }
            return null;
        }
    }
}