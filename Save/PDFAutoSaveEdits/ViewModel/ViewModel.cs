using Microsoft.Maui.Storage;
using Syncfusion.Maui.PdfViewer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PDFAutoSaveEdits
{
    internal class ViewModel : INotifyPropertyChanged
    {
        private SfPdfViewer _pdfViewer;
        private string _currentFilePath = string.Empty;
        private string _currentFileName = string.Empty;
        private string _notificationText = "No PDF loaded. Please click the “Open” button to select a PDF file.";
        private bool _isAutoSaveEnabled = true;
        private bool _isDocumentLoaded = false;

        public ViewModel()
        {
            OpenPdfCommand = new Command(OpenPdf);
            SavePdfCommand = new Command(SavePdf);
        }

        public bool IsDocumentLoaded
        {
            get => _isDocumentLoaded;
            set
            {
                if (_isDocumentLoaded != value)
                {
                    if(value == false)
                    {
                        NotficationText = _currentFileName + " - Closed";
                    }
                    else
                    {
                        NotficationText = _currentFileName+ " - Opened";
                    }
                    _isDocumentLoaded = value;
                }
            }
        }

        public string NotficationText
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

        public bool IsAutoSaveEnabled
        {
            get => _isAutoSaveEnabled;
            set
            {
                _isAutoSaveEnabled = value;
                OnPropertyChanged();
            }
        }

        public SfPdfViewer PdfViewer
        {
            get => _pdfViewer;
            set => _pdfViewer = value;
        }

        public Command OpenPdfCommand { get; }
        public Command SavePdfCommand { get; }

        private async void OpenPdf()
        {
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                        { DevicePlatform.Android, new[] { "application/pdf" } },
                        { DevicePlatform.iOS, new[] { "com.adobe.pdf" } },
                        { DevicePlatform.WinUI, new[] { ".pdf" } },
                        { DevicePlatform.macOS, new[] { "pdf" } },
                });

            var options = new PickOptions()
            {
                PickerTitle = "Please select a PDF file",
                FileTypes = customFileType,
            };

            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                var stream = await result.OpenReadAsync();
                PdfViewer.DocumentSource = stream;
                _currentFilePath = result.FullPath;
                _currentFileName = result.FileName;
            }
            NotficationText = "Opening - "+ _currentFileName + "...";
        }

        private async void SavePdf()
        {
            if (PdfViewer != null)
            {
                NotficationText = "Saving - " + _currentFileName + "...";

                using var saveStream = new MemoryStream();
                await PdfViewer.SaveDocumentAsync(saveStream);

                using (var fileStream = new FileStream(_currentFilePath, FileMode.Open, FileAccess.Write))
                {
                    saveStream.Position = 0;
                    await saveStream.CopyToAsync(fileStream);
                }
                NotficationText = _currentFileName + " - Saved";
            }
        }

        public void OnDocumentEdited()
        {
            if (IsAutoSaveEnabled)
            {
                SavePdf();
            }
            else
            {
                NotficationText = _currentFileName + " - Edited";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
