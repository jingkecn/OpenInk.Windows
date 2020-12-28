using MyScript.OpenInk.Core.Commands;
using MyScript.OpenInk.Core.Services;

namespace MyScript.OpenInk.Core.Models
{
    public interface IInteractiveInkModel
    {
        IInteractiveInkCommands InteractiveInkCommands { get; }
        IInteractiveInkServices InteractiveInkServices { get; }
    }
}
