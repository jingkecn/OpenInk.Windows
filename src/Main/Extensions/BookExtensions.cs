using System;
using System.Linq;
using System.Threading.Tasks;
using MyScript.IInk;
using MyScript.InteractiveInk.Annotations;
using MyScript.InteractiveInk.Constants;
using MyScript.InteractiveInk.Extensions;
using MyScript.OpenInk.Core.Infrastructure.Extensions;
using MyScript.OpenInk.Core.Models;
using MyScript.OpenInk.Core.Services;
using MyScript.OpenInk.Main.Configuration;
using MyScript.OpenInk.Main.Models;

namespace MyScript.OpenInk.Main.Extensions
{
    public static class BookExtensions
    {
        private const string ParamBookName = "Book.Name";
        private static string DefaultName => "BookNameDefault".Localize();

        private static IInteractiveInkServices InteractiveInkServices =>
            ServiceLocator.Current.Get<IInteractiveInkServices>();

        public static async Task<IBook> ToPlatformAsync([NotNull] this ContentPackage source)
        {
            var result = new Book
            {
                Name = source.GetValue(ParamBookName, DefaultName),
                Pages = source.GetParts().Select(part => part.ToPlatform())
            };
            var configuration = InteractiveInkServices.EngineService.Engine.Configuration;
            var identifier = configuration.GetString(ConfigurationKeys.Language);
            result.Language = InteractiveInkServices.LanguageService.Languages.Single(x => x.Tag == identifier);
            if (!(await source.GetTargetFileAsync() is { } file))
            {
                return result;
            }

            result.Name = source.GetValue(ParamBookName, file.DisplayName);
            result.TargetFile = file;
            result.TargetFileProperties = await file.GetBasicPropertiesAsync();

            return result;
        }
    }
}
