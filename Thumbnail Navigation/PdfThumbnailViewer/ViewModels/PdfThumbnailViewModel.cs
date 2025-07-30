using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PdfThumbnailViewer.Models;
using Syncfusion.Maui.PdfToImageConverter;
using Syncfusion.Maui.PdfViewer;

namespace PdfThumbnailViewer.ViewModels
{
    /// <summary>
    /// ViewModel responsible for handling PDF document loading,
    /// thumbnail generation, page navigation, and the state required for UI binding.
    /// </summary>
    public class PdfThumbnailViewModel : INotifyPropertyChanged
    {
        // Underlying Syncfusion PDF viewer UI instance
        private SfPdfViewer? _pdfViewer;
        // Backing list for thumbnail view models
        private ObservableCollection<PageThumbnail> _thumbnails;
        // The currently selected (highlighted) thumbnail
        private PageThumbnail? _selectedThumbnail;
        // PDF file stream (embedded resource)
        private Stream? _pdfDocumentStream;
        // Tracks whether loading operations are in progress
        private bool _isLoading;
        // Total PDF page count
        private int _totalPages;
        // Whether the thumbnail strip is expanded
        private bool _thumbnailsMaximized = true;
        // Syncfusion converter for generating thumbnail images
        private PdfToImageConverter? _pdfToImageConverter = null;

        /// <summary>
        /// Initializes properties, commands, and collections required for thumbnail viewing.
        /// </summary>
        public PdfThumbnailViewModel()
        {
            _thumbnails = new ObservableCollection<PageThumbnail>();
            NavigateToPageCommand = new Command<PageThumbnail>(OnNavigateToPage);
            ToggleThumbnailsCommand = new Command(() => ThumbnailsMaximized = !ThumbnailsMaximized);
            IsLoading = false;
        }

        /// <summary>
        /// Registers the Syncfusion PDF Viewer and attaches document loaded event handler.
        /// </summary>
        /// <param name="pdfViewer">Displayed PDF viewer control</param>
        public void Initialize(SfPdfViewer pdfViewer)
        {
            _pdfViewer = pdfViewer;
            _pdfViewer.DocumentLoaded += PdfViewer_DocumentLoaded;
        }

        /// <summary>
        /// Loads the embedded PDF document and prepares the converter. Triggers loading spinner.
        /// </summary>
        /// <param name="resourceName">Name of the embedded PDF resource</param>
        public void LoadPdfDocument(string resourceName)
        {
            IsLoading = true;
            Thumbnails.Clear();
            Assembly assembly = typeof(App).GetTypeInfo().Assembly;
            _pdfDocumentStream = assembly.GetManifestResourceStream(resourceName);

            if (_pdfDocumentStream != null && _pdfViewer != null)
            {
                _pdfViewer.DocumentSource = _pdfDocumentStream;
                _pdfDocumentStream.Position = 0;
                _pdfToImageConverter = new PdfToImageConverter(_pdfDocumentStream);
            }
        }

        /// <summary>
        /// Handles the event when a document finishes loading in the viewer.
        /// Begins thumbnail generation for the entire PDF.
        /// </summary>
        private async void PdfViewer_DocumentLoaded(object? sender, EventArgs? e)
        {
            if (_pdfViewer == null) return;
            _totalPages = _pdfViewer.PageCount;
            await GeneratePlaceholderThumbnailsAsync();
            if (Thumbnails.Count > 0)
            {
                SelectedThumbnail = Thumbnails[0];
                SelectedThumbnail.IsSelected = true;
            }
        }

        /// <summary>
        /// Asynchronously generates thumbnail images for every PDF page and adds them to the UI-bound list.
        /// </summary>
        private async Task GeneratePlaceholderThumbnailsAsync()
        {
            Thumbnails.Clear();
            if (_pdfToImageConverter == null)
                return;
            for (int i = 1; i <= _totalPages; i++)
            {
                // Convert page to a low-res thumbnail image stream
                Stream? pageStream = await _pdfToImageConverter.ConvertAsync(i - 1, scaleFactor: 0.25f);
                PageThumbnail thumbnail = new PageThumbnail
                {
                    PageNumber = i,
                    ThumbnailImage = ImageSource.FromStream(() => pageStream),
                    NavigateToPageCommand = NavigateToPageCommand,
                    IsSelected = (i == 1)
                };
                Thumbnails.Add(thumbnail);
                if (i == 1)
                    IsLoading = false;
            }
        }

        /// <summary>
        /// Navigates to the selected page in the viewer and updates the selection highlight state.
        /// </summary>
        /// <param name="thumbnail">The thumbnail representing the PDF page.</param>
        private void OnNavigateToPage(PageThumbnail thumbnail)
        {
            if (_pdfViewer != null && thumbnail != null)
            {
                _pdfViewer.GoToPage(thumbnail.PageNumber);
                foreach (var item in Thumbnails)
                    item.IsSelected = (item.PageNumber == thumbnail.PageNumber);
                SelectedThumbnail = thumbnail;
            }
        }

        /// <summary>
        /// List of page thumbnails, bound to the UI.
        /// </summary>
        public ObservableCollection<PageThumbnail> Thumbnails
        {
            get => _thumbnails;
            set
            {
                _thumbnails = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The currently selected (highlighted) thumbnail, bound to the UI.
        /// </summary>
        public PageThumbnail? SelectedThumbnail
        {
            get => _selectedThumbnail;
            set
            {
                if (_selectedThumbnail != value)
                {
                    _selectedThumbnail = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Indicates if the document or thumbnails are loading (shows spinner in UI).
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Command to navigate to the selected page via a thumbnail tap.
        /// </summary>
        public ICommand NavigateToPageCommand { get; private set; }
        /// <summary>
        /// Command to toggle the minimized or maximized state of the thumbnail pane.
        /// </summary>
        public ICommand ToggleThumbnailsCommand { get; private set; }

        /// <summary>
        /// If true, thumbnail pane is fully expanded. If false, minimized.
        /// </summary>
        public bool ThumbnailsMaximized
        {
            get => _thumbnailsMaximized;
            set
            {
                if (_thumbnailsMaximized != value)
                {
                    _thumbnailsMaximized = value;
                    OnPropertyChanged();
                }
            }
        }

        #region INotifyPropertyChanged

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>
        /// Notifies the UI about property changes for data binding.
        /// </summary>
        /// <param name="propertyName">Name of changed property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}