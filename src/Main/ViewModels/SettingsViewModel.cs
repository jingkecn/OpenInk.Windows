using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.Networking.BackgroundTransfer;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Helpers;
using MyScript.InteractiveInk.Annotations;
using MyScript.OpenInk.Core.Infrastructure.Commands;
using MyScript.OpenInk.Core.Infrastructure.Extensions;
using MyScript.OpenInk.Core.Models;
using MyScript.OpenInk.Core.Services;
using MyScript.OpenInk.Core.ViewModels;
using MyScript.OpenInk.Main.Views.Languages;

namespace MyScript.OpenInk.Main.ViewModels
{
    public partial class SettingsViewModel : InteractiveInkViewModel
    {
        private ILanguageService LanguageService => InteractiveInkServices.LanguageService;

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            InstalledLanguages.SyncWith(
                InteractiveInkServices.LanguageService.Languages.Where(x =>
                    LanguageService.IsInstalled(x) || LanguageService.IsInstalling(x)));
            InstalledLanguages.SortBy(x => x.Id);
            InteractiveInkServices.LanguageService.Downloading += OnDownloading;
            InteractiveInkServices.LanguageService.Initialized += OnInitialized;
            InteractiveInkServices.LanguageService.Initializing += OnInitializing;
        }

        public override async Task InitializeAsync([NotNull] IDictionary<string, string> arguments,
            NavigationMode mode = NavigationMode.Refresh)
        {
            await base.InitializeAsync(arguments, mode);
            if (mode != NavigationMode.New ||
                !arguments.TryGetValue("view", out var view) || view != "settings" ||
                !arguments.TryGetValue("action", out var action))
            {
                return;
            }

            var languages = InteractiveInkServices.LanguageService.Languages;
            switch (action)
            {
                case "manageLanguages":
                    CommandManageLanguages.Execute(null);
                    break;
                case "installLanguage":
                    if (!arguments.TryGetValue("language", out var idToInstall) ||
                        !(languages.SingleOrDefault(x => x.Id == idToInstall) is { } languageToInstall))
                    {
                        return;
                    }

                    InteractiveInkCommands.LanguageCommands.CommandInstall.Execute(languageToInstall);
                    break;
                case "resumeLanguage":
                    if (!arguments.TryGetValue("language", out var idToResume) ||
                        !(languages.SingleOrDefault(x => x.Id == idToResume) is { } languageToResume))
                    {
                        return;
                    }

                    InteractiveInkCommands.LanguageCommands.CommandResume.Execute(languageToResume);
                    break;
            }

            await Task.CompletedTask;
        }

        protected override void ReleaseManagedResources()
        {
            InteractiveInkServices.LanguageService.Downloading -= OnDownloading;
            InteractiveInkServices.LanguageService.Initialized -= OnInitialized;
            InteractiveInkServices.LanguageService.Initializing -= OnInitializing;
            base.ReleaseManagedResources();
        }

        private void TryShowLanguagesPane()
        {
            PaneFrame.Navigate(typeof(LanguageCollectionViewPage), null, new SuppressNavigationTransitionInfo());
            IsPaneOpen = true;
        }
    }

    public partial class SettingsViewModel : ISettingsViewModel
    {
        private ICommand _commandManageLanguages;

        public ICommand CommandManageLanguages =>
            _commandManageLanguages ??= new RelayCommand(_ => TryShowLanguagesPane());

        public ObservableCollection<ILanguage> InstalledLanguages { get; } = new();
        public string Version => Package.Current.Id.Version.ToFormattedString();

        private async void OnDownloading(ILanguage sender, BackgroundDownloadProgress args)
        {
            await InfrastructureServices.ContextService.RunAsync(() =>
            {
                if (InstalledLanguages.Contains(sender))
                {
                    return;
                }

                InstalledLanguages.Add(sender);
                InstalledLanguages.SortBy(x => x.Id);
            });
        }

        private async void OnInitialized(ILanguage sender, BackgroundDownloadProgress args)
        {
            await InfrastructureServices.ContextService.RunAsync(() =>
            {
                if (!LanguageService.IsInstalled(sender))
                {
                    InstalledLanguages.Remove(sender);
                    return;
                }

                if (InstalledLanguages.Contains(sender))
                {
                    return;
                }

                InstalledLanguages.Add(sender);
                InstalledLanguages.SortBy(x => x.Id);
            });
        }

        private async void OnInitializing(ILanguage sender, BackgroundDownloadProgress args)
        {
            await InfrastructureServices.ContextService.RunAsync(() =>
            {
                if (InstalledLanguages.Contains(sender))
                {
                    return;
                }

                InstalledLanguages.Add(sender);
                InstalledLanguages.SortBy(x => x.Id);
            });
        }
    }
}
