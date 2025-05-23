﻿namespace Hospital.Utils
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;

    /// <summary>
    /// Converts TimeSpan to string and vice versa.
    /// </summary>
    public class TimeSpanToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts a TimeSpan value to a string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>The converted value as a string.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TimeSpan timeSpan)
            {
                return timeSpan.ToString(@"hh\:mm");
            }

            return string.Empty;
        }

        /// <summary>
        /// Converts a string value back to a TimeSpan.
        /// </summary>
        /// <param name="value">The value to convert back.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>The converted value as a TimeSpan, or UnsetValue on failure.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string timeString &&
                TimeSpan.TryParseExact(timeString, @"hh\:mm", null, out TimeSpan parsedTime))
            {
                return parsedTime;
            }

            // Prevents binding loop when invalid input is typed
            return DependencyProperty.UnsetValue;
        }
    }
}
