using System.Runtime.CompilerServices;
using MyScript.InteractiveInk.Annotations;
using MyScript.OpenInk.Core.Infrastructure.Services;
using MyScript.OpenInk.Core.Infrastructure.ViewModels;

namespace MyScript.OpenInk.Core.Infrastructure.Patterns
{
    public abstract partial class ObservableAsync : Observable
    {
        [NotifyPropertyChangedInvocator]
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            InfrastructureServices?.ContextService?.RunAsync(() => base.OnPropertyChanged(propertyName));
        }
    }

    public abstract partial class ObservableAsync : IInfrastructureViewModel
    {
        [CanBeNull] public abstract IInfrastructureServices InfrastructureServices { get; }
    }
}
