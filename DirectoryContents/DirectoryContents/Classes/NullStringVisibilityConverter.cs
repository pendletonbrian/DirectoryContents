using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DirectoryContents.Classes
{
    public class NullStringVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = string.IsNullOrWhiteSpace(value.ToString()).Equals(false);

            if (ConverterMethods.IsVisibilityInverted(parameter))
            {
                isVisible = !isVisible;
            }

            return (isVisible ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = ((Visibility)value == Visibility.Visible);

            // If visibility is inverted by the converter parameter, then invert
            // our value
            if (ConverterMethods.IsVisibilityInverted(parameter))
            {
                isVisible = !isVisible;
            }

            return isVisible;
        }
    }
}