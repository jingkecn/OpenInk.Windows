using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using MyScript.InteractiveInk.Annotations;

namespace MyScript.OpenInk.Core.Infrastructure.ViewModels
{
    public interface IPaneViewModel
    {
        bool IsPaneOpen { get; }
        [CanBeNull] Frame PaneFrame { get; }

        Task InitializeAsync([NotNull] SplitView view);
    }
}
