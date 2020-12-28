using System.Windows.Input;
using Windows.Storage;
using MyScript.OpenInk.Core.Commands;
using MyScript.OpenInk.Core.Infrastructure.Commands;
using MyScript.OpenInk.Core.Services;
using MyScript.OpenInk.Main.Configuration;

namespace MyScript.OpenInk.Main.Commands
{
    public class BookCommands : IBookCommands
    {
        private ICommand _commandCreate;
        private ICommand _commandOpen;
        private ICommand _commandSave;

        public ICommand CommandCreate =>
            _commandCreate ??= new RelayCommand<string>(path => InteractiveInkServices.EngineService.Open(path));

        public ICommand CommandOpen => _commandOpen ??=
            new RelayCommand<StorageFile>(async file => await InteractiveInkServices.EngineService.OpenAsync(file));

        public ICommand CommandSave => _commandSave ??=
            new RelayCommand<bool>(async asNew => await InteractiveInkServices.EngineService.SaveAsync(asNew));

        public IInteractiveInkServices InteractiveInkServices => ServiceLocator.Current.Get<IInteractiveInkServices>();
    }
}
