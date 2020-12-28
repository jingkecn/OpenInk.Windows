using System;
using System.Collections.Generic;
using MyScript.InteractiveInk.Annotations;

namespace MyScript.InteractiveInk.Extensions
{
    internal static class CollectionExtensions
    {
        public static void Dispose([NotNull] this IEnumerable<IDisposable> source)
        {
            foreach (var disposable in source)
            {
                disposable.Dispose();
            }
        }
    }
}
