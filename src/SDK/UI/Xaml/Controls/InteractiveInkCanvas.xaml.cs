using System;
using System.Diagnostics;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Microsoft.Graphics.Canvas.UI.Xaml;
using MyScript.IInk;
using MyScript.InteractiveInk.Annotations;
using MyScript.InteractiveInk.Extensions;
using Canvas = MyScript.InteractiveInk.UI.Implementations.Canvas;

namespace MyScript.InteractiveInk.UI.Xaml.Controls
{
    public sealed partial class InteractiveInkCanvas
    {
        public static readonly DependencyProperty EditorProperty =
            DependencyProperty.Register("Editor", typeof(Editor), typeof(InteractiveInkCanvas),
                new PropertyMetadata(default(Editor)));

        public static readonly DependencyProperty InkToolbarToolProperty =
            DependencyProperty.Register("InkToolbarTool", typeof(InkToolbarTool), typeof(InteractiveInkCanvas),
                new PropertyMetadata(default(InkToolbarTool)));

        public static readonly DependencyProperty IsFingerInkingEnabledProperty =
            DependencyProperty.Register("IsFingerInkingEnabled", typeof(bool), typeof(InteractiveInkCanvas),
                new PropertyMetadata(default(bool)));

        public InteractiveInkCanvas()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     <inheritdoc cref="Editor" />
        /// </summary>
        [CanBeNull]
        public Editor Editor
        {
            get => GetValue(EditorProperty) as Editor;
            set => Initialize(value);
        }

        public bool IsFingerInkingEnabled
        {
            get => GetValue(IsFingerInkingEnabledProperty) is bool value && value;
            set => SetValue(IsFingerInkingEnabledProperty, value);
        }

        /// <summary>
        ///     <inheritdoc cref="InkToolbarTool" />
        /// </summary>
        public InkToolbarTool InkToolbarTool
        {
            get => GetValue(InkToolbarToolProperty) is InkToolbarTool
                ? (InkToolbarTool)GetValue(InkToolbarToolProperty)
                : default;
            set => SetValue(InkToolbarToolProperty, value);
        }

        /// <summary>
        ///     <inheritdoc cref="Renderer" />
        /// </summary>
        [CanBeNull]
        public Renderer Renderer => Editor?.Renderer;

        private void Initialize([CanBeNull] Editor editor)
        {
            Editor?.RemoveListener(this);
            SetValue(EditorProperty, editor);
            Initialize(Editor.GetGestureSettings());
            Editor.AddListener(this);
        }
    }

    public sealed partial class InteractiveInkCanvas : IEditorListener
    {
        public void PartChanged(Editor editor)
        {
            Dispatcher?.RunAsync(CoreDispatcherPriority.Normal, () => Initialize(editor.GetGestureSettings()))
                ?.AsTask();
        }

        public void ContentChanged(Editor editor, string[] blockIds)
        {
        }

        public void OnError(Editor editor, string blockId, string message)
        {
        }
    }

    /// <summary>
    ///     Implements <see cref="IRenderTarget" />.
    ///     <inheritdoc cref="IRenderTarget" />
    /// </summary>
    public sealed partial class InteractiveInkCanvas : IRenderTarget
    {
        public void Invalidate(Renderer renderer, int x, int y, int width, int height,
            LayerType layers = LayerType.LayerType_ALL)
        {
            if (width <= 0 || height <= 0)
            {
                return;
            }

            Invalidate(new Rect(x, y, width, height), layers);
        }

        public void Invalidate(Renderer renderer, LayerType layers = LayerType.LayerType_ALL)
        {
            Invalidate(renderer, 0, 0, (int)ActualWidth, (int)ActualHeight, layers);
        }

        private void Invalidate(Rect region, LayerType layers = LayerType.LayerType_ALL)
        {
            if (layers.HasFlag(LayerType.BACKGROUND))
            {
                Invalidate(BackgroundLayer, region);
            }

            if (layers.HasFlag(LayerType.CAPTURE))
            {
                Invalidate(CaptureLayer, region);
            }

            if (layers.HasFlag(LayerType.MODEL))
            {
                Invalidate(ModelLayer, region);
            }

            if (layers.HasFlag(LayerType.TEMPORARY))
            {
                Invalidate(TemporaryLayer, region);
            }
        }

        private static void Invalidate([NotNull] CanvasVirtualControl target, Rect region)
        {
            target.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var clamped = region.Clamp(target);
                if (!clamped.IsValid())
                {
                    return;
                }

                target.Invalidate(clamped);
            }).AsTask();
        }
    }

    /// <summary>
    ///     Handles gestures.
    /// </summary>
    public sealed partial class InteractiveInkCanvas
    {
        private void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (!(sender is UIElement element) || e.PointerDeviceType == PointerDeviceType.Pen)
            {
                return;
            }

            var position = e.GetPosition(element);
            Editor?.Typeset(position);
        }
    }

    /// <summary>
    ///     Handles manipulations.
    /// </summary>
    public sealed partial class InteractiveInkCanvas
    {
        private GestureRecognizer _gestureRecognizer;

        private GestureRecognizer GestureRecognizer => _gestureRecognizer ??=
            new GestureRecognizer {GestureSettings = EditorExtensions.DefaultSettings};

        private void Initialize(GestureSettings settings)
        {
            GestureRecognizer.CompleteGesture();
            GestureRecognizer.GestureSettings = settings;
        }

        private void OnManipulationUpdated(GestureRecognizer sender, ManipulationUpdatedEventArgs args)
        {
            if (args.PointerDeviceType == PointerDeviceType.Pen || !Editor.IsScrollAllowed() ||
                (IsFingerInkingEnabled && !HasMultiTouches))
            {
                return;
            }

            Debug.WriteLine($"{nameof(InteractiveInkCanvas)}.{nameof(OnManipulationUpdated)}");
            Renderer?.ChangeViewAt(args.Position.ToNative(), args.Delta.Translation.ToNative(), args.Delta.Scale,
                offset => Editor?.ClampViewOffset(offset));
        }
    }

    /// <summary>
    ///     Handles pointer events.
    /// </summary>
    public sealed partial class InteractiveInkCanvas
    {
        private bool HasMultiTouches { get; set; }
        private PointerPoint PrimaryPointerPoint { get; set; }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!(sender is UIElement element))
            {
                return;
            }

            element.CapturePointer(e.Pointer);
            var point = e.GetCurrentPoint(element);
            HasMultiTouches = !point.Properties.IsPrimary;
            PrimaryPointerPoint = point.Properties.IsPrimary ? point : PrimaryPointerPoint;
            GestureRecognizer.ProcessDownEvent(point);
            if (IsFingerInkingEnabled && HasMultiTouches)
            {
                Editor?.PointerCancel(PrimaryPointerPoint);
            }
            else
            {
                Editor?.PointerDown(point, IsFingerInkingEnabled, InkToolbarTool == InkToolbarTool.Eraser);
            }

            e.Handled = true;
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!(sender is UIElement element))
            {
                return;
            }

            var point = e.GetCurrentPoint(element);
            PrimaryPointerPoint = point.Properties.IsPrimary ? point : PrimaryPointerPoint;
            var points = e.GetIntermediatePoints(element);
            GestureRecognizer.ProcessMoveEvents(points);
            if (!IsFingerInkingEnabled || !HasMultiTouches)
            {
                Editor?.PointerMove(point, IsFingerInkingEnabled, InkToolbarTool == InkToolbarTool.Eraser);
            }

            e.Handled = true;
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!(sender is UIElement element))
            {
                return;
            }

            var point = e.GetCurrentPoint(element);
            PrimaryPointerPoint = point.Properties.IsPrimary ? point : PrimaryPointerPoint;
            GestureRecognizer.ProcessUpEvent(point);
            if (!IsFingerInkingEnabled || !HasMultiTouches)
            {
                Editor?.PointerUp(point, IsFingerInkingEnabled, InkToolbarTool == InkToolbarTool.Eraser);
            }

            element.ReleasePointerCapture(e.Pointer);
            e.Handled = true;
        }

        private void OnPointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            if (!(sender is UIElement element))
            {
                return;
            }

            GestureRecognizer.CompleteGesture();
            var point = e.GetCurrentPoint(element);
            PrimaryPointerPoint = point.Properties.IsPrimary ? point : PrimaryPointerPoint;
            Editor?.PointerCancel(point);
            element.ReleasePointerCapture(e.Pointer);
            e.Handled = true;
        }
    }

    /// <summary>
    ///     Handles regional invalidation events.
    /// </summary>
    public sealed partial class InteractiveInkCanvas
    {
        private void OnRegionsInvalidated(CanvasVirtualControl sender, CanvasRegionsInvalidatedEventArgs args)
        {
            var layer = sender.Name switch
            {
                nameof(BackgroundLayer) => LayerType.BACKGROUND,
                nameof(CaptureLayer) => LayerType.CAPTURE,
                nameof(ModelLayer) => LayerType.MODEL,
                nameof(TemporaryLayer) => LayerType.TEMPORARY,
                _ => LayerType.LayerType_ALL
            };

            foreach (var region in args.InvalidatedRegions)
            {
                var clamped = region.Clamp(sender);
                if (!clamped.IsValid())
                {
                    continue;
                }

                using var session = sender.CreateDrawingSession(clamped);
                using var canvas = new Canvas {DrawingSession = session};
                Renderer?.Draw(clamped, layer, canvas);
            }
        }
    }

    /// <summary>
    ///     Handles lifecycle events.
    /// </summary>
    public sealed partial class InteractiveInkCanvas
    {
        private void OnLoaded(object sender, RoutedEventArgs _)
        {
            Editor?.SetViewSize((int)ActualWidth, (int)ActualHeight);
            GestureRecognizer.ManipulationUpdated += OnManipulationUpdated;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Editor?.SetViewSize((int)e.NewSize.Width, (int)e.NewSize.Height);
            Invalidate(Renderer);
        }

        private void OnUnloaded(object sender, RoutedEventArgs _)
        {
            GestureRecognizer.CompleteGesture();
            GestureRecognizer.ManipulationUpdated -= OnManipulationUpdated;
            BackgroundLayer.RemoveFromVisualTree();
            CaptureLayer.RemoveFromVisualTree();
            ModelLayer.RemoveFromVisualTree();
            TemporaryLayer.RemoveFromVisualTree();
        }
    }
}
