using Syncfusion.Maui.PdfViewer;
using System.ComponentModel;
using System.Windows.Input;

namespace Stickynote
{
    public class PdfViewerViewModel : INotifyPropertyChanged
    {
        private ICommand _openFileCommand;
        private Stream _documentStream;
        private float _selectedThickness;
        private float _selectedOpacity;
        private float _selectedFillColorOpacity;
        private bool _isSelectedAnnotationClosed;
        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

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
        /// Gets the command to browse file in the disk.
        /// </summary>
        public ICommand OpenFileCommand => _openFileCommand;
        
        public PdfViewerViewModel()
        {
            PdfDocumentStream = this.GetType().Assembly.GetManifestResourceStream("Stickynote.Assets.Stickynote.pdf");
            _openFileCommand = new Command<object>(OpenFile);
          
        }

        /// <summary>
        /// Trigges the property changed event.
        /// </summary>
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public float SelectedThickness
        {
            get => _selectedThickness;
            set
            {
                _selectedThickness = value;
                OnPropertyChanged("SelectedThickness");
            }
        }

        public float SelectedFillColorOpacity
        {
            get => _selectedFillColorOpacity;
            set
            {
                _selectedFillColorOpacity = value;
                OnPropertyChanged("SelectedFillColorOpacity");
            }
        }

        public bool IsSelectedAnnotationClosed
        {
            get => _isSelectedAnnotationClosed;
            set
            {
                _isSelectedAnnotationClosed = value;
                OnPropertyChanged("IsSelectedAnnotationClosed");
            }
        }

        public float SelectedOpacity
        {
            get => _selectedOpacity;
            set
            {
                _selectedOpacity = value;
                OnPropertyChanged("SelectedOpacity");
            }
        }

        /// <summary>
        /// Opens the file from the disk and create document stream from it.
        /// </summary>
        async internal void OpenFile(object commandParameter)
        {
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