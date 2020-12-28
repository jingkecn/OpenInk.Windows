using Windows.UI.Xaml;
using MyScript.OpenInk.Core.Models;

namespace MyScript.OpenInk.Main.Views.Languages.Controls
{
    public sealed partial class LanguageSelectionListViewItemPresenter
    {
        public LanguageSelectionListViewItemPresenter()
        {
            InitializeComponent();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.InitializeAsync();
            await ViewModel.InitializeAsync(DataContext as ILanguage);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Dispose();
        }
    }
}
