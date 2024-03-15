using Syncfusion.Maui.PdfViewer;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;

namespace ElectronicSignature
{
    public class PdfViewerViewModel : INotifyPropertyChanged
    {
        private PdfFileData? fileData;
        public Command OpenCommand { get; }
        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        
        async void OnOpenCommand()
        {
            var fileData = await FileService.OpenFile("pdf");
            if (fileData != null)
            {
                FileData = fileData;
            }
        }
        public PdfFileData? FileData
        {
            get { return fileData; }
            set
            {
                if (fileData != value)
                {
                    fileData = value;
                    OnPropertyChanged(nameof(FileData));
                }
            }
        }

        public PdfViewerViewModel()
        {
            Stream? PdfDocumentStream = this.GetType().Assembly.GetManifestResourceStream("ElectronicSignature.handwritten-signature.pdf");
            FileData = new PdfFileData("handwritten-signature.pdf", PdfDocumentStream);
            OpenCommand = new Command(OnOpenCommand);

        }

        /// <summary>
        /// Trigges the property changed event.
        /// </summary>
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
                       
    }
}