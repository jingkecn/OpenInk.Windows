using MyScript.OpenInk.Main.ViewModels;

namespace MyScript.OpenInk.Main.Views.Dashboard.Dialogs
{
    public sealed partial class LanguageSelector
    {
        private DashboardViewModel _viewModel;

        public LanguageSelector()
        {
            InitializeComponent();
        }

        private DashboardViewModel ViewModel => _viewModel ??= DataContext as DashboardViewModel;
    }
}
