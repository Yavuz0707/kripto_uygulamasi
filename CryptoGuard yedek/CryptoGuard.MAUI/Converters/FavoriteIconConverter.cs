using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace CryptoGuard.MAUI.Converters
{
    public class FavoriteIconConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool isFavorite = value is bool b && b;
            // favori_dolu.png sarı ise bu kullanılacak, değilse kullanıcıdan yeni ikon istenebilir.
            return isFavorite ? "favori_dolu.png" : "favori_bos.png";
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
} 