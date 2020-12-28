using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;

namespace MyScript.OpenInk.Main.Views.Book
{
    public sealed partial class BookViewPage
    {
        public BookViewPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.Dispose();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await ViewModel.InitializeAsync();
            switch (e.Parameter)
            {
                case StorageFile file:
                    await ViewModel.InitializeAsync(file, e.NavigationMode);
                    break;
                case string path:
                    await ViewModel.InitializeAsync(path, e.NavigationMode);
                    break;
                case IDictionary<string, string> arguments:
                    await ViewModel.InitializeAsync(arguments, e.NavigationMode);
                    break;
                default:
                    await ViewModel.InitializeAsync(ViewModel.Book);
                    break;
            }
        }
    }
}
