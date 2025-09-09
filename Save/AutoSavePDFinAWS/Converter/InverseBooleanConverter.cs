using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSavePDFinAWS
{
    /// <summary>
    /// Value converter that inverts boolean values for data binding scenarios.
    /// Used in XAML to bind UI elements that need the opposite of a boolean property.
    /// For example, enabling a Save button when Auto-Save is disabled.
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to its inverse.
        /// </summary>
        /// <param name="value">The boolean value to convert</param>
        /// <param name="targetType">The type of the binding target property (must be bool)</param>
        /// <param name="parameter">Optional parameter for the converter (not used)</param>
        /// <param name="culture">Culture information for the conversion (not used)</param>
        /// <returns>The inverted boolean value</returns>
        /// <exception cref="InvalidOperationException">Thrown when target type is not boolean</exception>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Validate that the target type is boolean
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            // Return the inverse of the input boolean value
            return value is bool boolValue ? !boolValue : false;
        }

        /// <summary>
        /// Converts back from the inverted value to the original (not supported).
        /// This converter is designed for one-way binding only.
        /// </summary>
        /// <param name="value">The value to convert back</param>
        /// <param name="targetType">The type to convert to</param>
        /// <param name="parameter">Optional parameter for the converter</param>
        /// <param name="culture">Culture information for the conversion</param>
        /// <returns>Not supported</returns>
        /// <exception cref="NotSupportedException">Always thrown as this operation is not supported</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("InverseBooleanConverter does not support ConvertBack operation");
        }
    }
}
