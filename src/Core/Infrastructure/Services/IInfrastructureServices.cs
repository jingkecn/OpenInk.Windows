namespace MyScript.OpenInk.Core.Infrastructure.Services
{
    public interface IInfrastructureServices
    {
        IContextService ContextService { get; }
        INavigationService NavigationService { get; }
        IThemeService ThemeService { get; }
    }
}
