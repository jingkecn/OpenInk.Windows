using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;

namespace MyScript.OpenInk.Main.Views.Settings
{
    public sealed partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.Dispose();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await ViewModel.InitializeAsync();
            await ViewModel.InitializeAsync(SplitView);
            if (!(e.Parameter is IDictionary<string, string> arguments))
            {
                return;
            }

            await ViewModel.InitializeAsync(arguments, e.NavigationMode);
        }
    }
}
