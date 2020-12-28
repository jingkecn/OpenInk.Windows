using Windows.Foundation;

namespace MyScript.InteractiveInk.Extensions
{
    public static class StructExtensions
    {
        public static Point ToPlatform(this IInk.Graphics.Point source)
        {
            return new(source.X, source.Y);
        }

        public static IInk.Graphics.Point ToNative(this Point source)
        {
            return new((float)source.X, (float)source.Y);
        }
    }
}
