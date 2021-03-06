using MyScript.InteractiveInk.Enumerations;

namespace MyScript.OpenInk.Core.Models
{
    public interface IPage : IInteractiveInkModel
    {
        int Index { get; set; }
        bool IsDocument { get; }
        bool IsViewScaleEnabled { get; }
        ContentType Type { get; set; }
    }
}
