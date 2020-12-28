using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System.UserProfile;
using MyScript.OpenInk.Core.Models;

namespace MyScript.OpenInk.Main.Models
{
    public partial struct Language
    {
        private static string BundledAssetsFolderPath => Path.Combine(Package.Current.InstalledLocation.Path, "Assets");
        private static string LocalAssetsFolderPath => Path.Combine(ApplicationData.Current.LocalFolder.Path, "Assets");
    }

    public partial struct Language : ILanguage
    {
        [JsonPropertyName("sha")] public string Checksum { get; set; }
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("tag")] public string Tag { get; set; }
        [JsonPropertyName("url")] public string Url { get; set; }

        [JsonIgnore]
        public string BundledConfigurationFilePath => Path.Combine(BundledAssetsFolderPath, "conf", $"{Tag}.conf");

        [JsonIgnore]
        public string BundledExtraResourcesFolderPath => Path.Combine(BundledAssetsFolderPath, "resources", "mul");

        [JsonIgnore]
        public string BundledResourcesFolderPath => Path.Combine(BundledAssetsFolderPath, "resources", Tag);

        [JsonIgnore]
        public bool IsBundled =>
            File.Exists(BundledConfigurationFilePath) && Directory.Exists(BundledResourcesFolderPath);

        [JsonIgnore]
        public string LocalConfigurationFilePath => Path.Combine(LocalAssetsFolderPath, "conf", $"{Tag}.conf");

        [JsonIgnore] public string LocalInfoFilePath => Path.Combine(LocalAssetsFolderPath, "INFO", $"{Checksum}.txt");
        [JsonIgnore] public string LocalResourcesFolderPath => Path.Combine(LocalAssetsFolderPath, "resources", Tag);
        [JsonIgnore] public Windows.Globalization.Language PlatformModel => new(Id);

        [JsonIgnore]
        public bool IsPlatformLanguage =>
            PlatformModel.LanguageTag == GlobalizationPreferences.Languages.FirstOrDefault();
    }
}
