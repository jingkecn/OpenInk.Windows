using MyScript.OpenInk.Main.ViewModels;

namespace MyScript.OpenInk.Main.Views.Settings.Controls
{
    public sealed partial class About
    {
        private SettingsViewModel _viewModel;

        public About()
        {
            InitializeComponent();
        }

        private SettingsViewModel ViewModel => _viewModel ??= DataContext as SettingsViewModel;
    }
}
