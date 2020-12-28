using Windows.UI.Xaml.Navigation;

namespace MyScript.OpenInk.Main.Views.Dashboard
{
    public sealed partial class DashboardViewPage
    {
        public DashboardViewPage()
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
    }
}
