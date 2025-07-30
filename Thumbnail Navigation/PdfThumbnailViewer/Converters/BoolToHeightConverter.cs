using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PdfThumbnailViewer.Converters
{
    /// <summary>
    /// Converter that maps a boolean value (expanded/collapsed) to a numeric height for UI panels.
    /// Used for toggleable thumbnail strips where expanded/collapsed height is data-bound in XAML.
    /// </summary>
    public class BoolToHeightConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean to height.
        /// Passes in parameter format: "max,min" (e.g., "150,30").
        /// Returns maxHeight if true, minHeight if false. Defaults to 50.0 if parsing fails.
        /// </summary>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool maximized && parameter is string heights)
            {
                var arr = heights.Split(',');
                if (arr.Length == 2 && double.TryParse(arr[0], out double maxH) && double.TryParse(arr[1], out double minH))
                {
                    return maximized ? maxH : minH;
                }
            }
            return 50d; // Default fallback
        }
        /// <summary>
        /// Not implemented (one-way conversion only).
        /// </summary>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
