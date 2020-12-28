using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;
using MyScript.InteractiveInk.Annotations;
using MyScript.OpenInk.Core.Models;

namespace MyScript.OpenInk.Core.ViewModels
{
    public interface IBookViewModel
    {
        IBook Book { get; }
        Collection<IPage> Pages { get; }
        Task InitializeAsync(IBook book, NavigationMode mode = NavigationMode.Refresh);
        Task InitializeAsync([NotNull] StorageFile file, NavigationMode mode = NavigationMode.Refresh);
        Task InitializeAsync([NotNull] string path, NavigationMode mode = NavigationMode.Refresh);
    }
}
