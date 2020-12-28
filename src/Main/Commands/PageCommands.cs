using System;
using System.Windows.Input;
using Windows.UI.Xaml.Media.Animation;
using MyScript.InteractiveInk.Enumerations;
using MyScript.OpenInk.Core.Commands;
using MyScript.OpenInk.Core.Infrastructure.Commands;
using MyScript.OpenInk.Core.Infrastructure.Services;
using MyScript.OpenInk.Core.Models;
using MyScript.OpenInk.Core.Services;
using MyScript.OpenInk.Main.Configuration;
using MyScript.OpenInk.Main.Extensions;
using MyScript.OpenInk.Main.Views.Page;

namespace MyScript.OpenInk.Main.Commands
{
    public class PageCommands : IPageCommands
    {
        private ICommand _commandAdd;
        private ICommand _commandGoBack;
        private ICommand _commandGoForward;
        private ICommand _commandOpen;
        private ICommand _commandRemove;

        public static IInfrastructureServices InfrastructureServices =>
            ServiceLocator.Current.Get<IInfrastructureServices>();

        public static IInteractiveInkServices InteractiveInkServices =>
            ServiceLocator.Current.Get<IInteractiveInkServices>();

        public ICommand CommandGoBack =>
            _commandGoBack ??= new RelayCommand(_ => InteractiveInkServices.EditorService.GoBack());

        public ICommand CommandGoForward => _commandGoForward ??=
            new RelayCommand(_ => InteractiveInkServices.EditorService.GoForward());

        public ICommand CommandAdd => _commandAdd ??=
            new RelayCommand<ContentType>(type => InteractiveInkServices.EngineService.CreateContentPart(type));

        public ICommand CommandDelete => _commandRemove ??=
            new RelayCommand<IPage>(page =>
                InteractiveInkServices.EngineService.RemoveContentPart(page.ToNative()));

        public ICommand CommandOpen => _commandOpen ??=
            new RelayCommand<IPage>(async page =>
                await InfrastructureServices.ContextService.RunAsync(() =>
                    InfrastructureServices.NavigationService.Navigate<PageViewPage>(page,
                        new DrillInNavigationTransitionInfo())));
    }
}
