using MyScript.OpenInk.Main.ViewModels;

namespace MyScript.OpenInk.Main.Views.Book.Controls
{
    public sealed partial class BookLoading
    {
        private BookViewModel _viewModel;

        public BookLoading()
        {
            InitializeComponent();
        }

        private BookViewModel ViewModel => _viewModel ??= DataContext as BookViewModel;
    }
}
