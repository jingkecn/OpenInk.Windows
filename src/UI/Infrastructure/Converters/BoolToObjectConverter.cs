using System;
using Windows.UI.Xaml.Data;

namespace MyScript.OpenInk.UI.Infrastructure.Converters
{
    public class BoolToObjectConverter : IValueConverter
    {
        public object FalseValue { get; set; }
        public object TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var result = value is bool b && b;
            // Negate if needed
            if (parameter is string boolString && bool.TryParse(boolString, out var invert) && invert)
            {
                result = !result;
            }

            return result ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var result = Equals(value, TrueValue);
            // Negate if needed
            if (parameter is string boolString && bool.TryParse(boolString, out var invert) && invert)
            {
                result = !result;
            }

            return result;
        }
    }
}
