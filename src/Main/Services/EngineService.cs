using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Microsoft.Win32.SafeHandles;
using MyScript.IInk;
using MyScript.InteractiveInk.Constants;
using MyScript.InteractiveInk.Enumerations;
using MyScript.InteractiveInk.Extensions;
using MyScript.InteractiveInk.UI.Implementations;
using MyScript.OpenInk.Core.Infrastructure.Extensions;
using MyScript.OpenInk.Core.Infrastructure.Patterns;
using MyScript.OpenInk.Core.Infrastructure.Services;
using MyScript.OpenInk.Core.Models;
using MyScript.OpenInk.Core.Services;
using MyScript.OpenInk.Main.Configuration;
using Path = System.IO.Path;

namespace MyScript.OpenInk.Main.Services
{
    public partial class EngineService : Disposable
    {
        protected override void ReleaseUnmanagedResources()
        {
            if (!(ContentPackage is { } package))
            {
                return;
            }

            // Unlock file handle.
            var token = package.GetValue(MetadataKeys.ContentPackageMetadataTargetFileToken, string.Empty);
            if (!SafeFileHandles.TryGetValue(token, out var handle))
            {
                return;
            }

            handle.Close();
            handle.Dispose();
        }
    }

    public partial class EngineService : IEngineService
    {
        private bool IsInitialized { get; set; }
        private ConcurrentDictionary<string, SafeFileHandle> SafeFileHandles { get; } = new();
        public Engine Engine { get; private set; }

        public event TypedEventHandler<ContentPackage, Tuple<IEnumerable<ContentPart>, IEnumerable<ContentPart>>>
            PackageContentChanged;

        public event TypedEventHandler<ContentPackage, ContentPackage> PackageChanged;
        public IEnumerable<ContentPart> ContentParts => ContentPackage?.GetParts();

        public ContentPart CreateContentPart(ContentType type = ContentType.TextDocument)
        {
            if (!(ContentPackage is { } package))
            {
                throw new InvalidOperationException("The package is not initialized.");
            }

            var part = package.CreatePart(type.ToNative());
            OnPackageContentChanged(package,
                new Tuple<IEnumerable<ContentPart>, IEnumerable<ContentPart>>(null, new[] {part}));
            return part;
        }

        public void RemoveContentPart(ContentPart part)
        {
            if (!(ContentPackage is { } package))
            {
                throw new InvalidOperationException("The package is not initialized.");
            }

            package.RemovePart(part);
            OnPackageContentChanged(package,
                new Tuple<IEnumerable<ContentPart>, IEnumerable<ContentPart>>(new[] {part}, null));
        }

        public ContentPackage ContentPackage { get; private set; }

        public async Task<StorageFile> GetTargetFileAsync()
        {
            if (!(ContentPackage is { } package))
            {
                throw new InvalidOperationException("The package is not initialized.");
            }

            return await package.GetTargetFileAsync();
        }

        public ContentPackage Open(string path)
        {
            if (!(Engine is { } engine))
            {
                throw new InvalidOperationException("The engine is not initialized.");
            }

            Debug.WriteLine($"{nameof(IEngineService)}.{nameof(Open)}:");
            var previous = ContentPackage;
            ContentPackage = engine.Open(path);
            OnPackageChanged(ContentPackage, previous);
            return ContentPackage;
        }

        public async Task<ContentPackage> OpenAsync(StorageFile file)
        {
            if (!(Engine is { } engine))
            {
                throw new InvalidOperationException("The engine is not initialized.");
            }

            Debug.WriteLine($"{nameof(IEngineService)}.{nameof(OpenAsync)}:");
            // Unlock file handle if any.
            file?.CreateSafeFileHandle()?.Close();
            var previous = ContentPackage;
            if (!(await engine.OpenAsync(file) is { } package))
            {
                return ContentPackage = null;
            }

            ContentPackage = package;
            // Lock file handle
            file = await GetTargetFileAsync();
            var token = ContentPackage.GetValue(MetadataKeys.ContentPackageMetadataTargetFileToken, string.Empty);
            if (SafeFileHandles.TryGetValue(token, out var handle))
            {
                handle.Close();
                handle.Dispose();
            }

            StorageApplicationPermissions.MostRecentlyUsedList.Add(file, file.Path,
                RecentStorageItemVisibility.AppAndSystem);
            SafeFileHandles[token] = file.CreateSafeFileHandle(share: FileShare.None);
            // Invoke events.
            OnPackageChanged(ContentPackage, previous);
            return ContentPackage;
        }

        public bool Save(string path = null)
        {
            if (!(ContentPackage is { } package))
            {
                throw new InvalidOperationException("The package is not initialized.");
            }

            return package.Save(path);
        }

        public async Task<bool> SaveAsync(bool asNew = false)
        {
            if (!(ContentPackage is { } package))
            {
                throw new InvalidOperationException("The package is not initialized.");
            }

            var token = package.GetValue(MetadataKeys.ContentPackageMetadataTargetFileToken, string.Empty);
            if (SafeFileHandles.TryGetValue(token, out var handle))
            {
                handle.Close();
                handle.Dispose();
            }

            var file = await GetTargetFileAsync();
            if (asNew || file == null)
            {
                // Picks a file from the file save picker.
                var picker = new FileSavePicker
                {
                    SuggestedFileName = "New Document", SuggestedStartLocation = PickerLocationId.DocumentsLibrary
                };
                picker.FileTypeChoices.Add("MyScriptInteractiveInkFile".Localize(), new[] {FileTypes.IInk});
                //picker.FileTypeChoices.Add("MyScriptNeboFile".Localize(), new[] {FileTypes.Nebo});
                if (!(await picker.PickSaveFileAsync() is { } picked))
                {
                    return false;
                }

                file = picked;
            }

            if (!await package.SaveAsync(file))
            {
                return false;
            }

            await OpenAsync(await GetTargetFileAsync());
            return true;
        }

        public Editor CreateEditor(IRenderTarget target = null)
        {
            if (!(Engine is { } engine))
            {
                throw new InvalidOperationException("The engine is not initialized.");
            }

            var context = ServiceLocator.Current.Get<IContextService>();
            var dpi = context.Dpi;
            var renderer = engine.CreateRenderer(dpi.X, dpi.Y, target);
            var editor = engine.CreateEditor(renderer);
            editor.SetFontMetricsProvider(new FontMetricsProvider(dpi.X, dpi.Y));
            return editor;
        }

        public void Initialize(sbyte[] certificate)
        {
            if (IsInitialized)
            {
                return;
            }

            var engine = Engine.Create((byte[])(Array)certificate);
            var configuration = engine.Configuration;
            configuration.SetStringArray(ConfigurationKeys.ConfigurationManagerSearchPath,
                new[] {Path.Combine(ApplicationData.Current.LocalFolder.Path, "Assets", "conf")});
            configuration.SetString(ConfigurationKeys.ContentPackageTempFolder,
                ApplicationData.Current.TemporaryFolder.Path);
#if DEBUG
            configuration.SetBoolean(ConfigurationKeys.RendererDebugDrawArcOutlines, true);
            configuration.SetBoolean(ConfigurationKeys.RendererDebugDrawObjectBoxes, true);
            // TODO: fix crash when typesetting with text boxes enabled.
            //configuration.SetBoolean(ConfigurationKeys.RendererDebugDrawTextBoxes, true);
#endif
            Engine = engine;
            IsInitialized = true;
        }

        public async Task InitializeAsync(ILanguage language)
        {
            if (!(Engine is { } engine))
            {
                throw new InvalidOperationException("The engine is not initialized.");
            }

            Debug.WriteLine($"{nameof(IEngineService)}.{nameof(InitializeAsync)}({language.Id})");
            var configuration = engine.Configuration;
            configuration.SetString(ConfigurationKeys.Language, language.Tag);
            await Task.CompletedTask;
        }

        protected virtual void OnPackageChanged(ContentPackage sender, ContentPackage args)
        {
            PackageChanged?.Invoke(sender, args);
        }

        protected virtual void OnPackageContentChanged(ContentPackage sender,
            Tuple<IEnumerable<ContentPart>, IEnumerable<ContentPart>> args)
        {
            PackageContentChanged?.Invoke(sender, args);
        }
    }
}
