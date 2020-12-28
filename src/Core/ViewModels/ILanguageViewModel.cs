using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using MyScript.OpenInk.Core.Models;

namespace MyScript.OpenInk.Core.ViewModels
{
    public partial interface ILanguageViewModel
    {
        ILanguage Language { get; }

        Task InitializeAsync(ILanguage language);
    }

    public partial interface ILanguageViewModel
    {
        bool CanUninstall { get; }
        bool CanUpdate { get; }
        bool IsInstalled { get; }
        bool IsUpToDate { get; }
    }

    public partial interface ILanguageViewModel
    {
        bool IsIndeterminate { get; }
        bool IsInstalling { get; }
        bool IsNetworkAvailable { get; }
        bool IsTransferPaused { get; }
    }

    public partial interface ILanguageViewModel
    {
        BackgroundDownloadProgress DownloadProgress { get; }
        double ReceivedSize { get; }
        double TotalSize { get; }

        Task InitializeAsync(BackgroundDownloadProgress progress, bool isIndeterminate = true);
    }
}
