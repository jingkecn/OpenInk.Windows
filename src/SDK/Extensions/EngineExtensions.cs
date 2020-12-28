using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using MyScript.IInk;
using MyScript.InteractiveInk.Annotations;
using MyScript.InteractiveInk.Constants;

namespace MyScript.InteractiveInk.Extensions
{
    public static class EngineExtensions
    {
        #region Open

        public static ContentPackage Open([NotNull] this Engine source, [NotNull] string path)
        {
            Debug.WriteLine($"{nameof(EngineExtensions)}.{nameof(Open)}({path}):");
            var configuration = source.Configuration;
            Debug.WriteLine(
                $"\tengine.language: {configuration.GetString(ParameterKeys.EngineConfigurationLanguageIdentifier)}");
            configuration.SetBoolean(ParameterKeys.EngineConfigurationContentPackageMetadataFullAccess, true);
            var package = source.OpenPackage(path, PackageOpenOption.CREATE);
            var language = package.GetValue(ParameterKeys.ContentPackageMetadataLanguageIdentifier, string.Empty);
            Debug.WriteLine($"\tpackage.language: {language}");
            if (!string.IsNullOrEmpty(language))
            {
                configuration.SetString(ParameterKeys.EngineConfigurationLanguageIdentifier, language);
            }

            configuration.SetBoolean(ParameterKeys.EngineConfigurationContentPackageMetadataFullAccess, false);
            return package;
        }

        public static async Task<ContentPackage> OpenAsync([NotNull] this Engine source,
            [NotNull] StorageFile file)
        {
            Debug.WriteLine($"{nameof(EngineExtensions)}.{nameof(OpenAsync)}:");
            var token = StorageApplicationPermissions.FutureAccessList.Add(file);
            Debug.WriteLine($"\ttoken: {token}");
            // Creates a temporary file and open the content package from the temporary file.
            var temp = await file.CopyAsync(ApplicationData.Current.LocalCacheFolder, file.Name,
                NameCollisionOption.ReplaceExisting);
            // ReSharper disable once MethodHasAsyncOverload
            var package = source.Open(temp.Path);
            // Updates file access token.
            package.SetValue(ParameterKeys.ContentPackageMetadataTargetFileToken, token);
            return package;
        }

        #endregion
    }
}
