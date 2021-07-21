using Windows.UI.Xaml.Controls;
using MyScript.OpenInk.Core.Infrastructure.Extensions;
using MyScript.OpenInk.Core.Models;
using MyScript.OpenInk.Main.ViewModels;

namespace MyScript.OpenInk.Main.Views.Settings.Controls
{
    public sealed partial class LanguageManager
    {
        private SettingsViewModel _viewModel;

        public LanguageManager()
        {
            InitializeComponent();
        }

        private SettingsViewModel ViewModel => _viewModel ??= DataContext as SettingsViewModel;

        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is not ILanguage language)
            {
                return;
            }

            var commands = ViewModel.InteractiveInkCommands.LanguageCommands;
            var progress = ViewModel.InteractiveInkServices.LanguageService.GetDownloadProgress(language);
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
    }
}
