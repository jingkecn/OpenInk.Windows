using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Windows.UI.ViewManagement;
using Microsoft.Extensions.DependencyInjection;
using MyScript.InteractiveInk.Annotations;
using MyScript.OpenInk.Core.Commands;
using MyScript.OpenInk.Core.Infrastructure.Patterns;
using MyScript.OpenInk.Core.Infrastructure.Services;
using MyScript.OpenInk.Core.Services;
using MyScript.OpenInk.Main.Commands;
using MyScript.OpenInk.Main.Infrastructure.Services;
using MyScript.OpenInk.Main.Services;

namespace MyScript.OpenInk.Main.Configuration
{
    public partial class ServiceLocator
    {
        private static readonly ConcurrentDictionary<int, ServiceLocator> Instances = new();

        [NotNull]
        public static ServiceLocator Current =>
            Instances.GetOrAdd(ApplicationView.GetForCurrentView().Id, _ => new ServiceLocator());

        private static bool IsConfigured { get; set; }
        [NotNull] private static ServiceProvider Provider { get; set; }

        public static void Configure([NotNull] IServiceCollection services)
        {
            if (IsConfigured)
            {
                return;
            }

            // Infrastructures
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddScoped<IContextService, ContextService>();
            services.AddScoped<INavigationService, NavigationService>();
            // MyScript Interactive Ink SDK
            services.AddSingleton<ILanguageService, LanguageService>();
            services.AddScoped<IEngineService, EngineService>();
            services.AddScoped<IEditorService, EditorService>();
            // Notebook
            services.AddScoped<IContentCommands, ContentCommands>();
            services.AddScoped<IBookCommands, BookCommands>();
            services.AddScoped<ILanguageCommands, LanguageCommands>();
            services.AddScoped<IPageCommands, PageCommands>();
            // Aggregation
            services.AddTransient<IInfrastructureServices, InfrastructureServices>();
            services.AddTransient<IInteractiveInkCommands, InteractiveInkCommands>();
            services.AddTransient<IInteractiveInkServices, InteractiveInkServices>();
            // Build services
            Provider = services.BuildServiceProvider();
            IsConfigured = true;
        }
    }

    public partial class ServiceLocator
    {
        private ServiceLocator()
        {
            Scope = Provider.CreateScope();
        }

        [NotNull] private IServiceScope Scope { get; }

        public TService Get<TService>(bool isRequired = true)
        {
            var provider = Scope.ServiceProvider;
            return isRequired ? provider.GetService<TService>() : provider.GetRequiredService<TService>();
        }
    }

    public partial class ServiceLocator : Disposable
    {
        protected override void ReleaseManagedResources()
        {
            Debug.WriteLine(
                $"{nameof(ServiceLocator)}.{nameof(ReleaseManagedResources)}:" +
                $"\n\t{Current.Get<IContextService>()}");
            (Current.Get<IInfrastructureServices>() as IDisposable)?.Dispose();
            (Current.Get<IInteractiveInkServices>() as IDisposable)?.Dispose();
            Instances.TryRemove(ApplicationView.GetForCurrentView().Id, out _);
        }
    }
}
