using MyScript.OpenInk.Core.Commands;
using MyScript.OpenInk.Main.Configuration;

namespace MyScript.OpenInk.Main.Commands
{
    public class InteractiveInkCommands : IInteractiveInkCommands
    {
        public IBookCommands BookCommands => ServiceLocator.Current.Get<IBookCommands>();
        public IContentCommands ContentCommands => ServiceLocator.Current.Get<IContentCommands>();
        public ILanguageCommands LanguageCommands => ServiceLocator.Current.Get<ILanguageCommands>();
        public IPageCommands PageCommands => ServiceLocator.Current.Get<IPageCommands>();
    }
}
