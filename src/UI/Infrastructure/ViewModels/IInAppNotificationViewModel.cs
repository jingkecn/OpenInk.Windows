using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Controls;
using MyScript.InteractiveInk.Annotations;

namespace MyScript.OpenInk.UI.Infrastructure.ViewModels
{
    public interface IInAppNotificationViewModel
    {
        [CanBeNull] InAppNotification InAppNotification { get; }

        Task InitializeAsync([NotNull] InAppNotification notifier);
    }
}
