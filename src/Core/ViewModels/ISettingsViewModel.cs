using System.Collections.ObjectModel;
using System.Windows.Input;
using MyScript.OpenInk.Core.Models;

namespace MyScript.OpenInk.Core.ViewModels
{
    public interface ISettingsViewModel
    {
        ICommand CommandManageLanguages { get; }
        ObservableCollection<ILanguage> InstalledLanguages { get; }
        string Version { get; }
    }
}
