using MyScript.OpenInk.Core.Infrastructure.Services;

namespace MyScript.OpenInk.Core.Infrastructure.ViewModels
{
    public interface IInfrastructureViewModel
    {
        IInfrastructureServices InfrastructureServices { get; }
    }
}
