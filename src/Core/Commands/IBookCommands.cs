using System.Windows.Input;
using MyScript.OpenInk.Core.Services;

namespace MyScript.OpenInk.Core.Commands
{
    public partial interface IBookCommands
    {
        IInteractiveInkServices InteractiveInkServices { get; }
    }

    public partial interface IBookCommands
    {
        ICommand CommandCreate { get; }
        ICommand CommandOpen { get; }
        ICommand CommandSave { get; }
    }
}
