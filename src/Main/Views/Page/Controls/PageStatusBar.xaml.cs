using MyScript.OpenInk.Main.ViewModels;

namespace MyScript.OpenInk.Main.Views.Page.Controls
{
    public sealed partial class PageStatusBar
    {
        private PageViewModel _viewModel;

        public PageStatusBar()
        {
            InitializeComponent();
        }

        private PageViewModel ViewModel => _viewModel ??= DataContext as PageViewModel;
    }
}
