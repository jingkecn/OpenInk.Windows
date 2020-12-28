namespace MyScript.OpenInk.Core.Services
{
    public interface IInteractiveInkServices
    {
        IEditorService EditorService { get; }
        IEngineService EngineService { get; }
        ILanguageService LanguageService { get; }
    }
}
