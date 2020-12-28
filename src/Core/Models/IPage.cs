using MyScript.InteractiveInk.Enumerations;

namespace MyScript.OpenInk.Core.Models
{
    public interface IPage : IInteractiveInkModel
    {
        int Index { get; set; }
        bool IsDocument { get; }
        ContentType Type { get; set; }
    }
}
