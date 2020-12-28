using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace MyScript.OpenInk.Core.Models
{
    public interface IBook : IInteractiveInkModel
    {
        ILanguage Language { get; }
        string Name { get; }
        int PageCount { get; }
        IEnumerable<IPage> Pages { get; }
        StorageFile TargetFile { get; }
        BasicProperties TargetFileProperties { get; }
    }
}
