using System.ComponentModel;

namespace CustomSignatureDialog
{   
    public class PdfViewerViewModel : INotifyPropertyChanged
    {
        // Field for FileData property
        private PdfFileData? fileData;

        // Command to open a PDF file
        public Command OpenCommand { get; }

        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        // Method executed when the OpenCommand is invoked
        async void OnOpenCommand()
        {
            // Call service to open a PDF file
            var fileData = await FileService.OpenFile("pdf");
            if (fileData != null)
            {
                FileData = fileData; // Set the returned file data
            }
        }

        // Property for managing file data in the ViewModel
        public PdfFileData? FileData
        {
            get { return fileData; }
            set
            {
                if (fileData != value)
                {
                    fileData = value; // Update file data
                    OnPropertyChanged(nameof(FileData)); // Notify property change
                }
            }
        }

        // Constructor initializing the ViewModel
        public PdfViewerViewModel()
        {
            // Load a PDF from embedded resources
            Stream? PdfDocumentStream = this.GetType().Assembly.GetManifestResourceStream("CustomSignatureDialog.Assets.handwritten-signature.pdf");
            FileData = new PdfFileData("handwritten-signature.pdf", PdfDocumentStream); // Initialize with embedded PDF
            OpenCommand = new Command(OnOpenCommand); // Assign the open command
        }

        /// <summary>
        /// Triggers the property changed event.
        /// </summary>
        public void OnPropertyChanged(string name)
        {
            // Invoke the PropertyChanged event for the specified property
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}