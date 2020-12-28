using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using MyScript.InteractiveInk.Annotations;
using MyScript.OpenInk.Core.Models;

namespace MyScript.OpenInk.Core.Services
{
    public partial interface ILanguageService
    {
        IEnumerable<ILanguage> Languages { get; }

        Task<Deferral> InitializeAsync([NotNull] Uri uri, bool isActivatedFromNotification = false);
    }

    public partial interface ILanguageService
    {
        bool CanCancel(ILanguage language);
        bool CanInstall(ILanguage language);
        bool CanPause(ILanguage language);
        bool CanResume(ILanguage language);
        bool CanUninstall(ILanguage language);
        bool CanUpdate(ILanguage language);
    }

    public partial interface ILanguageService
    {
        bool IsInstalling(ILanguage language);
        bool IsInstalled(ILanguage language);
        bool IsUpToDate(ILanguage language);
    }

    public partial interface ILanguageService
    {
        Task CancelAsync(ILanguage language);
        Task InstallAsync(ILanguage language, BackgroundTransferPriority priority = default);
        Task PauseAsync(ILanguage language);
        Task ResumeAsync(ILanguage language);
        Task UninstallAsync(ILanguage language);
    }

    public partial interface ILanguageService
    {
        BackgroundDownloadProgress GetDownloadProgress(ILanguage language);
        Task HandleDownLoadAsync([NotNull] DownloadOperation download, bool start = true);
        Task HandleResultFileAsync([NotNull] IStorageFile file);
    }

    public partial interface ILanguageService
    {
        Task NotifyUpdateAsync(ILanguage language);
        Task NotifyResumeAsync(ILanguage language);
    }

    public partial interface ILanguageService
    {
        event TypedEventHandler<ILanguage, BackgroundDownloadProgress> Downloading;
        event TypedEventHandler<ILanguage, BackgroundDownloadProgress> Initialized;
        event TypedEventHandler<ILanguage, BackgroundDownloadProgress> Initializing;
    }
}
