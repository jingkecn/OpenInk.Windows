using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Controls;
using MyScript.InteractiveInk.Annotations;
using MyScript.OpenInk.Main.Views.Book;
using MyScript.OpenInk.Main.Views.Page;
using MyScript.OpenInk.Main.Views.Settings;
using MyScript.OpenInk.UI.Infrastructure.Extensions;

namespace MyScript.OpenInk.Main.ViewModels
{
    public partial class MainViewModel
    {
        private NavigationViewItem _selectedItem;

        public NavigationViewItem SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value, nameof(SelectedItem));
        }
    }

    public partial class MainViewModel
    {
        [CanBeNull] private IEnumerable<NavigationViewItem> Items { get; set; }
        [CanBeNull] private NavigationViewItem SettingsItem { get; set; }

        public void Initialize([NotNull] IDictionary<string, string> arguments)
        {
            if (!arguments.TryGetValue("view", out var view))
            {
                return;
            }

            switch (view)
            {
                case "settings":
                    InfrastructureServices.NavigationService.Navigate<SettingsPage>(arguments,
                        new SuppressNavigationTransitionInfo());
                    break;
            }
        }

        public void Initialize([NotNull] IEnumerable<NavigationViewItem> items)
        {
            Items = items;
            Items.FirstOrDefault()?.Navigate(InfrastructureServices.NavigationService);
        }

        public void Initialize([NotNull] NavigationViewItem settingsItem)
        {
            SettingsItem = settingsItem;
        }
    }

    public partial class MainViewModel : InteractiveInkViewModel
    {
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            InfrastructureServices.NavigationService.Navigated += OnNavigated;
        }

        protected override void ReleaseManagedResources()
        {
            base.ReleaseManagedResources();
            InfrastructureServices.NavigationService.Navigated -= OnNavigated;
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            var page = e.SourcePageType == typeof(PageViewPage) ? typeof(BookViewPage) : e.SourcePageType;
            SelectedItem = Items?.SingleOrDefault(item => item.GetNavigation() == page) ??
                           (page == typeof(SettingsPage) ? SettingsItem : SelectedItem);
        }
    }
}
