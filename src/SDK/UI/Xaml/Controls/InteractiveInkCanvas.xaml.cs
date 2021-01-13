using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Microsoft.Graphics.Canvas.UI.Xaml;
using MyScript.IInk;
using MyScript.InteractiveInk.Annotations;
using MyScript.InteractiveInk.Enumerations;
using MyScript.InteractiveInk.Extensions;
using Canvas = MyScript.InteractiveInk.UI.Implementations.Canvas;
using Point = MyScript.IInk.Graphics.Point;

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

        public static readonly DependencyProperty IsMouseInkingEnabledProperty =
            DependencyProperty.Register("IsMouseInkingEnabled", typeof(bool), typeof(InteractiveInkCanvas),
                new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty MaxZoomFactorProperty =
            DependencyProperty.Register("MaxZoomFactor", typeof(double), typeof(InteractiveInkCanvas),
                new PropertyMetadata(double.MaxValue));

        public static readonly DependencyProperty MinZoomFactorProperty =
            DependencyProperty.Register("MinZoomFactor", typeof(double), typeof(InteractiveInkCanvas),
                new PropertyMetadata(default(double)));

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

        public bool IsMouseInkingEnabled
        {
            get => GetValue(IsMouseInkingEnabledProperty) is bool value && value;
            set => SetValue(IsMouseInkingEnabledProperty, value);
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

        public double MaxZoomFactor
        {
            get => (double)GetValue(MaxZoomFactorProperty);
            set => SetValue(MaxZoomFactorProperty, value);
        }

        public double MinZoomFactor
        {
            get => (double)GetValue(MinZoomFactorProperty);
            set => SetValue(MinZoomFactorProperty, value);
        }

        private void Initialize([CanBeNull] Editor editor)
        {
            SetValue(EditorProperty, editor);
            if (editor == null)
            {
                return;
            }

            Initialize(editor.GetGestureSettings());
            (IsHorizontalScrollBarVisible, IsVerticalScrollBarVisible) = editor.GetScrollBarVisibility();
        }
    }

    public sealed partial class InteractiveInkCanvas
    {
        private bool _isHorizontalScrollBarVisible;
        private bool _isVerticalScrollBarVisible;
        private Vector2 _viewOffset;
        private Point _viewOffsetMaximum;

        private bool IsHorizontalScrollBarVisible
        {
            get => _isHorizontalScrollBarVisible;
            set => Set(ref _isHorizontalScrollBarVisible, value, nameof(IsHorizontalScrollBarVisible));
        }

        private bool IsVerticalScrollBarVisible
        {
            get => _isVerticalScrollBarVisible;
            set => Set(ref _isVerticalScrollBarVisible, value, nameof(IsVerticalScrollBarVisible));
        }

        private Vector2 ViewOffset
        {
            get => _viewOffset;
            set => Set(ref _viewOffset, value, nameof(ViewOffset));
        }

        private Point ViewOffsetMaximum
        {
            get => _viewOffsetMaximum;
            set => Set(ref _viewOffsetMaximum, value, nameof(ViewOffsetMaximum));
        }
    }

    public sealed partial class InteractiveInkCanvas : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Set<TValue>(ref TValue storage, TValue value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TValue>.Default.Equals(value, storage))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
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

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (!(Editor is {Renderer: { } renderer} editor))
                {
                    return;
                }

                var layout = new Rect(0, 0, editor.ViewWidth, editor.ViewHeight);
                layout.Union(editor.GetDocumentBounds());
                ViewOffsetMaximum = new Point((float)layout.Width, (float)layout.Height);
                var offset = renderer.ViewOffset;
                ViewOffset = new Vector2(offset.X, offset.Y);
            }).AsTask();
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
            if (!(sender is UIElement element) || e.PointerDeviceType == PointerDeviceType.Pen ||
                IsFingerInkingEnabled || IsMouseInkingEnabled)
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
            if (args.PointerDeviceType == PointerDeviceType.Pen ||
                !(Editor is {Renderer: { } renderer} editor) || !editor.IsScrollAllowed() ||
                (IsFingerInkingEnabled && !HasMultiTouches) || (!IsFingerInkingEnabled && IsMouseInkingEnabled))
            {
                return;
            }


            Debug.WriteLine($"{nameof(InteractiveInkCanvas)}.{nameof(OnManipulationUpdated)}");
            renderer.ChangeViewAt(args.Position.ToNative(), args.Delta.Translation.ToNative(), args.Delta.Scale,
                (float)MaxZoomFactor, (float)MinZoomFactor, offset => editor.ClampViewOffset(offset));
        }
    }

    /// <summary>
    ///     Handles pointer events.
    /// </summary>
    public sealed partial class InteractiveInkCanvas
    {
        private bool HasMultiTouches { get; set; }

        private InkingInput InkingInput
        {
            get
            {
                var input = InkingInput.Pen;
                input |= IsFingerInkingEnabled ? InkingInput.Touch : input;
                input |= IsMouseInkingEnabled ? InkingInput.Mouse : input;
                return input;
            }
        }

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
                Editor?.PointerDown(point, InkingInput, InkToolbarTool == InkToolbarTool.Eraser);
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
                Editor?.PointerMove(point, InkingInput, InkToolbarTool == InkToolbarTool.Eraser);
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
                Editor?.PointerUp(point, InkingInput, InkToolbarTool == InkToolbarTool.Eraser);
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

        private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (!(sender is UIElement element))
            {
                return;
            }

            var point = e.GetCurrentPoint(element);
            var delta = point.Properties.MouseWheelDelta;
            var isHorizontal = point.Properties.IsHorizontalMouseWheel;
            var deltaX = isHorizontal ? delta : default;
            var deltaY = isHorizontal ? default : delta;
            Editor?.Renderer?.Scroll(new Point(deltaX, deltaY), offset => Editor?.ClampViewOffset(offset));
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
                Editor?.Renderer?.Draw(clamped, layer, canvas);
            }
        }
    }

    public sealed partial class InteractiveInkCanvas
    {
        private void OnHorizontalScroll(object sender, ScrollEventArgs e)
        {
            Debug.WriteLine($"{nameof(InteractiveInkCanvas)}.{nameof(OnHorizontalScroll)}:");
            Debug.WriteLine($"\ttype: {e.ScrollEventType}");
            Debug.WriteLine($"\tvalue: {e.NewValue}");
            if (!(Editor is {Renderer: { } renderer} editor) || !editor.IsScrollAllowed())
            {
                return;
            }

            var offset = new Point((float)e.NewValue, renderer.ViewOffset.Y);
            renderer.ScrollTo(offset, x => editor.ClampViewOffset(x));
        }

        private void OnVerticalScroll(object sender, ScrollEventArgs e)
        {
            Debug.WriteLine($"{nameof(InteractiveInkCanvas)}.{nameof(OnVerticalScroll)}:");
            Debug.WriteLine($"\ttype: {e.ScrollEventType}");
            Debug.WriteLine($"\tvalue: {e.NewValue}");
            if (!(Editor is {Renderer: { } renderer} editor) || !editor.IsScrollAllowed())
            {
                return;
            }

            var offset = new Point(renderer.ViewOffset.X, (float)e.NewValue);
            renderer.ScrollTo(offset, x => editor.ClampViewOffset(x));
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
            if (!(Editor is { } editor))
            {
                return;
            }

            editor.SetViewSize((int)e.NewSize.Width, (int)e.NewSize.Height);
            if (!(editor.Renderer is { } renderer))
            {
                return;
            }

            renderer.ResetViewScale();
            if (!(editor.Part is { } part))
            {
                return;
            }

            var type = part.Type.ToPlatformContentType();
            if (type == ContentType.Text || type == ContentType.TextDocument)
            {
                return;
            }

            renderer.ResetViewOffset();
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
