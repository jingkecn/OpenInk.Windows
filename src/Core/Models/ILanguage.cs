using Windows.Globalization;
using MyScript.InteractiveInk.Annotations;

namespace MyScript.OpenInk.Core.Models
{
    public partial interface ILanguage
    {
        string Checksum { get; }
        string Id { get; }
        string Tag { get; }
        string Url { get; }
    }

    public partial interface ILanguage
    {
        bool IsPlatformLanguage { get; }
        [NotNull] Language PlatformModel { get; }
    }

    public partial interface ILanguage
    {
        string BundledConfigurationFilePath { get; }
        string BundledExtraResourcesFolderPath { get; }
        string BundledResourcesFolderPath { get; }
        bool IsBundled { get; }
        string LocalConfigurationFilePath { get; }
        string LocalInfoFilePath { get; }
        string LocalResourcesFolderPath { get; }
    }
}
