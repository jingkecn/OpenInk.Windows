using System.Windows.Input;
using Windows.UI.Xaml;

namespace MyScript.OpenInk.Core.Infrastructure.Services
{
    public interface IThemeService
    {
        ICommand CommandApplyTheme { get; }
        ElementTheme Theme { get; set; }
    }
}
