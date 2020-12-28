using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Controls;
using MyScript.OpenInk.Main.Views.Settings;
using MyScript.OpenInk.UI.Infrastructure.Extensions;

namespace MyScript.OpenInk.Main.Views
{
    public sealed partial class MainPage
    {
        public MainPage()
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
            await ViewModel.InitializeAsync(RootFrame);
            ViewModel.Initialize(NavigationView.MenuItems.OfType<NavigationViewItem>());
            if (!(e.Parameter is IDictionary<string, string> arguments))
            {
                return;
            }

            ViewModel.Initialize(arguments);
        }

        private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                ViewModel.Initialize(args.InvokedItemContainer as NavigationViewItem);
                ViewModel.InfrastructureServices.NavigationService.Navigate<SettingsPage>(
                    info: args.RecommendedNavigationTransitionInfo);
                return;
            }

            (args.InvokedItemContainer as NavigationViewItem)?.Navigate(
                ViewModel.InfrastructureServices.NavigationService,
                info: args.RecommendedNavigationTransitionInfo);
        }
    }
}
