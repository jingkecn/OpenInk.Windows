using System;
using System.Numerics;
using Windows.Graphics.Display;
using MyScript.InteractiveInk.Annotations;

namespace MyScript.OpenInk.UI.Infrastructure.Extensions
{
    public static class DisplayExtensions
    {
        public static Vector2 GetDpi([NotNull] this DisplayInformation source)
        {
            var dpi = new Vector2(source.RawDpiX, source.RawDpiY);
            var density = source.GetPixelDensity();
            if (density > 0)
            {
                dpi.X /= (float)density;
                dpi.Y /= (float)density;
            }

            if (dpi.X == 0 || dpi.Y == 0)
            {
                dpi.X = dpi.Y = 96;
            }

            return dpi;
        }

        public static double GetPixelDensity([NotNull] this DisplayInformation source)
        {
            return source.RawPixelsPerViewPixel > 0
                ? source.RawPixelsPerViewPixel
                : source.ResolutionScale != ResolutionScale.Invalid
                    ? Convert.ToDouble(source.ResolutionScale) / 100.0f
                    : default;
        }
    }
}
