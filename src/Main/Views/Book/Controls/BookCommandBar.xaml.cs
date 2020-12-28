using MyScript.OpenInk.Main.ViewModels;

namespace MyScript.OpenInk.Main.Views.Book.Controls
{
    public sealed partial class BookCommandBar
    {
        private BookViewModel _viewModel;

        public BookCommandBar()
        {
            InitializeComponent();
        }

        private BookViewModel ViewModel => _viewModel ??= DataContext as BookViewModel;
    }
}
