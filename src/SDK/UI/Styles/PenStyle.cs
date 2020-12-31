using Windows.Foundation;
using Windows.UI;
using MyScript.InteractiveInk.Enumerations;

namespace MyScript.InteractiveInk.UI.Styles
{
    public struct PenStyle
    {
        public PenBrush Brush { get; set; }
        public Color Color { get; set; }
        public Size Size { get; set; }
    }
}
