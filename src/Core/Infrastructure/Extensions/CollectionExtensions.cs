using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using MyScript.InteractiveInk.Annotations;

namespace MyScript.OpenInk.Core.Infrastructure.Extensions
{
    public static partial class CollectionExtensions
    {
        public static Collection<TSource> SortBy<TSource, TKey>([NotNull] this Collection<TSource> source,
            Func<TSource, TKey> selector)
        {
            var sorted = source.OrderBy(selector).ToImmutableList();
            for (var index = 0; index < sorted.Count; index++)
            {
                var item = sorted[index];
                if (source.IndexOf(item) == index)
                {
                    continue;
                }

                source.Remove(item);
                source.Insert(index, item);
            }

            return source;
        }
    }

    public static partial class CollectionExtensions
    {
        /// <summary>
        ///     Sync a collection with:
        ///     - Items to add,
        ///     - Items to remove.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <param name="source">The sequence collection in which to sync.</param>
        /// <param name="itemsToAdd">Items to add.</param>
        /// <param name="itemsToRemove">Items to remove.</param>
        /// <returns>The sequence collection</returns>
        public static ICollection<TSource> Sync<TSource>([NotNull] this ICollection<TSource> source,
            [CanBeNull] IEnumerable<TSource> itemsToAdd,
            [CanBeNull] IEnumerable<TSource> itemsToRemove)
        {
            itemsToAdd?.ToList().ForEach(source.Add);
            itemsToRemove?.ToList().ForEach(item => source.Remove(item));
            return source;
        }

        /// <summary>
        ///     Sync a collection with another collection.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <param name="source">A sequence in which to sync.</param>
        /// <param name="another">A sequence with which to sync.</param>
        /// <param name="comparer">An equality comparer to compare values.</param>
        /// <returns>The sequence collection</returns>
        public static ICollection<TSource> SyncWith<TSource>([NotNull] this ICollection<TSource> source,
            [NotNull] IEnumerable<TSource> another,
            [CanBeNull] IEqualityComparer<TSource> comparer = null)
        {
            var reference = another.ToList();
            var itemsToAdd = from item in reference
                // ReSharper disable once ImplicitlyCapturedClosure
                where comparer == null ? !source.Contains(item) : !source.Contains(item, comparer)
                select item;
            var itemsToRemove = from item in source
                // ReSharper disable once ImplicitlyCapturedClosure
                where comparer == null ? !reference.Contains(item) : !reference.Contains(item, comparer)
                select item;
            return source.Sync(itemsToAdd, itemsToRemove);
        }
    }

    public static partial class CollectionExtensions
    {
        public static void DisposeAll([NotNull] this IEnumerable<IDisposable> source)
        {
            foreach (var disposable in source)
            {
                disposable.Dispose();
            }
        }
    }
}
