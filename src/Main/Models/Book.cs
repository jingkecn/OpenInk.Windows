using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.Storage.FileProperties;
using MyScript.OpenInk.Core.Commands;
using MyScript.OpenInk.Core.Models;
using MyScript.OpenInk.Core.Services;
using MyScript.OpenInk.Main.Configuration;

namespace MyScript.OpenInk.Main.Models
{
    public struct Book : IBook
    {
        public ILanguage Language { get; set; }
        public string Name { get; set; }
        public int PageCount => Pages.Count();
        public IEnumerable<IPage> Pages { get; set; }
        public StorageFile TargetFile { get; set; }
        public BasicProperties TargetFileProperties { get; set; }
        public IInteractiveInkCommands InteractiveInkCommands => ServiceLocator.Current.Get<IInteractiveInkCommands>();
        public IInteractiveInkServices InteractiveInkServices => ServiceLocator.Current.Get<IInteractiveInkServices>();
    }
}
