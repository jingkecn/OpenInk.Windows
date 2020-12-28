using System.Windows.Input;
using MyScript.OpenInk.Core.Services;

namespace MyScript.OpenInk.Core.Commands
{
    public partial interface ILanguageCommands
    {
        ICommand CommandCancel { get; }
        ICommand CommandInstall { get; }
        ICommand CommandPause { get; }
        ICommand CommandResume { get; }
        ICommand CommandUninstall { get; }
    }

    public partial interface ILanguageCommands
    {
        IInteractiveInkServices InteractiveInkServices { get; }
    }
}
