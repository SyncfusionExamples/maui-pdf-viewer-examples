using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Reflection;

namespace OpenAndSavePDF
{
    class PdfViewerViewModel : INotifyPropertyChanged
    {
        private Stream? m_pdfDocumentStream;
        //public IRelayCommand<Stream> SaveChangesCommand { get; }

        /// <summary>
        /// An event to detect the change in the value of a property.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// The PDF document stream that is loaded into the instance of the PDF viewer. 
        /// </summary>
        public Stream? PdfDocumentStream
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
        public PdfViewerViewModel()
        {
            m_pdfDocumentStream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("OpenAndSavePDF.Assets.PDF_Succinctly.pdf");
        }
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public async void SavePDFDocument(Stream saveStream)
        {
            try
            {
                string fileName = "SavedPDF.pdf";
                string filePath;
                // Set the file path to save the saved pdf document.
#if WINDOWS
                filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName); // Define the file path for Windows.
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    saveStream.CopyTo(fileStream); // Copy the stream to the file stream.
                }
#elif ANDROID
                filePath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads)!.AbsolutePath, fileName);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    saveStream.CopyTo(fileStream); // Copy the stream to the file stream.
                }
#else
                filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName); // Define the file path for Android, iOS, and Mac Catalyst.
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    saveStream.CopyTo(fileStream); // Copy the stream to the file stream.
                }
#endif
                await Application.Current!.Windows[0].Page!.DisplayAlert("File Saved", filePath + " is successfully saved", "OK"); // Display a success message.       
            }
            catch (Exception ex)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlert( "File not saved", $"Error saving document: {ex.Message}", "OK"); // Display the error message if the file is not saved.
            }
        }

        public async void OpenDocument()
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
        async Task PickAndShow(PickOptions options)
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
                Application.Current?.Windows[0].Page?.DisplayAlert("Error", message, "OK");
            }
        }
    }
}