using System;
using System.Numerics;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.Core.Preview;
using Windows.UI.ViewManagement;
using MyScript.InteractiveInk.Annotations;
using MyScript.OpenInk.Core.Infrastructure.Patterns;
using MyScript.OpenInk.Core.Infrastructure.Services;
using MyScript.OpenInk.Main.Configuration;
using MyScript.OpenInk.UI.Infrastructure.Extensions;

namespace MyScript.OpenInk.Main.Infrastructure.Services
{
    public partial class ContextService : Disposable
    {
        protected override void ReleaseManagedResources()
        {
            ApplicationView.GetForCurrentView().Consolidated -= OnConsolidated;
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested -= OnCloseRequested;
        }

        public override string ToString()
        {
            return $"{nameof(ContextService)}: (" +
                   $"{nameof(Id)} = {Id}; " +
                   $"{nameof(IsMain)} = {IsMain}; " +
                   $"{nameof(Dpi)} = {Dpi})";
        }
    }

    public partial class ContextService : IContextService
    {
        [NotNull] private CoreDispatcher Dispatcher { get; set; }

        public event EventHandler<BackRequestedEventArgs> BackRequested;
        public event EventHandler<SystemNavigationCloseRequestedPreviewEventArgs> CloseRequested;
        public event TypedEventHandler<ApplicationView, ApplicationViewConsolidatedEventArgs> Consolidated;

        public void Initialize(int id, bool isMain, CoreDispatcher dispatcher)
        {
            Id = id;
            IsMain = isMain;
            Dispatcher = dispatcher;
            Dpi = DisplayInformation.GetForCurrentView().GetDpi();
            ApplicationView.GetForCurrentView().Consolidated += OnConsolidated;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += OnCloseRequested;
        }

        public IAsyncAction RunAsync(DispatchedHandler action)
        {
            return Dispatcher?.RunAsync(CoreDispatcherPriority.Normal, action);
        }

        public Vector2 Dpi { get; set; }

        public int Id { get; private set; }
        public bool IsMain { get; private set; }

        private void OnCloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            CloseRequested?.Invoke(sender, e);
        }

        protected virtual void OnBackRequested(object sender, BackRequestedEventArgs args)
        {
            BackRequested?.Invoke(sender, args);
        }

        protected virtual void OnConsolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            using var deferral = new Deferral(() =>
            {
                ServiceLocator.Current.Dispose();
                sender.Consolidated -= OnConsolidated;
            });
            Consolidated?.Invoke(sender, args);
            deferral.Complete();
        }
    }
}
