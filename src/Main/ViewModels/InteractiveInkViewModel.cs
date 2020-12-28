using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Popups;
using MyScript.OpenInk.Core.Commands;
using MyScript.OpenInk.Core.Infrastructure.Extensions;
using MyScript.OpenInk.Core.Services;
using MyScript.OpenInk.Main.Configuration;
using MyScript.OpenInk.Main.Infrastructure.ViewModels;

namespace MyScript.OpenInk.Main.ViewModels
{
    public abstract class InteractiveInkViewModel : InfrastructureViewModel
    {
        public IInteractiveInkCommands InteractiveInkCommands { get; } =
            ServiceLocator.Current.Get<IInteractiveInkCommands>();

        public IInteractiveInkServices InteractiveInkServices { get; } =
            ServiceLocator.Current.Get<IInteractiveInkServices>();

        public virtual async Task<bool> RequireSaveAsync()
        {
            var engineService = InteractiveInkServices.EngineService;
            if ((await engineService.GetTargetFileAsync() is { } file && File.Exists(file.Path)) ||
                InteractiveInkServices.EditorService.IsEmpty)
            {
                return false;
            }

            var commandSave = new UICommand("CommandSave".Localize());
            var commandDoNotSave = new UICommand("CommandDoNotSave".Localize());
            var commandCancel = new UICommand("CommandCancel".Localize());
            var dialog = new MessageDialog("MessageSave".Localize(), "TitleSave".Localize());
            dialog.Commands.Add(commandSave);
            dialog.Commands.Add(commandDoNotSave);
            dialog.Commands.Add(commandCancel);
            var result = await dialog.ShowAsync();
            return result == commandSave
                ? !await engineService.SaveAsync()
                : result != commandDoNotSave;
        }
    }
}
