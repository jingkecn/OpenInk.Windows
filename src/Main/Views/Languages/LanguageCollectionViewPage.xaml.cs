using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MyScript.OpenInk.Core.Infrastructure.Extensions;
using MyScript.OpenInk.Core.Models;

namespace MyScript.OpenInk.Main.Views.Languages
{
    public sealed partial class LanguageCollectionViewPage
    {
        public LanguageCollectionViewPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ViewModel.Dispose();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await ViewModel.InitializeAsync();
        }

        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (!(e.ClickedItem is ILanguage language))
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
