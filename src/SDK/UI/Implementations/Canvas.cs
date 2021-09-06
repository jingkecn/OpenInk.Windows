using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using MyScript.IInk.Graphics;
using MyScript.InteractiveInk.Annotations;
using MyScript.InteractiveInk.Extensions;
using Color = Windows.UI.Color;

namespace MyScript.InteractiveInk.UI.Implementations
{
    public sealed partial class Canvas
    {
        [CanBeNull] public CanvasDrawingSession DrawingSession { get; set; }
    }

    /// <summary>
    ///     Implements <see cref="ICanvas" />.
    ///     <inheritdoc cref="ICanvas" />
    /// </summary>
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public sealed partial class Canvas : ICanvas
    {
        private Dictionary<string, CanvasActiveLayer> ActiveLayers { get; } =
            new();

        public void StartGroup(string id, float x, float y, float width, float height, bool clipContent)
        {
            if (!clipContent)
            {
                return;
            }

            ActiveLayers[id] = DrawingSession?.CreateLayer(1, new Rect(x, y, width, height));
        }

        public void EndGroup(string id)
        {
            if (!ActiveLayers.ContainsKey(id))
            {
                return;
            }

            ActiveLayers.Remove(id, out var layer);
            layer?.Dispose();
        }

        public void StartItem(string id)
        {
        }

        public void EndItem(string id)
        {
        }

        #region Rendering

        public IPath CreatePath()
        {
            if (DrawingSession == null)
            {
                return null;
            }

            var builder = new CanvasPathBuilder(DrawingSession.Device);
            builder.SetFilledRegionDetermination(FillRule);
            builder.SetSegmentOptions(CanvasFigureSegmentOptions.None);
            return new Path {PathBuilder = builder};
        }

        public void DrawPath(IPath path)
        {
            if (path is not Path target || target.Geometry is not { } geometry)
            {
                return;
            }

            DrawingSession?.DrawGeometry(geometry, StrokeColor, StrokeThickness, StrokeStyle);
            DrawingSession?.FillGeometry(geometry, FillColor);
        }

        public void DrawRectangle(float x, float y, float width, float height)
        {
            DrawingSession?.DrawRectangle(x, y, width, height, StrokeColor, StrokeThickness, StrokeStyle);
            DrawingSession?.FillRectangle(x, y, width, height, FillColor);
        }

        public void DrawLine(float x1, float y1, float x2, float y2)
        {
            DrawingSession?.DrawLine(x1, y1, x2, y2, StrokeColor, StrokeThickness, StrokeStyle);
        }

        public void DrawObject(string url, string mimeType, float x, float y, float width, float height)
        {
            if (!File.Exists(url))
            {
                return;
            }

            var device = DrawingSession?.Device ?? CanvasDevice.GetSharedDevice();
            using var image = CanvasBitmap.LoadAsync(device, url).GetAwaiter().GetResult();
            DrawingSession?.DrawImage(image, new Rect(x, y, width, height), image.Bounds);
        }

        public void DrawText(string label, float x, float y, float minX, float minY, float maxX, float maxY)
        {
            // TODO: don't rely on the max / min values, issue when moving typeset text.
            DrawingSession?.DrawText(label, x, y - TextBaseLine, FillColor, TextFormat);
        }

        public Transform Transform
        {
            get => _transform ??= Matrix3x2.Identity.ToNative();
            set
            {
                _transform = value;
                if (DrawingSession == null)
                {
                    return;
                }

                DrawingSession.Transform = _transform.ToPlatform();
            }
        }

        private Transform _transform;

        #endregion

        #region Styles

        private Color FillColor { get; set; } = Colors.Black;
        private CanvasFilledRegionDetermination FillRule { get; set; } = CanvasFilledRegionDetermination.Winding;
        private Color StrokeColor { get; set; } = Colors.Transparent;
        private CanvasStrokeStyle StrokeStyle { get; } = new();
        private float StrokeThickness { get; set; } = 1;
        private float TextBaseLine { get; set; } = 1;
        private CanvasTextFormat TextFormat { get; } = new();

        public void SetStrokeColor(IInk.Graphics.Color color)
        {
            StrokeColor = color.ToPlatform();
        }

        public void SetStrokeWidth(float width)
        {
            StrokeThickness = width;
        }

        public void SetStrokeLineCap(LineCap lineCap)
        {
            StrokeStyle.DashCap = StrokeStyle.EndCap = StrokeStyle.StartCap = lineCap.ToPlatform();
        }

        public void SetStrokeLineJoin(LineJoin lineJoin)
        {
            StrokeStyle.LineJoin = lineJoin.ToPlatform();
        }

        public void SetStrokeMiterLimit(float limit)
        {
            StrokeStyle.MiterLimit = limit;
        }

        public void SetStrokeDashArray(float[] array)
        {
            StrokeStyle.CustomDashStyle = array;
        }

        public void SetStrokeDashOffset(float offset)
        {
            StrokeStyle.DashOffset = offset;
        }

        public void SetFillColor(IInk.Graphics.Color color)
        {
            FillColor = color.ToPlatform();
        }

        public void SetFillRule(FillRule rule)
        {
            FillRule = rule.ToPlatform();
        }

        public void SetFontProperties(string family, float lineHeight, float size, string style, string variant,
            int weight)
        {
            if (DrawingSession == null)
            {
                return;
            }

            var fontStyle = Enum.TryParse<FontStyle>(style, true, out var result) ? result : FontStyle.Normal;
            TextFormat.FontFamily = family.ToPlatformFontFamily(fontStyle);
            TextFormat.FontSize = size;
            TextFormat.FontStyle = fontStyle;
            TextFormat.FontWeight = new FontWeight {Weight = (ushort)weight};
            TextFormat.Options = CanvasDrawTextOptions.EnableColorFont;
            using var layout =
                new CanvasTextLayout(DrawingSession.Device, "k", TextFormat, float.MaxValue, float.MaxValue);
            TextBaseLine = layout.LineMetrics.First().Baseline;
        }

        #endregion
    }

    /// <summary>
    ///     Implements <see cref="ICanvas2" />.
    ///     <inheritdoc cref="ICanvas2" />
    /// </summary>
    public sealed partial class Canvas : ICanvas2
    {
        private CanvasActiveLayer DrawingLayer { get; set; }

        public void StartDraw(int x, int y, int width, int height)
        {
            var style = new Style();
            style.SetChangeFlags((uint)StyleFlag.StyleFlag_ALL);
            style.ApplyTo(this);
            if (DrawingSession != null)
            {
                DrawingSession.Antialiasing = CanvasAntialiasing.Antialiased;
                DrawingSession.TextAntialiasing = CanvasTextAntialiasing.Auto;
            }

            DrawingLayer = DrawingSession?.CreateLayer(1, new Rect(x, y, width, height));
            DrawingSession?.Clear(Colors.Transparent);
        }

        public void EndDraw()
        {
            DrawingSession?.Flush();
            DrawingLayer?.Dispose();
        }

        public void BlendOffscreen(uint id,
            float srcX, float srcY, float srcWidth, float srcHeight,
            float destX, float destY, float destWidth, float destHeight,
            IInk.Graphics.Color color)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Implements <see cref="IDisposable" />.
    ///     <inheritdoc cref="IDisposable" />
    /// </summary>
    public sealed partial class Canvas : IDisposable
    {
        public void Dispose()
        {
            DrawingSession?.Flush();
            ActiveLayers.Values.Dispose();
            ActiveLayers.Clear();
            DrawingLayer?.Dispose();
        }
    }
}
