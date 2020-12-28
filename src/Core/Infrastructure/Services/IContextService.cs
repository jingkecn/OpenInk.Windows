using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Core.Preview;
using Windows.UI.ViewManagement;
using MyScript.InteractiveInk.Annotations;

namespace MyScript.OpenInk.Core.Infrastructure.Services
{
    public partial interface IContextService
    {
        Vector2 Dpi { get; }
        int Id { get; }
        bool IsMain { get; }
    }

    public partial interface IContextService
    {
        void Initialize(int id, bool isMain, [NotNull] CoreDispatcher dispatcher);
        IAsyncAction RunAsync([NotNull] DispatchedHandler action);
    }

    public partial interface IContextService
    {
        /// <summary>
        ///     <inheritdoc cref="ISystemNavigationManager.BackRequested" />
        /// </summary>
        event EventHandler<BackRequestedEventArgs> BackRequested;

        /// <summary>
        ///     Occurs when the user invokes the system button for close (the 'x' button in the corner of the app's title
        ///     bar).
        /// </summary>
        event EventHandler<SystemNavigationCloseRequestedPreviewEventArgs> CloseRequested;

        /// <summary>
        ///     <inheritdoc cref="IApplicationView.Consolidated" />
        /// </summary>
        event TypedEventHandler<ApplicationView, ApplicationViewConsolidatedEventArgs> Consolidated;
    }
}
