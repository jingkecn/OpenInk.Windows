using System;
using Windows.UI.Xaml.Data;

namespace MyScript.OpenInk.UI.Infrastructure.Converters
{
    public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var sourceType = value.GetType();
            return sourceType.IsEnum && parameter is string name &&
                   Enum.TryParse(sourceType, name, true, out var parsed) && Equals(parsed, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
