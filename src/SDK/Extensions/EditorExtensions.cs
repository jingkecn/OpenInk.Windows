using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Input;
using MyScript.IInk;
using MyScript.InteractiveInk.Annotations;
using MyScript.InteractiveInk.Enumerations;

namespace MyScript.InteractiveInk.Extensions
{
    public static partial class EditorExtensions
    {
        public const GestureSettings DefaultSettings =
            GestureSettings.ManipulationMultipleFingerPanning |
            GestureSettings.ManipulationScale |
            GestureSettings.ManipulationScaleInertia |
            GestureSettings.ManipulationTranslateInertia |
            GestureSettings.ManipulationTranslateX |
            GestureSettings.ManipulationTranslateY;

        public static GestureSettings GetGestureSettings([NotNull] this Editor source)
        {
            return source.Part is { } part ? part.GetGestureSettings() : DefaultSettings;
        }
    }

    public static partial class EditorExtensions
    {
        public static void AddBlockAt([NotNull] this Editor source, Point position, ContentType type)
        {
            source.AddBlock((float)position.X, (float)position.Y, type.ToNative());
        }

        /// <summary>
        ///     Append a <see cref="ContentBlock" /> to the end of a document.
        /// </summary>
        /// <param name="source">The source <see cref="Editor" />.</param>
        /// <param name="type">
        ///     <inheritdoc cref="ContentType" />
        /// </param>
        /// <param name="autoScroll">
        ///     Set <code>true</code> to auto scroll to the new <see cref="ContentBlock" />, otherwise
        ///     <code>false</code>. The default value is <code>true</code>.
        /// </param>
        public static void AppendBlock([NotNull] this Editor source, ContentType type, bool autoScroll = true)
        {
            if (!source.CanAddBlock(type) || !(source.Renderer is { } renderer))
            {
                return;
            }

            var bounds = source.GetDocumentBounds();
            var lineHeight = source.GetLineHeight();
            var (x, y) = (bounds.Left, bounds.Bottom);
            source.AddBlock((float)x, (float)y + lineHeight, type.ToNative());
            if (!autoScroll)
            {
                return;
            }

            renderer.ScrollTo(new IInk.Graphics.Point((float)x, (float)y), source.ClampViewOffset);
        }

        public static async Task AddImageAsync([NotNull] this Editor source, Point position)
        {
            // Picks a file from the file picker.
            var picker = new FileOpenPicker {SuggestedStartLocation = PickerLocationId.PicturesLibrary};
            picker.FileTypeFilter.Add(MimeType.GIF.ToFileType());
            picker.FileTypeFilter.Add(MimeType.JPEG.ToFileType());
            picker.FileTypeFilter.Add(MimeType.PNG.ToFileType());
            picker.FileTypeFilter.Add(MimeType.SVG.ToFileType());
            if (!(await picker.PickSingleFileAsync() is { } picked))
            {
                return;
            }

            var folder = ApplicationData.Current.LocalCacheFolder;
            var file = await picked.CopyAsync(folder, picked.Name, NameCollisionOption.GenerateUniqueName);
            var (x, y) = ((float)position.X, (float)position.Y);
            source.AddImage(x, y, file.Path, file.FileType.ToMimeType());
        }

        public static async Task AppendImageAsync([NotNull] this Editor source, bool autoScroll = true)
        {
            if (!(source.Renderer is { } renderer))
            {
                return;
            }

            var bounds = source.GetDocumentBounds();
            var lineHeight = source.GetLineHeight();
            var (x, y) = (bounds.Left, bounds.Bottom);
            await source.AddImageAsync(new Point(x, y + lineHeight));
            if (!autoScroll)
            {
                return;
            }

            renderer.ScrollTo(new IInk.Graphics.Point((float)x, (float)y), source.ClampViewOffset);
        }

        public static bool CanAddBlock([NotNull] this Editor source, ContentType type, bool defaultValue = default)
        {
            return source.SupportedAddBlockTypes?.Contains(type.ToNative()) ?? defaultValue;
        }

        public static bool CanAddBlockAt([NotNull] this Editor source, Point position, bool defaultValue = default)
        {
            return (source.Part is { } part && part.Type.ToPlatformContentType() == ContentType.TextDocument &&
                    (!(source.GetBlockAt(position) is { } block) || block.IsContainer())) || defaultValue;
        }

        public static bool CanRemoveBlock([NotNull] this Editor source, [NotNull] ContentBlock block,
            bool defaultValue = default)
        {
            return !block.IsContainer() || defaultValue;
        }

        public static bool CanRemoveBlockAt([NotNull] this Editor source, Point position, bool defaultValue = default)
        {
            return (source.GetBlockAt(position) is { } block && source.CanRemoveBlock(block)) || defaultValue;
        }

        [CanBeNull]
        public static ContentBlock GetBlockAt([NotNull] this Editor source, Point position)
        {
            return source.HitBlock((float)position.X, (float)position.Y);
        }

        public static void RemoveBlockAt([NotNull] this Editor source, Point position)
        {
            if (!source.CanRemoveBlockAt(position))
            {
                return;
            }

            var block = source.HitBlock((float)position.X, (float)position.Y);
            source.RemoveBlock(block);
            block.Dispose();
        }
    }

    public static partial class EditorExtensions
    {
        public static bool CanCopy([NotNull] this Editor source, [NotNull] ContentBlock block)
        {
            return source.Part is { } part && part.Type.ToPlatformContentType() == ContentType.TextDocument &&
                   !block.IsContainer();
        }

        public static bool CanCopyAt([NotNull] this Editor source, Point position)
        {
            return source.GetBlockAt(position) is { } block && source.CanCopy(block);
        }

        public static bool CanPasteAt([NotNull] this Editor source, Point position)
        {
            return source.Part is { } part && part.Type.ToPlatformContentType() == ContentType.TextDocument &&
                   (!(source.GetBlockAt(position) is { } block) || block.IsContainer());
        }

        public static void CopyAt([NotNull] this Editor source, Point position)
        {
            if (!source.CanCopyAt(position))
            {
                return;
            }

            var block = source.GetBlockAt(position);
            source.Copy(block);
        }

        public static void PasteAt([NotNull] this Editor source, Point position)
        {
            if (!source.CanPasteAt(position))
            {
                return;
            }

            source.Paste((float)position.X, (float)position.Y);
        }

        public static void Paste([NotNull] this Editor source)
        {
            var bounds = source.GetDocumentBounds();
            var lineHeight = source.GetLineHeight();
            var (x, y) = (bounds.Left, bounds.Bottom);
            var position = new Point(x, y + lineHeight);
            if (!source.CanPasteAt(position))
            {
                return;
            }

            source.PasteAt(position);
        }
    }

    public static partial class EditorExtensions
    {
        public static bool CanGoBack([NotNull] this Editor source)
        {
            return source.Part?.HasPrevious() ?? false;
        }

        public static bool CanGoForward([NotNull] this Editor source)
        {
            return source.Part?.HasNext() ?? false;
        }

        public static void GoBack([NotNull] this Editor source)
        {
            source.Part = source.Part?.GetPrevious() ?? source.Part;
        }

        public static void GoForward([NotNull] this Editor source)
        {
            source.Part = source.Part?.GetNext() ?? source.Part;
        }
    }

    /// <summary>
    ///     Sends pointer events.
    ///     Please be careful with the timestamp: the editor accepts the pointer timestamp in milliseconds, whereas the UWP
    ///     pointer timestamp is in microseconds, therefore, a conversion between these two units is a must.
    ///     Otherwise, you would encounter the following error when handling pointer events on the text document part:
    ///     - ink rejected: stroke is too long.
    ///     This is because, on the text document part, the interactive-ink SDK checks the interval time between the first
    ///     point (pointer down) and the last point (pointer up / cancel) of a single stroke, and raises the error if the
    ///     interval time is too high (> 15s).
    /// </summary>
    public static partial class EditorExtensions
    {
        /// <summary>
        ///     <inheritdoc cref="Editor.PointerCancel" />
        /// </summary>
        /// <param name="source">The source <see cref="Editor" />.</param>
        /// <param name="point">The <see cref="PointerPoint" />.</param>
        public static void PointerCancel([NotNull] this Editor source, PointerPoint point)
        {
            if (!point.Properties.IsPrimary)
            {
                return;
            }

            var id = point.PointerId;
            source.PointerCancel((int)id);
        }

        /// <summary>
        ///     <inheritdoc cref="Editor.PointerDown" />
        /// </summary>
        /// <param name="source">The source <see cref="Editor" />.</param>
        /// <param name="point">The <see cref="PointerPoint" />.</param>
        /// <param name="enableFingerInking">Indicates if the pointer input should handle finger inking.</param>
        /// <param name="isEraser">Indicates if the pointer input is <see cref="PointerType.ERASER" />.</param>
        public static void PointerDown([NotNull] this Editor source, PointerPoint point,
            bool enableFingerInking = false,
            bool isEraser = false)
        {
            if (!point.Properties.IsPrimary)
            {
                return;
            }

            var x = point.Position.X;
            var y = point.Position.Y;
            var timestamp = point.Timestamp.FromMicrosecondsToMilliseconds();
            var pressure = point.Properties.Pressure;
            var type = point.PointerDevice.PointerDeviceType.ToNative(enableFingerInking, isEraser);
            var id = point.PointerId;
            source.PointerDown((float)x, (float)y, (long)timestamp, pressure, type, (int)id);
        }

        /// <summary>
        ///     <inheritdoc cref="Editor.PointerMove" />
        /// </summary>
        /// <param name="source">The source <see cref="Editor" />.</param>
        /// <param name="point">The <see cref="PointerPoint" />.</param>
        /// <param name="enableFingerInking">Indicates if the pointer input should handle finger inking.</param>
        /// <param name="isEraser">Indicates if the pointer input is <see cref="PointerType.ERASER" />.</param>
        public static void PointerMove([NotNull] this Editor source, PointerPoint point,
            bool enableFingerInking = false,
            bool isEraser = false)
        {
            if (!point.IsInContact || !point.Properties.IsPrimary)
            {
                return;
            }

            var x = point.Position.X;
            var y = point.Position.Y;
            var timestamp = point.Timestamp.FromMicrosecondsToMilliseconds();
            var pressure = point.Properties.Pressure;
            var type = point.PointerDevice.PointerDeviceType.ToNative(enableFingerInking, isEraser);
            var id = point.PointerId;
            source.PointerMove((float)x, (float)y, (long)timestamp, pressure, type, (int)id);
        }

        /// <summary>
        ///     <inheritdoc cref="Editor.PointerUp" />
        /// </summary>
        /// <param name="source">The source <see cref="Editor" />.</param>
        /// <param name="point">The <see cref="PointerPoint" />.</param>
        /// <param name="enableFingerInking">Indicates if the pointer input should handle finger inking.</param>
        /// <param name="isEraser">Indicates if the pointer input is <see cref="PointerType.ERASER" />.</param>
        public static void PointerUp([NotNull] this Editor source, PointerPoint point, bool enableFingerInking = false,
            bool isEraser = false)
        {
            if (!point.Properties.IsPrimary)
            {
                return;
            }

            var x = point.Position.X;
            var y = point.Position.Y;
            var timestamp = point.Timestamp.FromMicrosecondsToMilliseconds();
            var pressure = point.Properties.Pressure;
            var type = point.PointerDevice.PointerDeviceType.ToNative(enableFingerInking, isEraser);
            var id = point.PointerId;
            source.PointerUp((float)x, (float)y, (long)timestamp, pressure, type, (int)id);
        }
    }

    /// <summary>
    ///     Handles typeset commands.
    /// </summary>
    public static partial class EditorExtensions
    {
        public static void Typeset([NotNull] this Editor source, [CanBeNull] ContentBlock block = null)
        {
            var states = source.GetSupportedTargetConversionStates(block);
            if (!states.Any())
            {
                return;
            }

            source.Convert(block, states.First());
        }

        public static void Typeset([NotNull] this Editor source, float x, float y)
        {
            source.Typeset(source.HitBlock(x, y));
        }

        public static void Typeset([NotNull] this Editor source, Point position)
        {
            source.Typeset((float)position.X, (float)position.Y);
        }
    }

    public static partial class EditorExtensions
    {
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static Rect GetDocumentBounds([NotNull] this Editor source)
        {
            if (!(source.Renderer is { } renderer))
            {
                return Rect.Empty;
            }

            var block = source.GetRootBlock();
            var (dpiX, dpiY) = (renderer.DpiX, renderer.DpiY);
            return block.Box.ToPlatform().FromMillimeterToPixel(dpiX, dpiY);
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static float GetLineHeight([NotNull] this Editor source)
        {
            var styles = source.ListStyleClasses(_ => true);
            return source.Renderer is { } renderer && styles.TryGetValue("guide", out var style)
                ? style.FontLineHeight.FromMillimeterToPixel(renderer.DpiY)
                : default;
        }
    }
}
