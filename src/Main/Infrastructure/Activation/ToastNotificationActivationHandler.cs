using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using MyScript.OpenInk.Core.Infrastructure.Extensions;

namespace MyScript.OpenInk.Main.Infrastructure.Activation
{
    public class ToastNotificationActivationHandler : ActivationHandler<ToastNotificationActivatedEventArgs>
    {
        public ToastNotificationActivationHandler(Type defaultPage) : base(defaultPage)
        {
        }

        public bool Handled { get; private set; }

        protected override bool CanHandle(ToastNotificationActivatedEventArgs args)
        {
            return true;
        }

        public override async Task HandleAsync(ToastNotificationActivatedEventArgs args)
        {
            await base.HandleAsync(args);
            Handled = true;
            if (!(RootElement is Frame frame))
            {
                Window.Current.Content = frame = new Frame {IsNavigationStackEnabled = false};
            }

            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                await InfrastructureServices.NavigationService.SwitchAsync(InfrastructureServices.ContextService.Id);
            }

            InfrastructureServices.NavigationService.Initialize(frame);
            InfrastructureServices.NavigationService.Navigate(DefaultPage, args.Argument.ToArguments(),
                new SuppressNavigationTransitionInfo());
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
