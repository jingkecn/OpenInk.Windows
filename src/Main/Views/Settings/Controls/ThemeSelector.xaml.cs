using MyScript.OpenInk.Main.ViewModels;

namespace MyScript.OpenInk.Main.Views.Settings.Controls
{
    public sealed partial class ThemeSelector
    {
        private SettingsViewModel _viewModel;

        public ThemeSelector()
        {
            InitializeComponent();
        }

        private SettingsViewModel ViewModel => _viewModel ??= DataContext as SettingsViewModel;
    }
}
