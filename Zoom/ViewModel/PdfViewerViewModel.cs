using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Magnification
{
    /// <summary>
    /// View model class for the PDF Viewer page.
    /// </summary>
    public class PdfViewerViewModel : INotifyPropertyChanged
    {
        private double _currentZoom = 1;
        private double? _minZoom = 0.5;
        private double? _maxZoom = 8;
        private bool _canZoomIn = true;
        private bool _canZoomOut = true;
        private ICommand _zoomInCommand;
        private ICommand _zoomOutCommand;
        private Stream _pdfDocumentStream;
        private int _currentZoomIndex = 1;

        /// <summary>
        /// Gets the command to increase the current zoom.
        /// </summary>
        public ICommand ZoomInCommand
        {
            get
            {
                if (_zoomInCommand == null)
                    _zoomInCommand = new Command<object>(ZoomIn);
                return _zoomInCommand;
            }
        }

        /// <summary>
        /// Provides the available custom zoom percentages.
        /// </summary>
        public ObservableCollection<double> ZoomPercentages
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the command to decrease the current zoom.
        /// </summary>
        public ICommand ZoomOutCommand
        {
            get
            {
                if (_zoomOutCommand == null)
                    _zoomOutCommand = new Command<object>(ZoomOut);
                return _zoomOutCommand;
            }
        }

        /// <summary>
        /// Gets or sets the index of currently used zoom.
        /// </summary>
        public int CurrentZoomIndex
        {
            get
            {
                return _currentZoomIndex;
            }
            set
            {
                _currentZoomIndex = value;
                CurrentZoom = ZoomPercentages[value] / 100;
                OnPropertyChanged("CurrentZoomIndex");
            }
        }

        /// <summary>
        /// Executes the zoom in command.
        /// </summary>
        void ZoomIn(object commandParameter)
        {
            if (CurrentZoomIndex < ZoomPercentages.Count - 1)
                CurrentZoomIndex += 1;
        }

        /// <summary>
        /// Executes the zoom out command.
        /// </summary>
        void ZoomOut(object commandParameter)
        {
            if (CurrentZoomIndex > 0)
                CurrentZoomIndex -= 1;
        }

        /// <summary>
        /// Gets or sets the current zoom.
        /// </summary>
        public double CurrentZoom
        {
            get
            {
                return _currentZoom;
            }
            set
            {
                _currentZoom = value;
                OnPropertyChanged("CurrentZoom");
                ValidateZoomChange();
            }
        }

        /// <summary>
        /// Gets or sets the value that determines whether zoom in can be performed. 
        /// </summary>
        public bool CanZoomIn
        {
            get
            {
                return _canZoomIn;
            }
            set
            {
                _canZoomIn = value;
                OnPropertyChanged("CanZoomIn");
            }
        }

        /// <summary>
        /// Gets or sets the value that determines whether zoom out can be performed.
        /// </summary>
        public bool CanZoomOut
        {
            get
            {
                return _canZoomOut;
            }
            set
            {
                _canZoomOut = value;
                OnPropertyChanged("CanZoomOut");
            }
        }

        /// <summary>
        /// Validates the current zoom against the minimum and maximum zoom values, and determines whether further zoom can be performed.
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
            else if (_currentZoom <= _minZoom && _currentZoom < _maxZoom)
            {
                CanZoomIn = true;
                CanZoomOut = false;
            }
            else if (_currentZoom >= _maxZoom && _currentZoom > _minZoom)
            {
                CanZoomIn = false;
                CanZoomOut = true;
            }
        }

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
                return _pdfDocumentStream;
            }
            set
            {
                _pdfDocumentStream = value;
                OnPropertyChanged("PdfDocumentStream");
            }
        }

        /// <summary>
        /// Constructor of the view model class
        /// </summary>
        public PdfViewerViewModel()
        {
            PdfDocumentStream = this.GetType().Assembly.GetManifestResourceStream("Magnification.Assets.HTTP_Succinctly.pdf");
            //Initialize the zoom percentages with some custom values.
            ZoomPercentages = new ObservableCollection<double>()
            {
                50,75,100,125,150,200,300,400,800
            };
        }

        /// <summary>
        /// Invokes the property changed event
        /// </summary>
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
