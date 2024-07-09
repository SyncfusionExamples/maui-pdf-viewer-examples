using System.ComponentModel;

namespace Localization
{
    internal class RightToLeftViewModel : INotifyPropertyChanged
    {
        private Stream? _documentStream;
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the PDF document as a stream. 
        /// </summary>
        public Stream? DocumentStream
        {
            get => _documentStream;
            set
            {
                _documentStream = value;
                OnPropertyChanged("DocumentStream");
            }
        }

        /// <summary>
        /// Constructor of the view model class
        /// </summary>
        public RightToLeftViewModel()
        {
            DocumentStream = this.GetType().Assembly.GetManifestResourceStream("Localization.Assets.rtl_document.pdf");
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}