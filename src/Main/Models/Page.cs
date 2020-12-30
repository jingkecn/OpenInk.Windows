using MyScript.InteractiveInk.Enumerations;
using MyScript.OpenInk.Core.Commands;
using MyScript.OpenInk.Core.Models;
using MyScript.OpenInk.Core.Services;
using MyScript.OpenInk.Main.Configuration;

namespace MyScript.OpenInk.Main.Models
{
    public struct Page : IPage
    {
        public int Index { get; set; }
        public bool IsDocument => Type == ContentType.TextDocument;
        public bool IsViewScaleEnabled => Type != ContentType.Text && Type != ContentType.TextDocument;
        public ContentType Type { get; set; }
        public IInteractiveInkCommands InteractiveInkCommands => ServiceLocator.Current.Get<IInteractiveInkCommands>();
        public IInteractiveInkServices InteractiveInkServices => ServiceLocator.Current.Get<IInteractiveInkServices>();
    }
}
