using MyScript.InteractiveInk.Annotations;

namespace MyScript.OpenInk.UI.Infrastructure.ViewModels
{
    public interface IInfoBarViewModel
    {
        bool IsInfoBarOpen { get; set; }
        [CanBeNull] string InfoMessage { get; set; }
        [CanBeNull] string InfoTitle { get; set; }
    }
}
