using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.ViewManagement;
using MyScript.InteractiveInk.Annotations;
using MyScript.OpenInk.Main.Infrastructure.Activation;
using MyScript.OpenInk.Main.Infrastructure.Services;
using MyScript.OpenInk.Main.Infrastructure.ViewModels;
using static Windows.Storage.AccessCache.StorageApplicationPermissions;

namespace MyScript.OpenInk.Main.ViewModels
{
    public partial class RecentViewModel
    {
        public ObservableCollection<StorageFile> Files { get; } = new();
    }

    public partial class RecentViewModel : InfrastructureViewModel
    {
        private static ApplicationViewSwitchingOptions SwitchingOptions =>
            UIViewSettings.GetForCurrentView().UserInteractionMode switch
            {
                UserInteractionMode.Mouse => ApplicationViewSwitchingOptions.ConsolidateViews,
                UserInteractionMode.Touch => default,
                _ => throw new ArgumentOutOfRangeException()
            };

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            foreach (var entry in MostRecentlyUsedList.Entries)
            {
                await InitializeAsync(entry);
            }
        }

        private async Task InitializeAsync(AccessListEntry entry)
        {
            try
            {
                Files.Add(await MostRecentlyUsedList.GetFileAsync(entry.Token,
                    AccessCacheOptions.SuppressAccessTimeUpdate));
            }
            catch
            {
                MostRecentlyUsedList.Remove(entry.Token);
            }
        }

        public async Task OpenAsync([NotNull] StorageFile file)
        {
            if (!(ActivationService.FileActivationHandler is FileActivationHandler handler))
            {
                return;
            }

            await handler.HandleFileAsync(file, SwitchingOptions);
        }
    }
}
