using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources.Core;
using MyScript.InteractiveInk.Annotations;

namespace MyScript.OpenInk.Core.Infrastructure.Extensions
{
    public static partial class StringExtensions
    {
        [NotNull]
        public static string Localize([NotNull] this string source)
        {
            return ResourceManager.Current.MainResourceMap.GetValue($"Resources/{source}").ValueAsString;
        }

        [NotNull]
        public static string Localize([NotNull] this string source, params object[] args)
        {
            var result = ResourceManager.Current.MainResourceMap.GetValue($"Resources/{source}").ValueAsString;
            return string.Format(result, args);
        }
    }

    public static partial class StringExtensions
    {
        [NotNull]
        public static IDictionary<string, string> ToArguments([NotNull] this string source, char separator = '&',
            char assignmentOperator = '=')
        {
            return source
                .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(segment => segment.Split(assignmentOperator))
                .Where(pair => pair.Length == 2)
                .ToDictionary(pair => pair[0], pair => pair[1]);
        }
    }
}
