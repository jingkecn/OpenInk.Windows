using System;
using Windows.UI.Xaml.Data;

namespace MyScript.InteractiveInk.Converters
{
    public class BoolToObjectConverter : IValueConverter
    {
        public object FalseValue { get; set; }
        public object TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is bool b && b ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value == TrueValue;
        }
    }
}
