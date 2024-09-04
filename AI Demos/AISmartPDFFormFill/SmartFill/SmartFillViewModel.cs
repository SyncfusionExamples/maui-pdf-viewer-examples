using System.ComponentModel;

namespace SmartFill
{
    public class SmartFillViewModel : INotifyPropertyChanged
    {
        public string UserDetail1 { get; set; }
        public string UserDetail2 { get; set; }
        public string UserDetail3 { get; set; }
        // Backing field for the PdfFile property
        private Stream? _pdfFile;
        // Property to hold the PDF file stream
        public Stream? PdfFile
        {
            get { return _pdfFile; }
            set
            {
                _pdfFile = value;
                // Notify that PdfFile property has changed
                OnPropertyChange(nameof(PdfFile));
            }
        }

        // Event handler for property changes
        public event PropertyChangedEventHandler? PropertyChanged;

        // Method to notify listeners of property changes
        private void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Constructor to initialize the view model
        public SmartFillViewModel()
        {
            string basePath = "SmartFill.PDF";
            // Load a PDF file as a resource stream
            _pdfFile = this.GetType().Assembly.GetManifestResourceStream($"{basePath}.smart-form.pdf");
            UserDetail1 = "Hi, this is John. You can contact me at john123@emailid.com. I am male, born on February 20, 2005. I want to subscribe to a newspaper and learn courses, specifically a Machine Learning course. I am from Alaska.";
            UserDetail2 = "S David here. You can reach me at David123@emailid.com. I am male, born on March 15, 2003. I would like to subscribe to a newspaper and am interested in taking a Digital Marketing course. I am from New York.";
            UserDetail3 = "Hi, this is Alice. You can contact me at alice456@emailid.com. I am female, born on July 15, 1998. I want to unsubscribe from a newspaper and learn courses, specifically a Cloud Computing course. I am from Texas.";
        }
    }
}