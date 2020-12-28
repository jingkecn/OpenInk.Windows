using System;
using System.Collections.Immutable;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using MyScript.OpenInk.Core.Infrastructure.Commands;
using MyScript.OpenInk.Core.Infrastructure.Services;

namespace MyScript.OpenInk.Main.Infrastructure.Services
{
    public class ThemeService : IThemeService
    {
        private const string SettingsThemeKey = "Theme";
        private ICommand _commandApplyTheme;

        private static ApplicationDataContainer Settings => ApplicationData.Current.LocalSettings;

        public ICommand CommandApplyTheme =>
            _commandApplyTheme ??= new RelayCommand<ElementTheme>(theme => Theme = theme);

        public ElementTheme Theme
        {
            get => Settings.Values[SettingsThemeKey] is string name && Enum.TryParse<ElementTheme>(name, out var theme)
                ? theme
                : ElementTheme.Default;
            set
            {
                CoreApplication.Views.ToImmutableList().ForEach(async view =>
                {
                    await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if (!(Window.Current.Content is FrameworkElement element))
                        {
                            return;
                        }

                        element.RequestedTheme = value;
                    });
                });
                Settings.Values[SettingsThemeKey] = value.ToString();
            }
        }
    }
}
