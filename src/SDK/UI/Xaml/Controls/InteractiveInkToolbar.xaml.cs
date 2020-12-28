using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyScript.IInk;
using MyScript.InteractiveInk.Annotations;
using MyScript.InteractiveInk.Extensions;

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
        private void OnActiveToolChanged(InkToolbar sender, object args)
        {
            if (TargetInkCanvas == null)
            {
                return;
            }

            TargetInkCanvas.InkToolbarTool = sender.ActiveTool.ToolKind;
        }

        private void OnEraseAllClicked(InkToolbar sender, object args)
        {
            Editor?.Clear();
        }

        private void OnInkDrawingAttributesChanged(InkToolbar sender, object args)
        {
            if (Editor == null)
            {
                return;
            }

            var attributes = sender.InkDrawingAttributes;
            var color = attributes.Color.ToNative().ToHex();
            var size = attributes.Size.Width / 10;
            Editor.PenStyle = $"color: {color}; -myscript-pen-width: {size}";
        }
    }
}
