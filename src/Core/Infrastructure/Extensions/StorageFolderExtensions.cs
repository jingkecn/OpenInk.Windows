using System;
using System.Threading.Tasks;
using Windows.Storage;
using MyScript.InteractiveInk.Annotations;

namespace MyScript.OpenInk.Core.Infrastructure.Extensions
{
    public static class StorageFolderExtensions
    {
        public static async Task<IStorageFolder> CopyAsync([NotNull] this IStorageFolder source,
            [NotNull] IStorageFolder destinationContainer, [CanBeNull] string desiredNewName = null)
        {
            desiredNewName ??= source.Name;
            var destinationFolder =
                await destinationContainer.CreateFolderAsync(desiredNewName, CreationCollisionOption.OpenIfExists);
            foreach (var file in await source.GetFilesAsync())
            {
                await file.CopyAsync(destinationFolder, file.Name, NameCollisionOption.ReplaceExisting);
            }

            foreach (var folder in await source.GetFoldersAsync())
            {
                await folder.CopyAsync(destinationFolder);
            }

            return destinationFolder;
        }
    }
}
