using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Networking.Connectivity;
using MyScript.OpenInk.Core.Infrastructure.Extensions;
using MyScript.OpenInk.Core.Models;
using MyScript.OpenInk.Core.Services;
using MyScript.OpenInk.Core.ViewModels;

namespace MyScript.OpenInk.Main.ViewModels
{
    public partial class LanguageViewModel : InteractiveInkViewModel
    {
        private ILanguageService LanguageService => InteractiveInkServices.LanguageService;

        protected override void ReleaseManagedResources()
        {
            LanguageService.Downloading -= OnDownloading;
            LanguageService.Initialized -= OnInitialized;
            LanguageService.Initializing -= OnInitializing;
            NetworkInformation.NetworkStatusChanged -= OnNetworkStatusChanged;
            base.ReleaseManagedResources();
        }

        private async void OnDownloading(ILanguage sender, BackgroundDownloadProgress args)
        {
            if (sender.Id != Language.Id)
            {
                return;
            }

            await InitializeAsync(args, false);
        }

        private async void OnInitialized(ILanguage sender, BackgroundDownloadProgress args)
        {
            if (sender.Id != Language.Id)
            {
                return;
            }

            await InitializeAsync(args, !args.Status.IsPaused());
        }

        private async void OnInitializing(ILanguage sender, BackgroundDownloadProgress args)
        {
            if (sender.Id != Language.Id)
            {
                return;
            }

            await InitializeAsync(args);
        }

        private async void OnNetworkStatusChanged(object sender)
        {
            await InfrastructureServices.ContextService.RunAsync(() =>
                IsNetworkAvailable = NetworkInterface.GetIsNetworkAvailable());
        }
    }

    public partial class LanguageViewModel : ILanguageViewModel
    {
        private bool _canUninstall;
        private bool _canUpdate;
        private BackgroundDownloadProgress _downloadProgress;
        private bool _isIndeterminate;
        private bool _isInstalled;
        private bool _isInstalling;
        private bool _isNetworkAvailable;
        private bool _isTransferPaused;
        private bool _isUpToDate;
        private ILanguage _language;
        private double _receivedSize;
        private double _totalSize;

        public BackgroundDownloadProgress DownloadProgress
        {
            get => _downloadProgress;
            set => Set(ref _downloadProgress, value, nameof(DownloadProgress));
        }

        public double ReceivedSize
        {
            get => _receivedSize;
            set => Set(ref _receivedSize, value, nameof(ReceivedSize));
        }

        public double TotalSize
        {
            get => _totalSize;
            set => Set(ref _totalSize, value, nameof(TotalSize));
        }

        public async Task InitializeAsync(BackgroundDownloadProgress progress, bool isIndeterminate = true)
        {
            IsIndeterminate = isIndeterminate;
            IsInstalling = LanguageService.IsInstalling(Language);
            // Status
            CanUninstall = LanguageService.CanUninstall(Language);
            CanUpdate = LanguageService.CanUpdate(Language);
            IsInstalled = LanguageService.IsInstalled(Language);
            IsNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
            IsTransferPaused = progress.Status.IsPaused();
            IsUpToDate = LanguageService.IsUpToDate(Language);
            // Progress
            DownloadProgress = progress;
            ReceivedSize = Convert.ToDouble(progress.BytesReceived);
            TotalSize = Convert.ToDouble(progress.TotalBytesToReceive);
            await Task.CompletedTask;
        }

        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            set => Set(ref _isIndeterminate, value, nameof(IsIndeterminate));
        }

        public bool IsInstalling
        {
            get => _isInstalling;
            set => Set(ref _isInstalling, value, nameof(IsInstalling));
        }

        public bool IsNetworkAvailable
        {
            get => _isNetworkAvailable;
            set => Set(ref _isNetworkAvailable, value, nameof(IsNetworkAvailable));
        }

        public bool IsTransferPaused
        {
            get => _isTransferPaused;
            set => Set(ref _isTransferPaused, value, nameof(IsTransferPaused));
        }

        public bool CanUninstall
        {
            get => _canUninstall;
            set => Set(ref _canUninstall, value, nameof(CanUninstall));
        }

        public bool CanUpdate
        {
            get => _canUpdate;
            set => Set(ref _canUpdate, value, nameof(CanUpdate));
        }

        public bool IsInstalled
        {
            get => _isInstalled;
            set => Set(ref _isInstalled, value, nameof(IsInstalled));
        }

        public bool IsUpToDate
        {
            get => _isUpToDate;
            set => Set(ref _isUpToDate, value, nameof(IsUpToDate));
        }

        public ILanguage Language
        {
            get => _language;
            set => Set(ref _language, value, nameof(Language));
        }

        public async Task InitializeAsync(ILanguage language)
        {
            Language = language;
            var progress = LanguageService.GetDownloadProgress(language);
            await InitializeAsync(progress, !progress.Status.IsPaused());
            LanguageService.Downloading += OnDownloading;
            LanguageService.Initialized += OnInitialized;
            LanguageService.Initializing += OnInitializing;
            NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
            await Task.CompletedTask;
        }
    }
}
