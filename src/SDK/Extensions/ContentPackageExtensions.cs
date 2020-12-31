using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using MyScript.IInk;
using MyScript.InteractiveInk.Annotations;
using MyScript.InteractiveInk.Constants;

namespace MyScript.InteractiveInk.Extensions
{
    public static partial class ContentPackageExtensions
    {
        public static IEnumerable<ContentPart> GetParts([NotNull] this ContentPackage source)
        {
            var count = source.PartCount;
            for (var index = 0; index < count; index++)
            {
                yield return source.GetPart(index);
            }
        }
    }

    public static partial class ContentPackageExtensions
    {
        #region Getters

        public static bool GetValue([NotNull] this ContentPackage source, [NotNull] string key,
            bool defaultValue = default)
        {
            return source.Metadata?.GetBoolean(key, defaultValue) ?? defaultValue;
        }

        public static double GetValue([NotNull] this ContentPackage source, [NotNull] string key,
            double defaultValue = default)
        {
            return source.Metadata?.GetNumber(key, defaultValue) ?? defaultValue;
        }

        public static string GetValue([NotNull] this ContentPackage source, [NotNull] string key,
            string defaultValue = default)
        {
            defaultValue ??= string.Empty;
            return source.Metadata?.GetString(key, defaultValue) ?? defaultValue;
        }

        public static string[] GetValue([NotNull] this ContentPackage source, [NotNull] string key,
            string[] defaultValue = default)
        {
            return source.Metadata?.GetStringArray(key, defaultValue) ?? defaultValue;
        }

        #endregion

        #region Setters

        public static void SetValue([NotNull] this ContentPackage source, [NotNull] string key, bool value)
        {
            var parameters = source.Metadata;
            parameters.SetBoolean(key, value);
            source.Metadata = parameters;
        }

        public static void SetValue([NotNull] this ContentPackage source, [NotNull] string key, double value)
        {
            var parameters = source.Metadata;
            parameters.SetNumber(key, value);
            source.Metadata = parameters;
        }

        public static void SetValue([NotNull] this ContentPackage source, [NotNull] string key, string value)
        {
            var parameters = source.Metadata;
            parameters.SetString(key, value);
            source.Metadata = parameters;
        }

        public static void SetValue([NotNull] this ContentPackage source, [NotNull] string key, string[] value)
        {
            var parameters = source.Metadata;
            parameters.SetStringArray(key, value);
            source.Metadata = parameters;
        }

        #endregion
    }

    public static partial class ContentPackageExtensions
    {
        [CanBeNull]
        public static async Task<StorageFile> GetTargetFileAsync([NotNull] this ContentPackage source)
        {
            var token = source.GetValue(MetadataKeys.ContentPackageMetadataTargetFileToken, default(string));
            Debug.WriteLine($"{nameof(ContentPackage)}.{nameof(GetTargetFileAsync)}: {token}");
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            return await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
        }

        #region Save

        public static bool Save([NotNull] this ContentPackage source, [NotNull] string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    source.Save();
                    return File.Exists(path);
                }

                source.SaveAs(path);
                return File.Exists(path);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        [CanBeNull]
        public static async Task<bool> SaveAsync([NotNull] this ContentPackage source, [NotNull] StorageFile file)
        {
            var token = StorageApplicationPermissions.FutureAccessList.Add(file);
            Debug.WriteLine($"{nameof(ContentPackage)}.{nameof(SaveAsync)}: {token}");
            source.SetValue(MetadataKeys.ContentPackageMetadataTargetFileToken, token);
            var tempPath = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, file.Name);
            if (!source.Save(tempPath))
            {
                return false;
            }

            var temp = await StorageFile.GetFileFromPathAsync(tempPath);
            await temp.CopyAndReplaceAsync(file);
            await temp.DeleteAsync(StorageDeleteOption.PermanentDelete);
            return true;
        }

        #endregion
    }
}
