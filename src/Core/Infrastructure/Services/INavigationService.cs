using System;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using MyScript.InteractiveInk.Annotations;

namespace MyScript.OpenInk.Core.Infrastructure.Services
{
    public partial interface INavigationService
    {
        bool CanGoBack { get; }
        [CanBeNull] Frame Frame { get; }
    }

    public partial interface INavigationService
    {
        void GoBack();

        void Initialize([NotNull] Frame frame);

        bool Navigate([NotNull] Type page, [CanBeNull] object parameter = null,
            [CanBeNull] NavigationTransitionInfo info = null);

        bool Navigate<TPage>([CanBeNull] object parameter = null,
            [CanBeNull] NavigationTransitionInfo info = null) where TPage : Page;
    }

    public partial interface INavigationService
    {
        Task<ApplicationView> ShowAsStandaloneAsync([NotNull] Type page, [CanBeNull] object parameter = null,
            [CanBeNull] NavigationTransitionInfo info = null, ApplicationViewSwitchingOptions options = default);

        Task<ApplicationView> ShowAsStandaloneAsync<TPage>([CanBeNull] object parameter = null,
            [CanBeNull] NavigationTransitionInfo info = null, ApplicationViewSwitchingOptions options = default)
            where TPage : Page;

        Task SwitchAsync(int toViewId, ApplicationViewSwitchingOptions options = default);
        Task SwitchAsync(int fromViewId, int toViewId, ApplicationViewSwitchingOptions options = default);
    }


    public partial interface INavigationService
    {
        /// <summary>
        ///     <inheritdoc cref="IFrame.Navigated" />
        /// </summary>
        event NavigatedEventHandler Navigated;

        /// <summary>
        ///     <inheritdoc cref="IFrame.NavigationFailed" />
        /// </summary>
        event NavigationFailedEventHandler NavigationFailed;
    }
}
