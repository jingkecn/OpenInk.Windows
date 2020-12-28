using System.Windows.Input;

namespace MyScript.OpenInk.Core.ViewModels
{
    public interface IDashboardViewModel
    {
        ICommand CommandCreateBook { get; }
        ICommand CommandOpenBook { get; }
    }
}
