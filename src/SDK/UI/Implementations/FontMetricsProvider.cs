using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using MyScript.IInk.Graphics;
using MyScript.IInk.Text;
using MyScript.InteractiveInk.Extensions;

namespace MyScript.InteractiveInk.UI.Implementations
{
    public partial class FontMetricsProvider
    {
        public FontMetricsProvider(float dpiX, float dpiY)
        {
            Dpi = new Vector2(dpiX, dpiY);
        }

        private Vector2 Dpi { get; }
    }

    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class FontMetricsProvider : IFontMetricsProvider
    {
        public Rectangle[] GetCharacterBoundingBoxes(Text text, TextSpan[] spans)
        {
            return GetGlyphMetrics(text, spans).Select(metrics => metrics.BoundingBox).ToArray();
        }

        /// <summary>
        ///     Get font size. This method influences the line spacing of text document.
        ///     [!Attention]
        ///     Don't return font size in pixels, otherwise it will be double converted in pixels.
        /// </summary>
        /// <param name="style">
        ///     <inheritdoc cref="Style" />
        /// </param>
        /// <returns>Should return font size in millimeter.</returns>
        public float GetFontSizePx(Style style)
        {
            return style.FontSize;
        }
    }

    public partial class FontMetricsProvider : IFontMetricsProvider2
    {
        private const float VirtualSize = 10000;

        public bool SupportsGlyphMetrics()
        {
            return true;
        }

        /// <summary>
        ///     <inheritdoc cref="IFontMetricsProvider2.GetGlyphMetrics" />
        /// </summary>
        /// <param name="text">
        ///     <inheritdoc cref="Text" />
        /// </param>
        /// <param name="spans">
        ///     <inheritdoc cref="TextSpan" />
        /// </param>
        /// <returns>The metrics of each glyphs.</returns>
        /// <remarks>
        ///     TODO: explain why can't we use the Win2D <see cref="ICanvasTextRenderer" /> to get glyph metrics directly.
        /// </remarks>
        public GlyphMetrics[] GetGlyphMetrics(Text text, TextSpan[] spans)
        {
            // Create a virtual text layout for all glyphs.
            var device = CanvasDevice.GetSharedDevice();
            using var textLayout =
                new CanvasTextLayout(device, text.Label, new CanvasTextFormat(), VirtualSize, VirtualSize);
            // For each span, apply span style to the text layout.
            foreach (var span in spans)
            {
                var count = span.EndPosition - span.BeginPosition;
                var format = span.Style.ToCanvasTextFormat(Dpi.Y);
                textLayout.SetTextFormat(span.BeginPosition, count, format);
            }

            var result = new GlyphMetrics[text.GlyphCount];
            for (var index = 0; index < text.GlyphCount; index++)
            {
                var begin = text.GetGlyphBeginAt(index);
                var format = textLayout.GetTextFormat(index);
                var glyph = text.GetGlyphLabelAt(index);
                using var glyphLayout = new CanvasTextLayout(device, glyph, format, VirtualSize, VirtualSize);
                // TODO: explain why we choose draw bounds over layout bounds.
                // See: https://microsoft.github.io/Win2D/html/P_Microsoft_Graphics_Canvas_Text_CanvasTextLayout_DrawBounds.htm
                var drawBounds = glyphLayout.DrawBounds;
                var layoutBounds = glyphLayout.LayoutBounds;
                // Calculate horizontal bearings.
                var leftSideBearing = drawBounds.Left - layoutBounds.Left;
                var rightSideBearing = drawBounds.Right - layoutBounds.Right;
                // Translations
                drawBounds.X += textLayout.GetCaretPosition(begin, false).X;
                drawBounds.Y -= glyphLayout.LineMetrics.Single().Baseline;
                // Add calculated glyph metrics to the array.
                result[index] = new GlyphMetrics(drawBounds.FromPixelToMillimeter(Dpi.X, Dpi.Y).ToNative(),
                    -leftSideBearing.FromPixelToMillimeter(Dpi.X),
                    -rightSideBearing.FromPixelToMillimeter(Dpi.X));
            }

            return result;
        }
    }
}
