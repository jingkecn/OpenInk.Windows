using MyScript.OpenInk.Main.ViewModels;

namespace MyScript.OpenInk.Main.Views.Book.Controls
{
    public sealed partial class BookStatusBar
    {
        private BookViewModel _viewModel;

        public BookStatusBar()
        {
            InitializeComponent();
        }

        private BookViewModel ViewModel => _viewModel ??= DataContext as BookViewModel;
    }
}
