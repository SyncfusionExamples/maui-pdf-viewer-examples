using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace PdfThumbnailViewer.Models
{
    /// <summary>
    /// Represents a single PDF page thumbnail, including its preview image,
    /// navigation command, page number, and selection state for highlighting in the UI.
    /// </summary>
    public class PageThumbnail : INotifyPropertyChanged
    {
        private bool _isSelected;

        /// <summary>Gets or sets the page number for this thumbnail.</summary>
        public int PageNumber { get; set; }

        /// <summary>Gets or sets the image source representing the thumbnail preview.</summary>
        public ImageSource? ThumbnailImage { get; set; }

        /// <summary>Gets or sets the command used to navigate to this page when selected.</summary>
        public ICommand? NavigateToPageCommand { get; set; }
        
        /// <summary>
        /// Gets or sets whether this thumbnail is currently selected (highlighted in the UI).
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises property change notifications for data binding.
        /// </summary>
        /// <param name="propertyName">The updated property name.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
