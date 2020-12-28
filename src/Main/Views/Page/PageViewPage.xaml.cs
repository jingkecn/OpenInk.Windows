using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MyScript.OpenInk.Core.Models;

namespace MyScript.OpenInk.Main.Views.Page
{
    public sealed partial class PageViewPage
    {
        public PageViewPage()
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
            await ViewModel.InitializeAsync(InAppNotification);
            await ViewModel.InitializeAsync(InteractiveInkCanvas);
            await ViewModel.InitializeAsync(e.Parameter as IPage);
        }

        private void OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            ViewModel.Initialize(e.GetPosition(sender as UIElement));
        }
    }
}
