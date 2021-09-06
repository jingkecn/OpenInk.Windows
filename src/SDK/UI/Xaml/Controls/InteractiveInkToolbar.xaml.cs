using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyScript.IInk;
using MyScript.InteractiveInk.Annotations;
using MyScript.InteractiveInk.Enumerations;
using MyScript.InteractiveInk.Extensions;
using MyScript.InteractiveInk.UI.Styles;

namespace MyScript.InteractiveInk.UI.Xaml.Controls
{
    /// <summary>
    ///     <inheritdoc cref="InkToolbar" />
    /// </summary>
    public sealed partial class InteractiveInkToolbar
    {
        public static readonly DependencyProperty TargetInkCanvasProperty =
            DependencyProperty.Register("TargetInkCanvas", typeof(InteractiveInkCanvas), typeof(InteractiveInkToolbar),
                new PropertyMetadata(default(InteractiveInkCanvas)));

        public InteractiveInkToolbar()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     <inheritdoc cref="Editor" />
        /// </summary>
        [CanBeNull]
        private Editor Editor => TargetInkCanvas?.Editor;

        /// <summary>
        ///     <inheritdoc cref="InteractiveInkCanvas" />
        /// </summary>
        [CanBeNull]
        public InteractiveInkCanvas TargetInkCanvas
        {
            get => GetValue(TargetInkCanvasProperty) as InteractiveInkCanvas;
            set => SetValue(TargetInkCanvasProperty, value);
        }
    }

    public sealed partial class InteractiveInkToolbar
    {
        private PenStyle PenStyle { get; set; } = new() {Brush = PenBrush.FeltPen, Color = default};

        private void OnActiveToolChanged(Windows.UI.Xaml.Controls.InkToolbar sender, object args)
        {
            if (TargetInkCanvas is not { } canvas)
            {
                return;
            }

            var tool = sender.ActiveTool.ToolKind;
            canvas.InkToolbarTool = tool;
            if (Editor is not { } editor)
            {
                return;
            }

            var style = PenStyle;
            style.Brush = tool switch
            {
                InkToolbarTool.BallpointPen => PenBrush.FeltPen,
                InkToolbarTool.Pencil => PenBrush.FeltPen,
                InkToolbarTool.Highlighter => PenBrush.Polyline,
                InkToolbarTool.Eraser => style.Brush,
                InkToolbarTool.CustomPen => style.Brush,
                InkToolbarTool.CustomTool => style.Brush,
                _ => throw new ArgumentOutOfRangeException()
            };
            editor.Apply(PenStyle = style);
        }

        private void OnEraseAllClicked(Windows.UI.Xaml.Controls.InkToolbar sender, object args)
        {
            Editor?.Clear();
        }

        private void OnInkDrawingAttributesChanged(Windows.UI.Xaml.Controls.InkToolbar sender, object args)
        {
            if (Editor is not { } editor)
            {
                return;
            }

            var attributes = sender.InkDrawingAttributes;
            var style = PenStyle;
            var tool = sender.ActiveTool.ToolKind;
            var color = attributes.Color;
            color.A = tool switch
            {
                InkToolbarTool.BallpointPen => color.A,
                InkToolbarTool.Pencil => Convert.ToByte(144),
                InkToolbarTool.Highlighter => Convert.ToByte(128),
                InkToolbarTool.Eraser => color.A,
                InkToolbarTool.CustomPen => color.A,
                InkToolbarTool.CustomTool => color.A,
                _ => throw new ArgumentOutOfRangeException()
            };
            style.Color = color;
            style.Size = attributes.Size;
            editor.Apply(PenStyle = style);
        }

        private void OnCalligraphyPenChecked(object sender, RoutedEventArgs e)
        {
            if (Editor is not { } editor)
            {
                return;
            }

            var style = PenStyle;
            style.Brush = PenBrush.CalligraphicBrush;
            editor.Apply(PenStyle = style);
        }
    }
}
