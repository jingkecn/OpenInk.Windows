using System;
using Windows.UI.Xaml.Data;

namespace MyScript.OpenInk.UI.Infrastructure.Converters
{
    /// <summary>
    ///     Converts a file size in bytes to a more human-readable friendly format using
    ///     <see cref="Microsoft.Toolkit.Converters.ToFileSizeString(long)" />
    /// </summary>
    public class FileSizeToFriendlyStringConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Microsoft.Toolkit.Converters.ToFileSizeString(System.Convert.ToInt64(value));
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
