using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PdfThumbnailViewer.Converters
{
    public class BoolToTextConverter : IValueConverter
    {
        // Parameter format: "Minimize,Maximize"
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool maximized)
            {
                return maximized ? "\ue72b": "\ue734"; // Unicode for minimize and maximize icons
            }
            return "Toggle";
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}