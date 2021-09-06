using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyScript.InteractiveInk.Annotations;
using MyScript.OpenInk.Main.Infrastructure.Services;
using MyScript.OpenInk.Main.Views.Splash;

namespace MyScript.OpenInk.Main.Infrastructure.Activation
{
    public class FileActivationHandler : ActivationHandler<FileActivatedEventArgs>
    {
        internal FileActivationHandler(Type page) : base(page)
        {
        }

        private static bool IsLaunchActivationHandled =>
            (ActivationService.LaunchActivationHandler is LaunchActivationHandler launch && launch.Handled) ||
            (ActivationService.ToastNotificationActivationHandler is ToastNotificationActivationHandler toast &&
             toast.Handled);

        private Dictionary<string, ApplicationView> Views { get; } = new();

        protected override bool CanHandle(FileActivatedEventArgs args)
        {
            return args.Files.OfType<StorageFile>().Any();
        }

        public override async Task HandleAsync(FileActivatedEventArgs args)
        {
            await base.HandleAsync(args);
            if (!CanHandle(args))
            {
                return;
            }

            if (RootElement is Frame { Content: null })
            {
                Window.Current.Content = new ExtendedSplash();
            }

            var options = IsLaunchActivationHandled
                ? ApplicationViewSwitchingOptions.Default
                : ApplicationViewSwitchingOptions.ConsolidateViews;
            await InfrastructureServices.ContextService.RunAsync(async () =>
                await HandleFilesAsync(args.Files.OfType<StorageFile>().Where(file => file.IsAvailable), options));
            InfrastructureServices.ContextService.Consolidated += OnConsolidated;
        }

        protected override async Task HandleErrorAsync(FileActivatedEventArgs args)
        {
            await base.HandleErrorAsync(args);
            var dialog = new MessageDialog($"Cannot {args.Verb} file.", Package.Current.DisplayName);
            await dialog.ShowAsync();
        }

        private async Task HandleFilesAsync([NotNull] IEnumerable<StorageFile> files,
            ApplicationViewSwitchingOptions options)
        {
            foreach (var file in files)
            {
                await HandleFileAsync(file, options);
            }
        }

        public async Task HandleFileAsync([NotNull] IStorageItem file, ApplicationViewSwitchingOptions options)
        {
            if (Views.ContainsKey(file.Path))
            {
                var previous = Views[file.Path];
                await InfrastructureServices.NavigationService.SwitchAsync(previous.Id, options);
                return;
            }

            var view = await InfrastructureServices.NavigationService.ShowAsStandaloneAsync(DefaultPage, file,
                options: options);
            view.Consolidated += OnConsolidated;
            Views[file.Path] = view;
        }

        private void OnConsolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            using var deferral = new Deferral(() => sender.Consolidated -= OnConsolidated);
            if (!Views.ContainsValue(sender))
            {
                return;
            }

            var key = Views.SingleOrDefault(pair => pair.Value == sender).Key;
            if (!Views.ContainsKey(key))
            {
                return;
            }

            Views.Remove(key);
            deferral.Complete();
        }
    }
}
