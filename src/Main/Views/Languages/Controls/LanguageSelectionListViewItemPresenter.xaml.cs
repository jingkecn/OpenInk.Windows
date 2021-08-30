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

        private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue is not ILanguage language)
            {
                return;
            }

            await ViewModel.InitializeAsync(language);
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.InitializeAsync();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Dispose();
        }
    }
}
