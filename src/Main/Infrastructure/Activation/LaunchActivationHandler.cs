using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace MyScript.OpenInk.Main.Infrastructure.Activation
{
    public class LaunchActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
    {
        internal LaunchActivationHandler(Type defaultPage) : base(defaultPage)
        {
        }

        public bool Handled { get; private set; }

        protected override bool CanHandle(LaunchActivatedEventArgs args)
        {
            return !args.PrelaunchActivated;
        }

        public override async Task HandleAsync(LaunchActivatedEventArgs args)
        {
            await base.HandleAsync(args);
            if (!CanHandle(args))
            {
                return;
            }

            Handled = true;
            if (args.PreviousExecutionState == ApplicationExecutionState.Running ||
                args.PreviousExecutionState == ApplicationExecutionState.Suspended)
            {
                await InfrastructureServices.NavigationService.SwitchAsync(InfrastructureServices.ContextService.Id);
            }

            if (!(RootElement is Frame frame))
            {
                Window.Current.Content = frame = new Frame {IsNavigationStackEnabled = false};
            }

            if (frame.Content?.GetType() != DefaultPage)
            {
                InfrastructureServices.NavigationService.Initialize(frame);
                InfrastructureServices.NavigationService.Navigate(DefaultPage, args.Arguments,
                    new SuppressNavigationTransitionInfo());
            }

            InfrastructureServices.ContextService.Consolidated += OnConsolidated;
        }

        private void OnConsolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            if (RootElement is Frame frame)
            {
                frame.Content = null;
            }

            Handled = false;
        }
    }
}
