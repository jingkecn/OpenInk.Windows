using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MyScript.OpenInk.Core.Infrastructure.Extensions;
using MyScript.OpenInk.Core.Models;

namespace MyScript.OpenInk.Main.ViewModels
{
    public partial class LanguageCollectionViewModel
    {
        public ObservableCollection<ILanguage> Languages { get; } = new();
    }

    public partial class LanguageCollectionViewModel : InteractiveInkViewModel
    {
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            Languages.SyncWith(InteractiveInkServices.LanguageService.Languages);
            Languages.SortBy(x => x.Id);
        }
    }
}
