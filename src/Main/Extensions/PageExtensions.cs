using System;
using MyScript.IInk;
using MyScript.InteractiveInk.Annotations;
using MyScript.InteractiveInk.Extensions;
using MyScript.OpenInk.Core.Models;
using MyScript.OpenInk.Core.Services;
using MyScript.OpenInk.Main.Configuration;
using MyScript.OpenInk.Main.Models;

namespace MyScript.OpenInk.Main.Extensions
{
    public static class PageExtensions
    {
        private static IInteractiveInkServices InteractiveInkServices =>
            ServiceLocator.Current.Get<IInteractiveInkServices>();

        [NotNull]
        public static ContentPart ToNative(this IPage source)
        {
            if (InteractiveInkServices.EngineService.ContentPackage is not { } package)
            {
                throw new InvalidOperationException("The package is not initialized.");
            }

            var index = source.Index - 1;
            if (index < 0 || index >= package.PartCount)
            {
                throw new IndexOutOfRangeException($"Index {index} out of range: [0, {package.PartCount}).");
            }

            return package.GetPart(index);
        }

        public static IPage ToPlatform([NotNull] this ContentPart source)
        {
            if (InteractiveInkServices.EngineService.ContentPackage is not { } package)
            {
                throw new InvalidOperationException("The package is not initialized.");
            }

            var index = package.IndexOfPart(source);
            var type = source.Type.ToPlatformContentType();
            return new Page {Index = index + 1, Type = type};
        }
    }
}
