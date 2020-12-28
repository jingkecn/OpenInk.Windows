using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using MyScript.InteractiveInk.Annotations;
using MyScript.OpenInk.Core.Infrastructure.Services;

namespace MyScript.OpenInk.UI.Infrastructure.Extensions
{
    public static partial class NavigationExtensions
    {
        public static void Navigate([NotNull] this UIElement source, [NotNull] INavigationService service,
            [CanBeNull] object parameter = null, [CanBeNull] NavigationTransitionInfo info = null)
        {
            service.Navigate(source.GetNavigation(), parameter ?? source.GetNavigationParameter(), info);
        }
    }

    public static partial class NavigationExtensions
    {
        public static readonly DependencyProperty NavigationProperty = DependencyProperty.Register("Navigation",
            typeof(Type), typeof(UIElement), new PropertyMetadata(default(Type)));

        [NotNull]
        public static Type GetNavigation([NotNull] this UIElement source)
        {
            return (Type)source.GetValue(NavigationProperty);
        }

        public static void SetNavigation([NotNull] this UIElement source, [NotNull] Type value)
        {
            source.SetValue(NavigationProperty, value);
        }
    }

    public static partial class NavigationExtensions
    {
        public static readonly DependencyProperty NavigationParameterProperty =
            DependencyProperty.Register("NavigationParameter",
                typeof(object), typeof(UIElement), new PropertyMetadata(default));

        [CanBeNull]
        public static object GetNavigationParameter([NotNull] this UIElement source)
        {
            return source.GetValue(NavigationParameterProperty);
        }

        public static void SetNavigationParameter([NotNull] this UIElement source, [CanBeNull] object value)
        {
            source.SetValue(NavigationParameterProperty, value);
        }
    }
}
