using System;
using System.Globalization;
using System.Windows.Data;

namespace Capybook.Utilities
{
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double discount)
            {
                return $"{discount}%";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string discountText && discountText.EndsWith("%"))
            {
                if (double.TryParse(discountText.TrimEnd('%'), out double discount))
                {
                    return discount;
                }
            }
            return 0.0;
        }
    }
}
