using System.Windows.Input;
using Windows.Foundation;

namespace MyScript.OpenInk.Core.Commands
{
    public partial interface IContentCommands
    {
        ICommand CommandAdd { get; }
        ICommand CommandAppend { get; }
        ICommand CommandDelete { get; }
        ICommand CommandDeleteSelection { get; }
    }

    public partial interface IContentCommands
    {
        ICommand CommandRedo { get; }
        ICommand CommandUndo { get; }
    }

    public partial interface IContentCommands
    {
        ICommand CommandCopy { get; }
        ICommand CommandCopySelection { get; }
        ICommand CommandPaste { get; }
        ICommand CommandPasteSelection { get; }
    }

    public partial interface IContentCommands
    {
        ICommand CommandTypeset { get; }
    }

    public partial interface IContentCommands
    {
        Point ContextPosition { get; }
        void Initialize(Point position);
    }
}
