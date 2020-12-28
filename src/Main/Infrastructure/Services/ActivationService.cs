using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MyScript.OpenInk.Core.Infrastructure.Extensions;
using MyScript.OpenInk.Core.Infrastructure.Services;
using MyScript.OpenInk.Core.Services;
using MyScript.OpenInk.Main.Configuration;
using MyScript.OpenInk.Main.Constants;
using MyScript.OpenInk.Main.Infrastructure.Activation;
using MyScript.OpenInk.Main.Views;
using MyScript.OpenInk.Main.Views.Book;
using MyScript.OpenInk.Main.Views.Splash;

namespace MyScript.OpenInk.Main.Infrastructure.Services
{
    /// <summary>
    ///     Handles the applications initialization and activation.
    ///     For more information, see: https://github.com/Microsoft/WindowsTemplateStudio/blob/release/docs/UWP/activation.md
    /// </summary>
    public class ActivationService
    {
        private static ServiceCollection _services;

        public static ActivationHandler<FileActivatedEventArgs> FileActivationHandler { get; }
            = new FileActivationHandler(typeof(BookViewPage));

        public static ActivationHandler<LaunchActivatedEventArgs> LaunchActivationHandler { get; }
            = new LaunchActivationHandler(typeof(MainPage));

        public static ActivationHandler<ToastNotificationActivatedEventArgs> ToastNotificationActivationHandler { get; }
            = new ToastNotificationActivationHandler(typeof(MainPage));

        private static IInfrastructureServices InfrastructureServices =>
            ServiceLocator.Current.Get<IInfrastructureServices>();

        private static IInteractiveInkServices InteractiveInkServices =>
            ServiceLocator.Current.Get<IInteractiveInkServices>();

        private static bool IsLanguageServiceInitializationDeferralCompleted { get; set; }
        private static Deferral LanguageServiceInitializationDeferral { get; set; }

        private static ServiceCollection Services => _services ??= new ServiceCollection();

        /// <summary>
        ///     With the method <see cref="ActivateAsync" />,
        ///     it has one common entry point that is called from the app lifecycle events:
        ///     <see cref="Application.OnLaunched" />,
        ///     <see cref="Application.OnActivated" /> and
        ///     <see cref="Application.OnBackgroundActivated" />.
        ///     For more information, see: https://docs.microsoft.com/windows/uwp/launch-resume/app-lifecycle.
        /// </summary>
        /// <param name="args">
        ///     <inheritdoc cref="IActivatedEventArgs" />
        /// </param>
        /// <returns>
        ///     <inheritdoc cref="Task" />
        /// </returns>
        public async Task ActivateAsync(IActivatedEventArgs args)
        {
            await InitializeAsync(args);

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            Window.Current.Content ??= new ExtendedSplash();

            // Depending on activationArgs one of ActivationHandlers or DefaultActivationHandler
            // will navigate to the first page
            await HandleActivationAsync(args);

            // Ensure the current window is active
            Window.Current.Activate();

            // Tasks after activation
            await StartupAsync();
        }

        /// <summary>
        ///     Initializes services that you need before app activation take into account that the splash screen is shown while
        ///     this code runs.
        /// </summary>
        /// <returns></returns>
        private static async Task InitializeAsync(IActivatedEventArgs args)
        {
            ServiceLocator.Configure(Services);
            LanguageServiceInitializationDeferral ??= await InteractiveInkServices.LanguageService
                .InitializeAsync(new Uri(Urls.LanguagesJsonUrl), args is ToastNotificationActivatedEventArgs);
        }

        private static async Task HandleActivationAsync(IActivatedEventArgs args)
        {
            switch (args)
            {
                case FileActivatedEventArgs file:
                    await FileActivationHandler.HandleAsync(file);
                    break;
                case LaunchActivatedEventArgs launch:
                    await LaunchActivationHandler.HandleAsync(launch);
                    break;
                case ToastNotificationActivatedEventArgs toast:
                    await ToastNotificationActivationHandler.HandleAsync(toast);
                    break;
            }
        }

        private static async Task StartupAsync()
        {
            // Infrastructures
            InfrastructureServices.ThemeService.CommandApplyTheme.Execute(InfrastructureServices.ThemeService.Theme);
            // Interactive Ink
            await HandleBundledResources();
            if (IsLanguageServiceInitializationDeferralCompleted)
            {
                return;
            }

            LanguageServiceInitializationDeferral?.Complete();
            IsLanguageServiceInitializationDeferralCompleted = true;
            await Task.CompletedTask;
        }

        private static async Task HandleBundledResources()
        {
            var destination = ApplicationData.Current.LocalFolder;
            var destinationAssets = await destination.CreateFolderAsync("Assets", CreationCollisionOption.OpenIfExists);
            var sourceAssets = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
            if (!Directory.Exists(Path.Combine(destinationAssets.Path, "conf")))
            {
                var sourceConfigurations = await sourceAssets.GetFolderAsync("conf");
                await sourceConfigurations.CopyAsync(destinationAssets);
            }

            if (!Directory.Exists(Path.Combine(destinationAssets.Path, "resources")))
            {
                var sourceResources = await sourceAssets.GetFolderAsync("resources");
                await sourceResources.CopyAsync(destinationAssets);
            }

            await Task.CompletedTask;
        }
    }
}
