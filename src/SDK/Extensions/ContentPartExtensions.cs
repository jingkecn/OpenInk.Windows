using System;
using Windows.UI.Input;
using MyScript.IInk;
using MyScript.InteractiveInk.Annotations;
using MyScript.InteractiveInk.Enumerations;

namespace MyScript.InteractiveInk.Extensions
{
    public static partial class ContentPartExtensions
    {
        public static GestureSettings GetGestureSettings([NotNull] this ContentPart source)
        {
            var type = source.Type.ToPlatformContentType();
            var settings = GestureSettings.ManipulationScale |
                           GestureSettings.ManipulationScaleInertia |
                           GestureSettings.ManipulationTranslateInertia |
                           GestureSettings.ManipulationTranslateX |
                           GestureSettings.ManipulationTranslateY;
            switch (type)
            {
                case ContentType.Diagram:
                    break;
                case ContentType.Drawing:
                    break;
                case ContentType.Math:
                    settings ^= GestureSettings.ManipulationScale | GestureSettings.ManipulationScaleInertia;
                    break;
                case ContentType.RawContent:
                    break;
                case ContentType.Text:
                    settings ^= GestureSettings.ManipulationScale | GestureSettings.ManipulationScaleInertia |
                                GestureSettings.ManipulationTranslateX;
                    break;
                case ContentType.TextDocument:
                    settings ^= GestureSettings.ManipulationScale | GestureSettings.ManipulationScaleInertia |
                                GestureSettings.ManipulationTranslateX;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return settings;
        }

        public static Tuple<bool, bool> GetScrollBarVisibility([NotNull] this ContentPart source)
        {
            var type = source.Type.ToPlatformContentType();
            var isHorizontalScrollBarVisible = type != ContentType.Text && type != ContentType.TextDocument;
            return new Tuple<bool, bool>(isHorizontalScrollBarVisible, true);
        }
    }

    public static partial class ContentPartExtensions
    {
        [CanBeNull]
        public static ContentPart GetNext([NotNull] this ContentPart source)
        {
            if (!(source.Package is { } package))
            {
                return null;
            }

            var index = package.IndexOfPart(source);
            return index >= 0 && index < package.PartCount ? package.GetPart(++index) : null;
        }

        [CanBeNull]
        public static ContentPart GetPrevious([NotNull] this ContentPart source)
        {
            if (!(source.Package is { } package))
            {
                return null;
            }

            var index = package.IndexOfPart(source);
            return index > 0 && index < package.PartCount ? package.GetPart(--index) : null;
        }

        public static bool HasNext([NotNull] this ContentPart source)
        {
            var package = source.Package;
            return package.IndexOfPart(source) != package.PartCount - 1;
        }

        public static bool HasPrevious([NotNull] this ContentPart source)
        {
            var package = source.Package;
            return package.IndexOfPart(source) != 0;
        }
    }
}
