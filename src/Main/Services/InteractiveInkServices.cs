using System;
using MyScript.OpenInk.Core.Infrastructure.Patterns;
using MyScript.OpenInk.Core.Services;
using MyScript.OpenInk.Main.Configuration;

namespace MyScript.OpenInk.Main.Services
{
    public partial class InteractiveInkServices : Disposable
    {
        protected override void ReleaseManagedResources()
        {
            (EditorService as IDisposable)?.Dispose();
            (EngineService as IDisposable)?.Dispose();
            (LanguageService as IDisposable)?.Dispose();
        }
    }

    public partial class InteractiveInkServices : IInteractiveInkServices
    {
        public IEditorService EditorService { get; } = ServiceLocator.Current.Get<IEditorService>();
        public IEngineService EngineService { get; } = ServiceLocator.Current.Get<IEngineService>();
        public ILanguageService LanguageService { get; } = ServiceLocator.Current.Get<ILanguageService>();
    }
}
