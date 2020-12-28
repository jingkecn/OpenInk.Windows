using System;
using System.Collections.ObjectModel;
using Windows.Networking.BackgroundTransfer;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyScript.OpenInk.Core.Infrastructure.Extensions;
using MyScript.OpenInk.Core.Models;
using MyScript.OpenInk.Core.Services;
using MyScript.OpenInk.Main.ViewModels;

namespace MyScript.OpenInk.Main.Views.Dialogs
{
    public sealed partial class LanguageRequiringDialog
    {
        private PageViewModel _viewModel;

        public LanguageRequiringDialog()
        {
            InitializeComponent();
        }

        private ObservableCollection<ILanguage> RequiredLanguages { get; } = new();
        private ILanguageService LanguageService => ViewModel.InteractiveInkServices.LanguageService;
        private PageViewModel ViewModel => _viewModel ??= DataContext as PageViewModel;

        private void OnClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            args.Cancel = !(ViewModel.Book.Language is { } language) || !LanguageService.IsInstalled(language);
        }

        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (!(e.ClickedItem is ILanguage language))
            {
                return;
            }

            var commands = ViewModel.InteractiveInkCommands.LanguageCommands;
            var progress = LanguageService.GetDownloadProgress(language);
            if (progress.Status.IsPaused())
            {
                commands.CommandResume.Execute(language);
                return;
            }

            if (progress.Status.IsRunning())
            {
                commands.CommandPause.Execute(language);
                return;
            }

            commands.CommandInstall.Execute(language);
        }

        private void OnLanguageInitialized(ILanguage sender, BackgroundDownloadProgress args)
        {
            if (sender.Id != ViewModel.Book.Language.Id)
            {
                return;
            }

            if (!LanguageService.IsInstalled(sender))
            {
                return;
            }

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Hide).AsTask();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            RequiredLanguages.Add(ViewModel.Book.Language);
            ViewModel.InteractiveInkServices.LanguageService.Initialized += OnLanguageInitialized;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.InteractiveInkServices.LanguageService.Initialized -= OnLanguageInitialized;
        }
    }
}
