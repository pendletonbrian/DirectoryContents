using System;
using System.Windows;
using System.Windows.Data;

namespace DirectoryContents.Classes
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isVisible = (bool)value;

            if (ConverterMethods.IsVisibilityInverted(parameter))
            {
                isVisible = !isVisible;
            }

            return (isVisible ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
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
