namespace MyScript.OpenInk.Core.Commands
{
    public interface IInteractiveInkCommands
    {
        IBookCommands BookCommands { get; }
        IContentCommands ContentCommands { get; }
        ILanguageCommands LanguageCommands { get; }
        IPageCommands PageCommands { get; }
    }
}
