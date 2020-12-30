using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Media;

namespace MyScript.InteractiveInk.UI.Xaml.Controls.InkToolbar
{
    public sealed partial class CalligraphyPen
    {
        public CalligraphyPen()
        {
            InitializeComponent();
        }

        protected override InkDrawingAttributes CreateInkDrawingAttributesCore(Brush brush, double strokeWidth)
        {
            var attributes = new InkDrawingAttributes
            {
                Color = (brush as SolidColorBrush)?.Color ?? default,
                IgnorePressure = false,
                PenTip = PenTipShape.Circle,
                PenTipTransform = Matrix3x2.CreateRotation((float)(Math.PI * 45 / 180)),
                Size = new Size(strokeWidth, 2.0f * strokeWidth)
            };
            return attributes;
        }
    }
}
