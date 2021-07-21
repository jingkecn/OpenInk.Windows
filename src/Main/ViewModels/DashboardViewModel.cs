using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using MyScript.InteractiveInk.Constants;
using MyScript.OpenInk.Core.Infrastructure.Commands;
using MyScript.OpenInk.Core.Infrastructure.Extensions;
using MyScript.OpenInk.Core.Models;
using MyScript.OpenInk.Core.ViewModels;
using MyScript.OpenInk.Main.Views.Book;
using MyScript.OpenInk.Main.Views.Dashboard.Dialogs;
using MyScript.OpenInk.Main.Views.Settings;

namespace MyScript.OpenInk.Main.ViewModels
{
    public partial class DashboardViewModel : InteractiveInkViewModel
    {
        private ILanguage _selectedLanguage;
        public ObservableCollection<ILanguage> InstalledLanguages { get; } = new();

        public ILanguage SelectedLanguage
        {
            get => _selectedLanguage;
            set => Set(ref _selectedLanguage, value, nameof(SelectedLanguage));
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            InstalledLanguages.CollectionChanged += OnInstalledLanguagesChanged;
        }

        protected override void ReleaseManagedResources()
        {
            InstalledLanguages.CollectionChanged -= OnInstalledLanguagesChanged;
            base.ReleaseManagedResources();
        }

        private void OnInstalledLanguagesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SelectedLanguage ??= InstalledLanguages.FirstOrDefault();
        }
    }

    public partial class DashboardViewModel : IDashboardViewModel
    {
        private ICommand _commandCreateBook;
        private ICommand _commandOpenBook;

        private static ApplicationViewSwitchingOptions SwitchingOptions =>
            UIViewSettings.GetForCurrentView().UserInteractionMode switch
            {
                UserInteractionMode.Mouse => ApplicationViewSwitchingOptions.ConsolidateViews,
                UserInteractionMode.Touch => default,
                _ => throw new ArgumentOutOfRangeException()
            };

        public ICommand CommandCreateBook =>
            _commandCreateBook ??= new RelayCommand(async _ => await CreateBookAsync());

        public ICommand CommandOpenBook => _commandOpenBook ??= new RelayCommand(async _ => await OpenBookAsync());

        private async Task CreateBookAsync()
        {
            var result = await SelectLanguageAsync();
            switch (result)
            {
                case ContentDialogResult.None:
                    return;
                case ContentDialogResult.Primary:
                    break;
                case ContentDialogResult.Secondary:
                    InfrastructureServices.NavigationService.Navigate<SettingsPage>(
                        "view=settings&action=manageLanguages".ToArguments());
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (SelectedLanguage is not { } language)
            {
                return;
            }

            var folder = ApplicationData.Current.LocalCacheFolder;
            var name = $"{Path.GetRandomFileName()}{FileTypes.IInk}";
            var path = Path.Combine(folder.Path, name);
            await InfrastructureServices.NavigationService.ShowAsStandaloneAsync<BookViewPage>(
                $"view=book&action=create&language={language.Id}&path={path}".ToArguments(),
                options: SwitchingOptions);
        }

        private async Task OpenBookAsync()
        {
            var picker = new FileOpenPicker {SuggestedStartLocation = PickerLocationId.DocumentsLibrary};
            picker.FileTypeFilter.Add(FileTypes.IInk);
            //picker.FileTypeFilter.Add(FileTypes.Nebo);
            if (await picker.PickSingleFileAsync() is not { } file)
            {
                return;
            }

            await InfrastructureServices.NavigationService.ShowAsStandaloneAsync<BookViewPage>(file,
                options: SwitchingOptions);
        }

        private async Task<ContentDialogResult> SelectLanguageAsync()
        {
            var service = InteractiveInkServices.LanguageService;
            var languages = service.Languages.Where(x => service.IsInstalled(x) || service.IsInstalling(x));
            InstalledLanguages.SyncWith(languages);
            InstalledLanguages.SortBy(x => x.Id);
            InteractiveInkServices.LanguageService.Initialized += OnLanguageInitialized;
            var dialog = new LanguageSelector {DataContext = this};
            var result = await dialog.ShowAsync();
            InteractiveInkServices.LanguageService.Initialized -= OnLanguageInitialized;
            return result;
        }

        private void OnLanguageInitialized(ILanguage sender, BackgroundDownloadProgress args)
        {
            var service = InteractiveInkServices.LanguageService;
            if (!service.IsInstalled(sender))
            {
                InstalledLanguages.Remove(sender);
                return;
            }

            if (InstalledLanguages.Contains(sender))
            {
                return;
            }

            InstalledLanguages.Add(sender);
        }
    }
}
