using System.Diagnostics;
using Windows.Foundation;
using Windows.UI;
using MyScript.IInk;
using MyScript.InteractiveInk.Annotations;
using MyScript.InteractiveInk.Enumerations;
using MyScript.InteractiveInk.Extensions;

namespace MyScript.InteractiveInk.UI.Styles
{
    public partial struct PenStyle
    {
        public PenBrush Brush { get; set; }
        public Color Color { get; set; }
        public Size Size { get; set; }
    }

    public partial struct PenStyle
    {
        public void ApplyTo([NotNull] Editor editor)
        {
            Debug.WriteLine($"{nameof(PenStyle)}.{nameof(ApplyTo)}:");
            var style =
                $"color: {Color.ToNative().ToHex()}; " +
                $"-myscript-pen-brush: {Brush}; " +
                $"-myscript-pen-width: {Size.Width * 1.5f / 4f}";
            Debug.WriteLine($"\tstyle: {style}");
            editor.PenStyle = style;
        }
    }
}
