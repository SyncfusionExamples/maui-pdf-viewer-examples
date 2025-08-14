using Syncfusion.Maui.PdfViewer;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PDFAutoSaveEdits
{
    /// <summary>
    /// ViewModel class that handles PDF loading, saving, and auto-save functionality.
    /// Implements INotifyPropertyChanged to support data binding with the UI.
    /// </summary>
    internal class ViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        /// <summary>
        /// Reference to the Syncfusion PDF Viewer control for document operations
        /// </summary>
        private SfPdfViewer? _pdfViewer;

        /// <summary>
        /// Full path to the currently loaded PDF file
        /// </summary>
        private string _currentFilePath = string.Empty;

        /// <summary>
        /// Name of the currently loaded PDF file
        /// </summary>
        private string _currentFileName = string.Empty;

        /// <summary>
        /// Text displayed to user showing current operation status
        /// </summary>
        private string _notificationText = "No PDF loaded. Please click the OPEN button to select a PDF file.";

        /// <summary>
        /// Flag indicating whether auto-save is enabled
        /// </summary>
        private bool _isAutoSaveEnabled = true;

        /// <summary>
        /// Flag indicating whether a PDF document is currently loaded
        /// </summary>
        private bool _isDocumentLoaded = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ViewModel class.
        /// Sets up command bindings for UI interactions.
        /// </summary>
        public ViewModel()
        {
            OpenPdfCommand = new Command(OpenPdf);
            SavePdfCommand = new Command(SavePdf);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether a PDF document is currently loaded.
        /// Automatically updates the notification text when the value changes.
        /// </summary>
        public bool IsDocumentLoaded
        {
            get => _isDocumentLoaded;
            set
            {
                if (_isDocumentLoaded != value)
                {
                    // Update notification text based on document load state
                    if (value == false)
                    {
                        NotificationText = _currentFileName + " - Closed";
                    }
                    else
                    {
                        NotificationText = _currentFileName + " - Opened";
                    }
                    _isDocumentLoaded = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the notification text displayed to the user.
        /// Provides real-time feedback about current operations.
        /// </summary>
        public string NotificationText
        {
            get => _notificationText;
            set
            {
                if (_notificationText != value)
                {
                    _notificationText = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto-save is enabled.
        /// When true, PDF changes are automatically saved on edit.
        /// </summary>
        public bool IsAutoSaveEnabled
        {
            get => _isAutoSaveEnabled;
            set
            {
                if (_isAutoSaveEnabled != value)
                {
                    _isAutoSaveEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the reference to the Syncfusion PDF Viewer control.
        /// Used for document loading and saving operations.
        /// </summary>
        public SfPdfViewer? PdfViewer
        {
            get => _pdfViewer;
            set => _pdfViewer = value;
        }

        /// <summary>
        /// Command for opening/selecting a PDF file from device storage
        /// </summary>
        public Command OpenPdfCommand { get; }

        /// <summary>
        /// Command for manually saving the current PDF document
        /// </summary>
        public Command SavePdfCommand { get; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Opens a PDF file using the platform file picker.
        /// Configures platform-specific file types and loads the selected PDF into the viewer.
        /// </summary>
        private async void OpenPdf()
        {
            // Define platform-specific PDF file types for the file picker
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                        { DevicePlatform.Android, new[] { "application/pdf" } },
                        { DevicePlatform.iOS, new[] { "com.adobe.pdf" } },
                        { DevicePlatform.WinUI, new[] { ".pdf" } },
                        { DevicePlatform.MacCatalyst, new[] { "pdf" } },
                });

            // Configure file picker options
            var options = new PickOptions()
            {
                PickerTitle = "Please select a PDF file",
                FileTypes = customFileType,
            };

            NotificationText = "Opening file picker...";

            // Show file picker and get user selection
            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                // Open the selected file as a stream and load it into the PDF viewer
                var stream = await result.OpenReadAsync();
                if (PdfViewer != null)
                {
                    _currentFilePath = result.FullPath;
                    _currentFileName = result.FileName;
                    NotificationText = "Opening - " + _currentFileName + "...";
                    PdfViewer.DocumentSource = stream;
                }
                else
                {
                    NotificationText = "Error: PDF Viewer not initialized";
                }
            }
            else
            {
                NotificationText = "File selection cancelled";
            }
        }

        /// <summary>
        /// Saves the current PDF document to its original file location.
        /// Uses the Syncfusion PDF Viewer's SaveDocumentAsync method to preserve edits.
        /// </summary>
        private async void SavePdf()
        {
            if (PdfViewer != null && !string.IsNullOrEmpty(_currentFilePath))
            {
                FileInfo fileInfo = new FileInfo(_currentFilePath);
                if (fileInfo.IsReadOnly == false)
                {
                    NotificationText = "Saving - " + _currentFileName + "...";

                    // Create a memory stream to hold the saved PDF data
                    using var saveStream = new MemoryStream();

                    // Save the PDF document with all modifications to memory stream
                    await PdfViewer.SaveDocumentAsync(saveStream);

                    // Write the saved PDF data back to the original file
                    using (var fileStream = new FileStream(_currentFilePath, FileMode.Create, FileAccess.Write))
                    {
                        saveStream.Position = 0;
                        await saveStream.CopyToAsync(fileStream);
                    }

                    NotificationText = _currentFileName + " - Saved";
                }
                else
                {
                    NotificationText = _currentFileName + " is read-only and cannot be saved";
                }
            }
            else
            {
                NotificationText = "Error: No document loaded or invalid file path";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called when the PDF document is edited (annotations added, removed, or modified).
        /// Triggers auto-save if enabled, otherwise just updates the notification.
        /// </summary>
        public void OnDocumentEdited()
        {
            if (IsAutoSaveEnabled)
            {
                // Automatically save the document when auto-save is enabled
                SavePdf();
            }
            else
            {
                // Just notify user that document has been edited
                NotificationText = _currentFileName + " - Edited (Auto-save disabled)";
            }
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        /// <summary>
        /// Occurs when a property value changes
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event for data binding updates
        /// </summary>
        /// <param name="propertyName">Name of the property that changed (automatically provided by CallerMemberName)</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
