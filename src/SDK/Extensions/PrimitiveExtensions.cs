using Windows.Foundation;

namespace MyScript.InteractiveInk.Extensions
{
    public static partial class PrimitiveExtensions
    {
        private const float MillimetersPerInch = 25.4f;

        public static float FromPixelToMillimeter(this double source, float dpi)
        {
            return ((float)source).FromPixelToMillimeter(dpi);
        }

        public static float FromPixelToMillimeter(this float source, float dpi)
        {
            // DPI: dots or pixels per inch
            // => dpi = pixels / inch
            // => inch = pixels / dpi
            var inch = source / dpi;
            return inch * MillimetersPerInch;
        }

        public static float FromMillimeterToPixel(this double source, float dpi)
        {
            return ((float)source).FromMillimeterToPixel(dpi);
        }

        public static float FromMillimeterToPixel(this float source, float dpi)
        {
            // DPI: dots or pixels per inch
            // => dpi = pixels / inch
            // => pixels = dpi * inch
            var inch = source / MillimetersPerInch;
            return dpi * inch;
        }

        public static Rect FromPixelToMillimeter(this Rect source, float dpiX, float dpiY)
        {
            return new Rect
            {
                X = ((float)source.X).FromPixelToMillimeter(dpiX),
                Y = ((float)source.Y).FromPixelToMillimeter(dpiY),
                Width = ((float)source.Width).FromPixelToMillimeter(dpiX),
                Height = ((float)source.Height).FromPixelToMillimeter(dpiY)
            };
        }

        public static Rect FromMillimeterToPixel(this Rect source, float dpiX, float dpiY)
        {
            return new Rect
            {
                X = ((float)source.X).FromMillimeterToPixel(dpiX),
                Y = ((float)source.Y).FromMillimeterToPixel(dpiY),
                Width = ((float)source.Width).FromMillimeterToPixel(dpiX),
                Height = ((float)source.Height).FromMillimeterToPixel(dpiY)
            };
        }
    }

    public static partial class PrimitiveExtensions
    {
        public static float FromEmToPixel(this float source, float fontSize)
        {
            return source * fontSize;
        }

        public static float FromPixelToEm(this float source, float fontSize)
        {
            return source / fontSize;
        }

        public static Rect FromEmToPixel(this Rect source, float fontSize)
        {
            return new Rect
            {
                X = ((float)source.X).FromEmToPixel(fontSize),
                Y = ((float)source.Y).FromEmToPixel(fontSize),
                Width = ((float)source.Width).FromEmToPixel(fontSize),
                Height = ((float)source.Height).FromEmToPixel(fontSize)
            };
        }

        public static Rect FromPixelToEm(this Rect source, float fontSize)
        {
            return new Rect
            {
                X = ((float)source.X).FromPixelToEm(fontSize),
                Y = ((float)source.Y).FromPixelToEm(fontSize),
                Width = ((float)source.Width).FromPixelToEm(fontSize),
                Height = ((float)source.Height).FromPixelToEm(fontSize)
            };
        }
    }

    public static partial class PrimitiveExtensions
    {
        private const ulong MicrosecondsPerMillisecond = 1000;

        public static ulong FromMicrosecondsToMilliseconds(this ulong source)
        {
            return source / MicrosecondsPerMillisecond;
        }

        public static ulong FromMillisecondsToMicroseconds(this ulong source)
        {
            return source * MicrosecondsPerMillisecond;
        }
    }
}
