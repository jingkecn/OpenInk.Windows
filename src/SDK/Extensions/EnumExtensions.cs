using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Windows.Devices.Input;
using Microsoft.Graphics.Canvas.Geometry;
using MyScript.IInk;
using MyScript.IInk.Graphics;
using MyScript.InteractiveInk.Annotations;
using MyScript.InteractiveInk.Constants;
using MyScript.InteractiveInk.Enumerations;

namespace MyScript.InteractiveInk.Extensions
{
    public static partial class EnumExtensions
    {
        public static CanvasFilledRegionDetermination ToPlatform(this FillRule source)
        {
            return source switch
            {
                FillRule.NONZERO => CanvasFilledRegionDetermination.Winding,
                FillRule.EVENODD => CanvasFilledRegionDetermination.Alternate,
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };
        }
    }

    public static partial class EnumExtensions
    {
        public static CanvasCapStyle ToPlatform(this LineCap source)
        {
            return source switch
            {
                LineCap.BUTT => CanvasCapStyle.Flat,
                LineCap.ROUND => CanvasCapStyle.Round,
                LineCap.SQUARE => CanvasCapStyle.Square,
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };
        }

        public static CanvasLineJoin ToPlatform(this LineJoin source)
        {
            return source switch
            {
                LineJoin.MITER => CanvasLineJoin.Miter,
                LineJoin.ROUND => CanvasLineJoin.Round,
                LineJoin.BEVEL => CanvasLineJoin.Bevel,
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };
        }
    }

    public static partial class EnumExtensions
    {
        public static string ToNative(this ContentType source)
        {
            var type = typeof(ContentType);
            var name = Enum.GetName(type, source);
            return type.GetField(name).GetCustomAttributes<DescriptionAttribute>().First().Description;
        }

        public static ContentType ToPlatformContentType([NotNull] this string source)
        {
            var type = typeof(ContentType);
            var values = Enum.GetValues(type).OfType<ContentType>().ToArray();
            return values.Any(item => item.ToNative() == source)
                ? values.Single(item => item.ToNative() == source)
                : ContentType.TextDocument;
        }
    }

    public static partial class EnumExtensions
    {
        public static string ToFileType(this MimeType source, [CanBeNull] string fallback = null)
        {
            return source switch
            {
                MimeType.TEXT => FileTypes.Text,
                MimeType.HTML => FileTypes.Html,
                MimeType.MATHML => fallback,
                MimeType.LATEX => FileTypes.LaTex,
                MimeType.GRAPHML => fallback,
                MimeType.MUSICXML => fallback,
                MimeType.SVG => FileTypes.Svg,
                MimeType.JIIX => FileTypes.Jiix,
                MimeType.JPEG => FileTypes.Jpg,
                MimeType.PNG => FileTypes.Png,
                MimeType.GIF => FileTypes.Gif,
                MimeType.PDF => FileTypes.Pdf,
                MimeType.DOCX => FileTypes.Docx,
                MimeType.PPTX => FileTypes.Pptx,
                MimeType.OFFICE_CLIPBOARD => fallback,
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };
        }

        public static MimeType ToMimeType([NotNull] this string source)
        {
            return source switch
            {
                FileTypes.Docx => MimeType.DOCX,
                FileTypes.Gif => MimeType.GIF,
                FileTypes.Html => MimeType.HTML,
                FileTypes.Jiix => MimeType.JIIX,
                FileTypes.Jpg => MimeType.JPEG,
                FileTypes.LaTex => MimeType.LATEX,
                FileTypes.Pdf => MimeType.PDF,
                FileTypes.Png => MimeType.PNG,
                FileTypes.Pptx => MimeType.PPTX,
                FileTypes.Svg => MimeType.SVG,
                FileTypes.Text => MimeType.TEXT,
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };
        }
    }

    public static partial class EnumExtensions
    {
        public static PointerType ToNative(this PointerDeviceType source, bool isEraser = false)
        {
            return isEraser
                ? PointerType.ERASER
                : source switch
                {
                    PointerDeviceType.Touch => PointerType.TOUCH,
                    PointerDeviceType.Pen => PointerType.PEN,
                    PointerDeviceType.Mouse => PointerType.TOUCH,
                    _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
                };
        }
    }
}
