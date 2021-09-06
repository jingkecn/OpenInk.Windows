using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MyScript.OpenInk.Core.Infrastructure.Services;
using MyScript.OpenInk.Core.Infrastructure.ViewModels;
using MyScript.OpenInk.Main.Configuration;
using MyScript.OpenInk.UI.Infrastructure.ViewModels;

namespace MyScript.OpenInk.Main.Infrastructure.ViewModels
{
    public abstract partial class InfrastructureViewModel
    {
        public virtual async Task InitializeAsync()
        {
            CanGoBack = InfrastructureServices.NavigationService.CanGoBack;
            IsInMainView = InfrastructureServices.ContextService.IsMain;
            InfrastructureServices.ContextService.BackRequested += OnBackRequested;
            InfrastructureServices.NavigationService.Navigated += OnNavigated;
            await Task.CompletedTask;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = InfrastructureServices.NavigationService.CanGoBack;
            if (!e.Handled)
            {
                return;
            }

            InfrastructureServices.NavigationService.GoBack();
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            CanGoBack = InfrastructureServices.NavigationService.CanGoBack;
        }
    }

    public abstract partial class InfrastructureViewModel : DisposableViewModel
    {
        public override IInfrastructureServices InfrastructureServices { get; } =
            ServiceLocator.Current.Get<IInfrastructureServices>();

        protected override void ReleaseManagedResources()
        {
            InfrastructureServices.ContextService.BackRequested -= OnBackRequested;
            InfrastructureServices.NavigationService.Navigated -= OnNavigated;
        }
    }

    public abstract partial class InfrastructureViewModel : IContextViewModel
    {
        private bool _isInMainView;

        public bool IsInMainView
        {
            get => _isInMainView;
            set => Set(ref _isInMainView, value, nameof(IsInMainView));
        }
    }

    public abstract partial class InfrastructureViewModel : IInfoBarViewModel
    {
        private string _infoMessage;
        private string _infoTitle;
        private bool _isInfoBarOpen;

        public bool IsInfoBarOpen
        {
            get => _isInfoBarOpen;
            set => Set(ref _isInfoBarOpen, value, nameof(IsInfoBarOpen));
        }

        public string InfoMessage
        {
            get => _infoMessage;
            set => Set(ref _infoMessage, value, nameof(InfoMessage));
        }

        public string InfoTitle
        {
            get => _infoTitle;
            set => Set(ref _infoTitle, value, nameof(InfoTitle));
        }
    }

    public abstract partial class InfrastructureViewModel : INavigationViewModel
    {
        private bool _canGoBack;

        public bool CanGoBack
        {
            get => _canGoBack;
            set => Set(ref _canGoBack, value, nameof(CanGoBack));
        }

        public async Task InitializeAsync(Frame frame)
        {
            InfrastructureServices.NavigationService.Initialize(frame);
            await Task.CompletedTask;
        }

        public virtual async Task InitializeAsync(IDictionary<string, string> arguments,
            NavigationMode mode = NavigationMode.Refresh)
        {
            await Task.CompletedTask;
        }
    }

    public abstract partial class InfrastructureViewModel : IPaneViewModel
    {
        private bool _isPaneOpen;

        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set => Set(ref _isPaneOpen, value, nameof(IsPaneOpen));
        }

        public Frame PaneFrame { get; private set; }

        public async Task InitializeAsync(SplitView view)
        {
            PaneFrame = view.Pane as Frame;
            await Task.CompletedTask;
        }
    }
}
