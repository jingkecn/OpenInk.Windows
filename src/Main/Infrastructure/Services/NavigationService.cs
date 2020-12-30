using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using MyScript.OpenInk.Core.Infrastructure.Services;
using MyScript.OpenInk.Main.Configuration;

namespace MyScript.OpenInk.Main.Infrastructure.Services
{
    public partial class NavigationService
    {
        static NavigationService()
        {
            MainViewId = ApplicationView.GetForCurrentView().Id;
        }

        private static int MainViewId { get; }
    }

    public partial class NavigationService : INavigationService
    {
        public event NavigatedEventHandler Navigated;
        public event NavigationFailedEventHandler NavigationFailed;

        public async Task<ApplicationView> ShowAsStandaloneAsync(Type page, object parameter = null,
            NavigationTransitionInfo info = null, ApplicationViewSwitchingOptions options = default)
        {
            ApplicationView result = null;
            var view = CoreApplication.CreateNewView();
            await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                result = ApplicationView.GetForCurrentView();
                var frame = new Frame();
                var infrastructures = ServiceLocator.Current.Get<IInfrastructureServices>();
                infrastructures.ContextService.Initialize(result.Id, view.IsMain, view.Dispatcher);
                infrastructures.NavigationService.Initialize(frame);
                infrastructures.NavigationService.Navigate(page, parameter, info);
                infrastructures.ThemeService.CommandApplyTheme.Execute(infrastructures.ThemeService.Theme);
                Window.Current.Content = frame;
                Window.Current.Activate();
            });

            await SwitchAsync(result.Id, options);
            return result;
        }

        public async Task<ApplicationView> ShowAsStandaloneAsync<TPage>(object parameter = null,
            NavigationTransitionInfo info = null, ApplicationViewSwitchingOptions options = default)
            where TPage : Page
        {
            return await ShowAsStandaloneAsync(typeof(TPage), parameter, info, options);
        }

        public async Task SwitchAsync(int toViewId, ApplicationViewSwitchingOptions options = default)
        {
            await SwitchAsync(MainViewId, toViewId, options);
        }

        public async Task SwitchAsync(int fromViewId, int toViewId, ApplicationViewSwitchingOptions options = default)
        {
            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(fromViewId);
            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(toViewId);
            await ApplicationViewSwitcher.SwitchAsync(toViewId, fromViewId, options);
        }


        public void GoBack()
        {
            Frame?.GoBack();
        }

        public void Initialize(Frame frame)
        {
            if (Frame is { } previous)
            {
                previous.Navigated -= OnNavigated;
                previous.NavigationFailed -= OnNavigationFailed;
            }

            frame.Navigated += OnNavigated;
            frame.NavigationFailed += OnNavigationFailed;
            Frame = frame;
        }

        public bool Navigate(Type page, object parameter = null, NavigationTransitionInfo info = null)
        {
            return page.IsSubclassOf(typeof(Page)) && Frame is { } frame &&
                   (frame.Content?.GetType() != page || parameter != null) &&
                   frame.Navigate(page, parameter, info);
        }

        public bool Navigate<TPage>(object parameter = null, NavigationTransitionInfo info = null) where TPage : Page
        {
            return Navigate(typeof(TPage), parameter, info);
        }

        public bool CanGoBack => Frame is { } frame && frame.CanGoBack;

        public Frame Frame { get; private set; }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            Navigated?.Invoke(sender, e);
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            NavigationFailed?.Invoke(sender, e);
        }
    }
}
