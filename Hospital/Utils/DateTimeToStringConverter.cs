using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Hospital.Utils
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is DateTime dt ? dt.ToString("yyyy-MM-dd") : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string valueString &&
                DateTime.TryParseExact(valueString, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var result))
            {
                return result;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
