using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using MyScript.IInk.Graphics;
using MyScript.InteractiveInk.Annotations;

namespace MyScript.InteractiveInk.Extensions
{
    public static class ShapeExtensions
    {
        internal static Rect Clamp(this Rect source, [NotNull] FrameworkElement element)
        {
            return RectHelper.Intersect(LayoutInformation.GetLayoutSlot(element), source);
        }

        internal static bool IsValid(this Rect source)
        {
            return !source.IsEmpty && source.Width != 0 && source.Height != 0;
        }

        [NotNull]
        public static Rectangle ToNative(this Rect source)
        {
            return new((float)source.X, (float)source.Y,
                (float)source.Width, (float)source.Height);
        }

        public static Rect ToPlatform([NotNull] this Rectangle source)
        {
            return new() {X = source.X, Y = source.Y, Width = source.Width, Height = source.Height};
        }
    }
}
