using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MyScript.OpenInk.Main.Views.Recent
{
    public sealed partial class RecentViewPage
    {
        public RecentViewPage()
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
        }

        private async void OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (!(e.ClickedItem is StorageFile file))
            {
                return;
            }

            await ViewModel.OpenAsync(file);
        }
    }
}
