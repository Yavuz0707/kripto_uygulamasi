using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace CryptoGuard.MAUI.Converters
{
    public class ChangeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Colors.Gray;
            if (value is decimal dec)
            {
                if (dec > 0)
                    return Colors.LimeGreen;
                if (dec < 0)
                    return Colors.IndianRed;
                return Colors.Gray;
            }
            if (value is double d)
            {
                if (d > 0)
                    return Colors.LimeGreen;
                if (d < 0)
                    return Colors.IndianRed;
                return Colors.Gray;
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 