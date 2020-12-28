using System;
using Windows.UI.Xaml.Data;
using MyScript.OpenInk.Core.Infrastructure.Extensions;

namespace MyScript.OpenInk.Core.Infrastructure.Converters
{
    public class StringLocalizationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return parameter is string key ? value is null ? key.Localize() : key.Localize(value) : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
