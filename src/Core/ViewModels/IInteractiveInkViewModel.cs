using System.Threading.Tasks;
using MyScript.IInk;
using MyScript.InteractiveInk.Annotations;
using MyScript.OpenInk.Core.Commands;
using MyScript.OpenInk.Core.Services;

namespace MyScript.OpenInk.Core.ViewModels
{
    public interface IInteractiveInkViewModel
    {
        IInteractiveInkCommands InteractiveInkCommands { get; }
        IInteractiveInkServices InteractiveInkServices { get; }

        Task InitializeAsync([CanBeNull] IRenderTarget target = null);
    }
}
