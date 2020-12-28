using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MyScript.InteractiveInk.Annotations;
using MyScript.OpenInk.Core.Infrastructure.Services;
using MyScript.OpenInk.Core.Services;
using MyScript.OpenInk.Main.Configuration;

namespace MyScript.OpenInk.Main.Infrastructure.Activation
{
    public abstract class ActivationHandler<TArgs> where TArgs : IActivatedEventArgs
    {
        protected ActivationHandler(Type defaultPage)
        {
            if (!defaultPage.IsSubclassOf(typeof(Page)))
            {
                throw new ArgumentException($"{nameof(defaultPage)} must be a {nameof(Page)} type.");
            }

            DefaultPage = defaultPage;
        }

        [CanBeNull] protected static UIElement RootElement => Window.Current.Content;

        protected static IInfrastructureServices InfrastructureServices =>
            ServiceLocator.Current.Get<IInfrastructureServices>();

        protected static IInteractiveInkServices InteractiveInkServices =>
            ServiceLocator.Current.Get<IInteractiveInkServices>();

        protected Type DefaultPage { get; }

        protected abstract bool CanHandle(TArgs args);

        public virtual async Task HandleAsync(TArgs args)
        {
            if (!CanHandle(args))
            {
                await HandleErrorAsync(args);
                Application.Current.Exit();
                return;
            }

            InfrastructureServices.ContextService.Initialize(
                ApplicationView.GetForCurrentView().Id,
                CoreApplication.GetCurrentView().IsMain,
                Window.Current.Dispatcher);
            InfrastructureServices.NavigationService.NavigationFailed += OnNavigationFailed;
            await Task.CompletedTask;
        }

        protected virtual async Task HandleErrorAsync(TArgs args)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        ///     Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private static void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception($"Failed to load Page {e.SourcePageType.FullName}");
        }
    }
}
