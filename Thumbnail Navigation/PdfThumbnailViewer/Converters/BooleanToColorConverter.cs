using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace PdfThumbnailViewer.Converters
{
    /// <summary>
    /// Converter that maps a boolean selection state to a color for UI highlighting.
    /// Used to highlight selected thumbnails or controls.
    /// </summary>
    public class BooleanToColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value (typically selection state) to a color used for UI binding.
        /// If parameters are specified, parses those as custom color strings (selected,unselected).
        /// </summary>
        /// <param name="value">True if selected; otherwise, false.</param>
        /// <param name="targetType">Required for IValueConverter. (ignored)</param>
        /// <param name="parameter">Optional: Comma separated colors. E.g., "#FF0000,#CCCCCC"</param>
        /// <param name="culture">Current UI culture. (ignored)</param>
        /// <returns>A Microsoft.Maui.Graphics.Color instance for highlight.</returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isSelected)
            {
                if (parameter is string paramString)
                {
                    string[] colors = paramString.Split(',');
                    if (colors.Length >= 2)
                    {
                        return isSelected ? Color.Parse(colors[0]) : Color.Parse(colors[1]);
                    }
                }
                // Default highlight colors: blue for selected, grey for unselected
                return isSelected ? Color.FromArgb("#007bff") : Color.FromArgb("#e0e0e0");
            }
            return Color.FromArgb("#e0e0e0");
        }

        /// <summary>
        /// Not supported, used for one-way conversion only.
        /// </summary>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
