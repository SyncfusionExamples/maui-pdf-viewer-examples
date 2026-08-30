using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Localization
{
    public class PdfViewerViewModel : INotifyPropertyChanged
    {
        private string? selectedLanguage;

        public event PropertyChangedEventHandler? PropertyChanged;

        public PdfViewerViewModel()
        {
            // Populate available languages.
            Languages = new ObservableCollection<string> { "English", "German", "Arabic" };

            // Set default language.
            SelectedLanguage = "English";
        }

        public ObservableCollection<string> Languages { get; }

        public string? SelectedLanguage
        {
            get => selectedLanguage;
            set
            {
                if (selectedLanguage != value)
                {
                    selectedLanguage = value;
                    OnPropertyChanged(nameof(SelectedLanguage));
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}