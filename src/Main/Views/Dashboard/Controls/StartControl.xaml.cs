using MyScript.OpenInk.Main.ViewModels;

namespace MyScript.OpenInk.Main.Views.Dashboard.Controls
{
    public sealed partial class StartControl
    {
        private DashboardViewModel _viewModel;

        public StartControl()
        {
            InitializeComponent();
        }

        private DashboardViewModel ViewModel => _viewModel ??= DataContext as DashboardViewModel;
    }
}
