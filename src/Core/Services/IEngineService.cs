using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using MyScript.IInk;
using MyScript.InteractiveInk.Annotations;
using MyScript.InteractiveInk.Enumerations;
using MyScript.OpenInk.Core.Models;

namespace MyScript.OpenInk.Core.Services
{
    public partial interface IEngineService
    {
        [CanBeNull] Engine Engine { get; }

        void Initialize(sbyte[] certificate);
        Task InitializeAsync(ILanguage language);
    }

    public partial interface IEngineService
    {
        [NotNull]
        Editor CreateEditor([CanBeNull] IRenderTarget target = null);
    }

    public partial interface IEngineService
    {
        [CanBeNull] ContentPackage ContentPackage { get; }

        [CanBeNull]
        Task<StorageFile> GetTargetFileAsync();

        [NotNull]
        ContentPackage Open([NotNull] string path);

        [CanBeNull]
        Task<ContentPackage> OpenAsync([NotNull] StorageFile file);

        bool Save([CanBeNull] string path = null);
        Task<bool> SaveAsync(bool asNew = false);
    }

    public partial interface IEngineService
    {
        IEnumerable<ContentPart> ContentParts { get; }

        ContentPart CreateContentPart(ContentType type = ContentType.TextDocument);
        void RemoveContentPart([NotNull] ContentPart part);
    }

    public partial interface IEngineService
    {
        event TypedEventHandler<ContentPackage, Tuple<IEnumerable<ContentPart>, IEnumerable<ContentPart>>>
            PackageContentChanged;

        event TypedEventHandler<ContentPackage, ContentPackage> PackageChanged;
    }
}
