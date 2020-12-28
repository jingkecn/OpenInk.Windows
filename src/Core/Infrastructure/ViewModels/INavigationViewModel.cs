using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MyScript.InteractiveInk.Annotations;

namespace MyScript.OpenInk.Core.Infrastructure.ViewModels
{
    public interface INavigationViewModel
    {
        bool CanGoBack { get; }

        Task InitializeAsync([NotNull] Frame frame);

        Task InitializeAsync([NotNull] IDictionary<string, string> arguments,
            NavigationMode mode = NavigationMode.Refresh);
    }
}
