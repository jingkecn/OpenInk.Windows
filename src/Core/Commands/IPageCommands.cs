using System.Windows.Input;

namespace MyScript.OpenInk.Core.Commands
{
    public partial interface IPageCommands
    {
        ICommand CommandAdd { get; }
        ICommand CommandDelete { get; }
        ICommand CommandOpen { get; }
    }

    public partial interface IPageCommands
    {
        ICommand CommandGoBack { get; }
        ICommand CommandGoForward { get; }
    }
}
