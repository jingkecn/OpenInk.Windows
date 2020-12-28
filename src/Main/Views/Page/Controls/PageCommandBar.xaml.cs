using MyScript.OpenInk.Main.ViewModels;

namespace MyScript.OpenInk.Main.Views.Page.Controls
{
    public sealed partial class PageCommandBar
    {
        private PageViewModel _viewModel;

        public PageCommandBar()
        {
            InitializeComponent();
        }

        private PageViewModel ViewModel => _viewModel ??= DataContext as PageViewModel;
    }
}
