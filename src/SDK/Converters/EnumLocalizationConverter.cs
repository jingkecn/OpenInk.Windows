using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MyScript.InteractiveInk.Converters
{
    /// <summary>
    ///     Provides a type converter to convert <see cref="T:System.Enum"></see> objects to localized descriptions.
    /// </summary>
    public class EnumLocalizationConverter : EnumConverter
    {
        public EnumLocalizationConverter(Type type) : base(type)
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (!CanConvertTo(context, destinationType))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            var info = value?.GetType().GetField(value.ToString());
            if (info == null)
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            var attributes = info.GetCustomAttributes<DisplayNameAttribute>();
            return attributes.SingleOrDefault()?.DisplayName ?? value;
        }
    }
}
