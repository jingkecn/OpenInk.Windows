using System;
using System.ComponentModel;
using Windows.UI.Xaml.Data;

namespace MyScript.OpenInk.Core.Infrastructure.Converters
{
    public class TypeDescriptorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return TypeDescriptor.GetConverter(value).ConvertToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
