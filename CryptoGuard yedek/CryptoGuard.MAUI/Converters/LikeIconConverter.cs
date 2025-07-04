using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace CryptoGuard.MAUI.Converters
{
    public class LikeIconConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isLiked && isLiked)
                return "like_filled.png";
            return "like_icon.png";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 