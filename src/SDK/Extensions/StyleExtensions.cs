using System;
using Windows.UI.Text;
using Microsoft.Graphics.Canvas.Text;
using MyScript.IInk.Graphics;
using MyScript.InteractiveInk.Annotations;
using Color = Windows.UI.Color;

namespace MyScript.InteractiveInk.Extensions
{
    public static partial class StyleExtensions
    {
        public static Color ToPlatform(this IInk.Graphics.Color source)
        {
            return Color.FromArgb(
                Convert.ToByte(source.A),
                Convert.ToByte(source.R),
                Convert.ToByte(source.G),
                Convert.ToByte(source.B));
        }

        public static string ToHex(this IInk.Graphics.Color source)
        {
            return $"#{source.R:X2}{source.G:X2}{source.B:X2}{source.A:X2}";
        }

        public static IInk.Graphics.Color ToNative(this Color source)
        {
            return new IInk.Graphics.Color(source.R, source.G, source.B, source.A);
        }
    }

    public static partial class StyleExtensions
    {
        [NotNull]
        public static CanvasTextFormat GetTextFormat([NotNull] this CanvasTextLayout source, int characterIndex)
        {
            return new CanvasTextFormat
            {
                FontFamily = source.GetFontFamily(characterIndex),
                FontSize = source.GetFontSize(characterIndex),
                FontStretch = source.GetFontStretch(characterIndex),
                FontStyle = source.GetFontStyle(characterIndex),
                FontWeight = source.GetFontWeight(characterIndex)
            };
        }

        public static void SetTextFormat([NotNull] this CanvasTextLayout source, int characterIndex, int characterCount,
            [NotNull] CanvasTextFormat format)
        {
            source.SetFontFamily(characterIndex, characterCount, format.FontFamily);
            source.SetFontSize(characterIndex, characterCount, format.FontSize);
            source.SetFontStretch(characterIndex, characterCount, format.FontStretch);
            source.SetFontStyle(characterIndex, characterCount, format.FontStyle);
            source.SetFontWeight(characterIndex, characterCount, format.FontWeight);
        }

        [NotNull]
        public static CanvasTextFormat ToCanvasTextFormat([NotNull] this Style source, float dpi)
        {
            var style = Enum.TryParse<FontStyle>(source.FontStyle, true, out var result) ? result : FontStyle.Normal;
            return new CanvasTextFormat
            {
                FontFamily = source.FontFamily.ToPlatformFontFamily(style),
                FontSize = source.FontSize.FromMillimeterToPixel(dpi),
                FontStyle = style,
                FontWeight = new FontWeight { Weight = (ushort)source.FontWeight },
                Options = CanvasDrawTextOptions.EnableColorFont,
                WordWrapping = CanvasWordWrapping.NoWrap
            };
        }

        internal static string ToPlatformFontFamily(this string family, FontStyle style = FontStyle.Normal)
        {
            return family switch
            {
                "sans-serif" => "Segoe UI",
                var value when value == "STIXGeneral" && style == FontStyle.Italic => "STIX",
                _ => family
            };
        }
    }
}
