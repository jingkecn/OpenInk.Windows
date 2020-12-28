using System.Threading.Tasks;
using Windows.Foundation;
using MyScript.IInk;
using MyScript.InteractiveInk.Annotations;
using MyScript.OpenInk.Core.Models;

namespace MyScript.OpenInk.Core.ViewModels
{
    public partial interface IPageViewModel
    {
        IBook Book { get; }
        IPage Page { get; }

        Task InitializeAsync(IPage page);
        Task InitializeAsync([NotNull] IRenderTarget target);
        Task RequireLanguageAsync(ILanguage language);
    }

    public partial interface IPageViewModel
    {
        bool CanAddContent { get; }
        bool CanCopyContent { get; }
        bool CanPasteContent { get; }
        bool CanRemoveContent { get; }

        void Initialize(Point position);
    }

    public partial interface IPageViewModel
    {
        bool CanRedo { get; }
        bool CanUndo { get; }
    }
}
