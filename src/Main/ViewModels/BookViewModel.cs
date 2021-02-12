using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core.Preview;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Navigation;
using MyScript.Certificate;
using MyScript.IInk;
using MyScript.OpenInk.Core.Infrastructure.Extensions;
using MyScript.OpenInk.Core.Models;
using MyScript.OpenInk.Core.ViewModels;
using MyScript.OpenInk.Main.Extensions;

namespace MyScript.OpenInk.Main.ViewModels
{
    public partial class BookViewModel : InteractiveInkViewModel
    {
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            InfrastructureServices.ContextService.CloseRequested += OnCloseRequested;
            InteractiveInkServices.EngineService.Initialize(MyCertificate.Bytes);
            InteractiveInkServices.EngineService.PackageContentChanged += OnPackageContentChanged;
            InteractiveInkServices.EngineService.PackageChanged += OnPackageChanged;
        }

        private async void OnCloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            var deferral = e.GetDeferral();
            e.Handled = await RequireSaveAsync();
            deferral.Complete();
        }

        public override async Task InitializeAsync(IDictionary<string, string> arguments,
            NavigationMode mode = NavigationMode.Refresh)
        {
            await base.InitializeAsync(arguments, mode);
            if (!arguments.TryGetValue("view", out var view) || view != "book" ||
                !arguments.TryGetValue("action", out var action))
            {
                return;
            }

            switch (action)
            {
                case "create":
                    if (!arguments.TryGetValue("language", out var id))
                    {
                        return;
                    }

                    var language = InteractiveInkServices.LanguageService.Languages.Single(x => x.Id == id);
                    await InteractiveInkServices.EngineService.InitializeAsync(language);
                    if (!arguments.TryGetValue("path", out var path))
                    {
                        return;
                    }

                    await InitializeAsync(path, mode);
                    break;
            }
        }

        private async void OnPackageContentChanged(ContentPackage sender,
            Tuple<IEnumerable<ContentPart>, IEnumerable<ContentPart>> args)
        {
            if (!(sender is { } package))
            {
                return;
            }

            var book = await package.ToPlatformAsync();
            await InitializeAsync(book);
            if (!(args is { } tuple))
            {
                if (Pages.Any())
                {
                    InteractiveInkCommands.PageCommands.CommandOpen.Execute(Pages.First());
                }

                return;
            }

            var (removed, added) = tuple;
            if ((removed?.ToList() is { } removedList && removedList.Any()) ||
                !(added?.ToList() is { } addedList) || !addedList.Any())
            {
                return;
            }

            InteractiveInkCommands.PageCommands.CommandOpen.Execute(addedList.Last().ToPlatform());
        }

        private async void OnPackageChanged(ContentPackage current, ContentPackage previous)
        {
            if (!(current is { } package))
            {
                return;
            }

            var book = await package.ToPlatformAsync();
            var mode = previous == null ? NavigationMode.New : NavigationMode.Refresh;
            await InitializeAsync(book, mode);
            if (mode != NavigationMode.New)
            {
                return;
            }

            if (!Pages.Any())
            {
                return;
            }

            OnPackageContentChanged(package, null);
        }

        protected override void ReleaseManagedResources()
        {
            InfrastructureServices.ContextService.CloseRequested -= OnCloseRequested;
            InteractiveInkServices.EngineService.PackageContentChanged -= OnPackageContentChanged;
            InteractiveInkServices.EngineService.PackageChanged -= OnPackageChanged;
        }
    }

    public partial class BookViewModel : IBookViewModel
    {
        private IBook _book;

        public IBook Book
        {
            get => _book;
            set => Set(ref _book, value, nameof(Book));
        }

        public Collection<IPage> Pages { get; } = new ObservableCollection<IPage>();

        public async Task InitializeAsync(IBook book, NavigationMode mode = NavigationMode.Refresh)
        {
            ApplicationView.GetForCurrentView().Title = book.Name;
            Book = book;
            Pages.SyncWith(Book.Pages);
            Pages.SortBy(page => page.Index);
            await Task.CompletedTask;
        }

        public async Task InitializeAsync(StorageFile file, NavigationMode mode = NavigationMode.Refresh)
        {
            if (mode == NavigationMode.New)
            {
                InteractiveInkCommands.BookCommands.CommandOpen.Execute(file);
            }

            if (!(InteractiveInkServices.EngineService.ContentPackage is { } package))
            {
                return;
            }

            await InitializeAsync(await package.ToPlatformAsync(), mode);
        }

        public async Task InitializeAsync(string path, NavigationMode mode = NavigationMode.Refresh)
        {
            if (mode == NavigationMode.New)
            {
                InteractiveInkCommands.BookCommands.CommandCreate.Execute(path);
            }

            if (!(InteractiveInkServices.EngineService.ContentPackage is { } package))
            {
                return;
            }

            await InitializeAsync(await package.ToPlatformAsync(), mode);
        }
    }
}
