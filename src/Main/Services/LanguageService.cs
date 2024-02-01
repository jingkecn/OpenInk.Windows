using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.Toolkit.Uwp.Notifications;
using MyScript.OpenInk.Core.Infrastructure.Extensions;
using MyScript.OpenInk.Core.Infrastructure.Patterns;
using MyScript.OpenInk.Core.Infrastructure.Services;
using MyScript.OpenInk.Core.Models;
using MyScript.OpenInk.Core.Services;
using MyScript.OpenInk.Main.Configuration;
using MyScript.OpenInk.Main.Models;
using MyScript.OpenInk.Main.Views.Settings;

namespace MyScript.OpenInk.Main.Services
{
    public partial class LanguageService
    {
        private ConcurrentDictionary<ILanguage, DownloadOperation> ActiveDownloads { get; } = new();
        private ConcurrentDictionary<ILanguage, CancellationTokenSource> CancellationTokenSources { get; } = new();
        private static bool IsNetWorkAvailable => NetworkInterface.GetIsNetworkAvailable();

        private void OnProgress(DownloadOperation download)
        {
            var progress = download.Progress;
            var received = progress.BytesReceived;
            var total = progress.TotalBytesToReceive;
            if (total is not 0)
            {
                var percent = received * 100 / total;
                var status = progress.Status;
                Debug.WriteLine($"{nameof(LanguageService)}.{nameof(OnProgress)}:" +
                                $"\n\tprogress: ({percent}%) {received} / {total}" +
                                $"\n\tstatus: {status}" +
                                $"\n\turl: {download.RequestedUri}");
            }

            var language = Languages.Single(x => x.Url == download.RequestedUri.ToString());
            OnDownloading(language, progress);
        }
    }

    public partial class LanguageService : Disposable
    {
        protected override void ReleaseManagedResources()
        {
            CancellationTokenSources.Values.DisposeAll();
        }
    }

    public partial class LanguageService : ILanguageService
    {
        public event TypedEventHandler<ILanguage, BackgroundDownloadProgress> Downloading;
        public event TypedEventHandler<ILanguage, BackgroundDownloadProgress> Initialized;
        public event TypedEventHandler<ILanguage, BackgroundDownloadProgress> Initializing;

        public async Task NotifyUpdateAsync(ILanguage language)
        {
            if (!CanUpdate(language) || ActiveDownloads.ContainsKey(language))
            {
                return;
            }

            var builder = new ToastContentBuilder()
                .AddText(language.PlatformModel.NativeName)
                .AddText("MessageLanguageUpdate".Localize(language.PlatformModel.DisplayName))
                .AddToastActivationInfo($"view=settings&action=installLanguage&language={language.Id}",
                    ToastActivationType.Foreground)
                .SetToastScenario(ToastScenario.Reminder);
            var notification = new ToastNotification(builder.Content.GetXml())
            {
                Group = language.Id, Tag = language.Checksum
            };
            ToastNotificationManager.CreateToastNotifier().Show(notification);
            await Task.CompletedTask;
        }

        public async Task NotifyResumeAsync(ILanguage language)
        {
            if (!CanResume(language))
            {
                return;
            }

            var builder = new ToastContentBuilder()
                .AddText(language.PlatformModel.NativeName)
                .AddText("MessageLanguageResume".Localize(language.PlatformModel.DisplayName))
                .AddToastActivationInfo($"view=settings&action=resumeLanguage&language={language.Id}",
                    ToastActivationType.Foreground)
                .SetToastScenario(ToastScenario.Reminder);
            var notification = new ToastNotification(builder.Content.GetXml())
            {
                Group = language.Id, Tag = language.Checksum
            };
            ToastNotificationManager.CreateToastNotifier().Show(notification);
            await Task.CompletedTask;
        }

        public BackgroundDownloadProgress GetDownloadProgress(ILanguage language)
        {
            return ActiveDownloads.TryGetValue(language, out var download) ? download.Progress : default;
        }

        public async Task HandleDownLoadAsync(DownloadOperation download, bool start = true)
        {
            var language = Languages.Single(x => x.Url == download.RequestedUri.ToString());
            try
            {
                OnInitializing(language, download.Progress);
                var progress = new Progress<DownloadOperation>(OnProgress);
                var token = CancellationTokenSources.GetOrAdd(language, _ => new CancellationTokenSource()).Token;
                if (start)
                {
                    await download.StartAsync().AsTask(token, progress);
                }
                else
                {
                    await download.AttachAsync().AsTask(token, progress);
                }

                if (download.GetResponseInformation() is not { } response)
                {
                    return;
                }

                Debug.WriteLine($"{nameof(ILanguageService)}.{nameof(HandleDownLoadAsync)}:" +
                                $"\n\tdownload: {download.Guid}" +
                                $"\n\tstatus: {response.StatusCode}");
                await HandleResultFileAsync(download.ResultFile);
            }
            catch (TaskCanceledException exception)
            {
                Debug.WriteLine(exception);
                CancellationTokenSources.TryRemove(language, out _);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                ActiveDownloads.TryRemove(language, out download);
                OnInitialized(language, download.Progress);
            }
        }

        public async Task HandleResultFileAsync(IStorageFile file)
        {
            // Retrieve the associated language.
            var language = Languages.Single(x => file.Name == $"{x.Checksum}.zip");
            // TODO: validate result file with SHA256 checksum.
            var localFolder = ApplicationData.Current.LocalFolder;
            var assetsFolder = await localFolder.CreateFolderAsync("Assets", CreationCollisionOption.OpenIfExists);
            var temporaryFolder = ApplicationData.Current.TemporaryFolder;
            var destinationFolder =
                await temporaryFolder.CreateFolderAsync(language.Checksum, CreationCollisionOption.ReplaceExisting);
            try
            {
                OnInitializing(language, GetDownloadProgress(language));
                // Unzip the result file.
                ZipFile.ExtractToDirectory(file.Path, destinationFolder.Path, true);
                // Rename the info file.
                var languageInfoFolder = await destinationFolder.GetFolderAsync("INFO");
                var languageInfoFile =
                    await languageInfoFolder.GetFileAsync($"myscript-iink-recognition-text-{language.Tag}.version.txt");
                await languageInfoFile.RenameAsync($"{language.Checksum}.txt", NameCollisionOption.ReplaceExisting);
                // Copy extracted files to the local assets folder.
                await languageInfoFolder.CopyAsync(assetsFolder);
                var languageAssetsFolder = await destinationFolder.GetFolderAsync("recognition-assets");
                var languageConfigurationFolder = await languageAssetsFolder.GetFolderAsync("conf");
                await languageConfigurationFolder.CopyAsync(assetsFolder);
                var languageResourcesFolder = await languageAssetsFolder.GetFolderAsync("resources");
                await languageResourcesFolder.CopyAsync(assetsFolder);
            }
            finally
            {
                await destinationFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                OnInitialized(language, GetDownloadProgress(language));
            }
        }

        public async Task CancelAsync(ILanguage language)
        {
            if (!CanCancel(language) || !CancellationTokenSources.TryGetValue(language, out var source))
            {
                return;
            }

            source.Cancel();
            source.Dispose();
            await Task.CompletedTask;
        }

        public bool IsInstalling(ILanguage language)
        {
            return ActiveDownloads.ContainsKey(language);
        }

        public bool IsInstalled(ILanguage language)
        {
            return File.Exists(language.LocalConfigurationFilePath) &&
                   Directory.Exists(language.LocalResourcesFolderPath);
        }

        public bool IsUpToDate(ILanguage language)
        {
            return IsInstalled(language) && File.Exists(language.LocalInfoFilePath);
        }

        public bool CanCancel(ILanguage language)
        {
            return ActiveDownloads.ContainsKey(language);
        }

        public bool CanInstall(ILanguage language)
        {
            return !ActiveDownloads.ContainsKey(language) && (!IsInstalled(language) || CanUpdate(language)) &&
                   IsNetWorkAvailable;
        }

        public bool CanPause(ILanguage language)
        {
            return ActiveDownloads.TryGetValue(language, out var download) && download?.Progress is { } progress &&
                   progress.Status.IsRunning();
        }

        public bool CanResume(ILanguage language)
        {
            return ActiveDownloads.TryGetValue(language, out var download) && download?.Progress is { } progress &&
                   progress.Status.IsPaused(true) && IsNetWorkAvailable;
        }

        public bool CanUninstall(ILanguage language)
        {
            return !ActiveDownloads.ContainsKey(language) && !language.IsBundled && IsInstalled(language);
        }

        public bool CanUpdate(ILanguage language)
        {
            return IsInstalled(language) && !IsUpToDate(language);
        }

        public async Task InstallAsync(ILanguage language, BackgroundTransferPriority priority = default)
        {
            if (!CanInstall(language))
            {
                return;
            }

            if (!Uri.TryCreate(language.Url, UriKind.Absolute, out var url))
            {
                throw new UriFormatException(language.Url);
            }

            try
            {
                OnInitializing(language, GetDownloadProgress(language));
                var temporaryFolder = ApplicationData.Current.TemporaryFolder;
                var destinationFile = await temporaryFolder.CreateFileAsync($"{language.Checksum}.zip",
                    CreationCollisionOption.ReplaceExisting);
                var download = new BackgroundDownloader().CreateDownload(url, destinationFile);
                download.Priority = priority;
                Debug.WriteLine($"{nameof(ILanguageService)}.{nameof(InstallAsync)}:" +
                                $"\n\tsource: {download.RequestedUri}" +
                                $"\n\tdestination: {destinationFile.Path}" +
                                $"\n\tpriority: {priority}");
                ActiveDownloads.AddOrUpdate(language, _ => download, (_, __) => download);
                await HandleDownLoadAsync(download);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                OnInitialized(language, GetDownloadProgress(language));
            }
        }

        public async Task PauseAsync(ILanguage language)
        {
            if (!CanPause(language) || !ActiveDownloads.TryGetValue(language, out var download))
            {
                return;
            }

            try
            {
                download.Pause();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }

            await Task.CompletedTask;
        }

        public async Task ResumeAsync(ILanguage language)
        {
            if (!CanResume(language) || !ActiveDownloads.TryGetValue(language, out var download))
            {
                return;
            }

            try
            {
                download.Resume();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }

            await Task.CompletedTask;
        }

        public async Task UninstallAsync(ILanguage language)
        {
            if (!CanUninstall(language))
            {
                return;
            }

            OnInitializing(language, GetDownloadProgress(language));
            if (File.Exists(language.LocalConfigurationFilePath))
            {
                var configurationFile = await StorageFile.GetFileFromPathAsync(language.LocalConfigurationFilePath);
                await configurationFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }

            if (File.Exists(language.LocalInfoFilePath))
            {
                var infoFile = await StorageFile.GetFileFromPathAsync(language.LocalInfoFilePath);
                await infoFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }

            if (Directory.Exists(language.LocalResourcesFolderPath))
            {
                var resourceFolder = await StorageFolder.GetFolderFromPathAsync(language.LocalResourcesFolderPath);
                await resourceFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }

            OnInitialized(language, GetDownloadProgress(language));
        }

        public IEnumerable<ILanguage> Languages { get; private set; }

        public async Task<Deferral> InitializeAsync(Uri uri, bool isActivatedFromNotification = false)
        {
            Debug.WriteLine(
                $"{nameof(ILanguageService)}.{nameof(InitializeAsync)}({uri}, {isActivatedFromNotification}):");
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            using var stream = await file.OpenStreamForReadAsync();
            using var document = await JsonDocument.ParseAsync(stream);
            var deserialized = document.RootElement;
            var languages = new List<Language>();
            foreach (var element in deserialized.GetProperty("languages").EnumerateArray())
            {
                if (element.ToString() is not { } json)
                {
                    continue;
                }

                languages.Add(JsonSerializer.Deserialize<Language>(json));
            }

            Languages = languages.OfType<ILanguage>().OrderBy(x => x.Id);
            languages.ForEach(language => OnInitializing(language, GetDownloadProgress(language)));
            var downloads = await BackgroundDownloader.GetCurrentDownloadsAsync();
            var tasks = new List<Task>();
            foreach (var download in downloads)
            {
                if (languages.All(x => x.Url != download.RequestedUri.ToString()))
                {
                    continue;
                }

                Debug.WriteLine($"\tdiscovered: {download.Guid} ({download.Progress.Status})");
                var language = languages.SingleOrDefault(x => x.Url == download.RequestedUri.ToString());
                ActiveDownloads.AddOrUpdate(language, _ => download, (_, __) => download);
                tasks.Add(HandleDownLoadAsync(download, false));
            }

            languages.ForEach(language => OnInitialized(language, GetDownloadProgress(language)));
            if (isActivatedFromNotification)
            {
                return new Deferral(async () => await Task.WhenAll(tasks));
            }

            languages.ForEach(async language =>
            {
                await NotifyResumeAsync(language);
                await NotifyUpdateAsync(language);
            });

            return new Deferral(async () =>
            {
                if (ActiveDownloads.Any())
                {
                    var navigationService = ServiceLocator.Current.Get<INavigationService>();
                    navigationService.Navigate<SettingsPage>(info: new SuppressNavigationTransitionInfo());
                }

                await Task.WhenAll(tasks);
            });
        }

        protected virtual void OnDownloading(ILanguage sender, BackgroundDownloadProgress args)
        {
            Downloading?.Invoke(sender, args);
        }

        protected virtual void OnInitialized(ILanguage sender, BackgroundDownloadProgress args)
        {
            Initialized?.Invoke(sender, args);
        }

        protected virtual void OnInitializing(ILanguage sender, BackgroundDownloadProgress args)
        {
            Initializing?.Invoke(sender, args);
        }
    }
}
