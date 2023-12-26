using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FormFilling
{
    internal class ViewModal : INotifyPropertyChanged
    {
        private ICommand _openFileCommand;
        private Stream _documentStream;
        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
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
        public ViewModal()
        {
            PdfDocumentStream = this.GetType().Assembly.GetManifestResourceStream("FormFilling.Assets.form_document.pdf");
            _openFileCommand = new Command<object>(OpenFile);
        }
        /// <summary>
        /// Trigges the property changed event.
        /// </summary>
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        /// <summary>
        /// Opens the file from the disk and create document stream from it.
        /// </summary>
        async internal void OpenFile(object commandParameter)
        {
            FilePickerFileType pdfFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>{
                        { DevicePlatform.iOS, new[] { "com.adobe.pdf" } },
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
