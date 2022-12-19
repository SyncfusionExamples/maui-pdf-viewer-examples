using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;

namespace OpenLocalFile
{
    internal class PdfViewerViewModel : INotifyPropertyChanged
    {
        private Stream m_pdfDocumentStream;
        private ICommand m_openDocumentCommand;

        /// <summary>
        /// An event to detect the change in the value of a property.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Command to open document using a file picker.
        /// </summary>
        public ICommand OpenDocumentCommand
        {
            get
            {
                if (m_openDocumentCommand == null)
                    m_openDocumentCommand = new Command<object>(OpenDocument);
                return m_openDocumentCommand;
            }
        }

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
            //Accessing the PDF document that is added as embedded resource as stream.
            m_pdfDocumentStream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("OpenLocalFile.Assets.PDF_Succinctly.pdf");
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Opens a document using file picker.
        /// </summary>
        async void OpenDocument(object commandParameter)
        {
            //Create file picker with file type as PDF.
            FilePickerFileType pdfFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>{
                        { DevicePlatform.iOS, new[] { "public.pdf" } },
                        { DevicePlatform.Android, new[] { "application/pdf" } },
                        { DevicePlatform.WinUI, new[] { "pdf" } },
                        { DevicePlatform.MacCatalyst, new[] { "pdf" } },
                    });

            //Provide picker title if required.
            PickOptions options = new()
            {
                PickerTitle = "Please select a PDF file",
                FileTypes = pdfFileType,
            };
            await PickAndShow(options);
        }

        /// <summary>
        /// Picks the file from local storage and store as stream.
        /// </summary>
        public async Task PickAndShow(PickOptions options)
        {
            try
            {
                //Pick the file from local storage.
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    //Store the resultant PDF as stream.
                    PdfDocumentStream = await result.OpenReadAsync();
                }
            }
            catch (Exception ex)
            {
                //Display error when file picker failed to open files.
                string message;
                if (ex != null && string.IsNullOrEmpty(ex.Message) == false)
                    message = ex.Message;
                else
                    message = "File open failed.";
                Application.Current?.MainPage?.DisplayAlert("Error", message, "OK");
            }
        }
    }
}