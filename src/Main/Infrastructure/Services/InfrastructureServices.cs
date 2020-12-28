using System;
using MyScript.OpenInk.Core.Infrastructure.Patterns;
using MyScript.OpenInk.Core.Infrastructure.Services;
using MyScript.OpenInk.Main.Configuration;

namespace MyScript.OpenInk.Main.Infrastructure.Services
{
    public partial class InfrastructureServices : Disposable
    {
        protected override void ReleaseManagedResources()
        {
            (ContextService as IDisposable)?.Dispose();
        }
    }

    public partial class InfrastructureServices : IInfrastructureServices
    {
        public IContextService ContextService { get; } = ServiceLocator.Current.Get<IContextService>();
        public INavigationService NavigationService { get; } = ServiceLocator.Current.Get<INavigationService>();
        public IThemeService ThemeService { get; } = ServiceLocator.Current.Get<IThemeService>();
    }
}
