using System.Windows.Input;
using MyScript.OpenInk.Core.Commands;
using MyScript.OpenInk.Core.Infrastructure.Commands;
using MyScript.OpenInk.Core.Models;
using MyScript.OpenInk.Core.Services;
using MyScript.OpenInk.Main.Configuration;

namespace MyScript.OpenInk.Main.Commands
{
    public class LanguageCommands : ILanguageCommands
    {
        private ICommand _commandCancel;
        private ICommand _commandInstall;
        private ICommand _commandPause;
        private ICommand _commandResume;
        private ICommand _commandUninstall;
        public IInteractiveInkServices InteractiveInkServices => ServiceLocator.Current.Get<IInteractiveInkServices>();

        public ICommand CommandCancel => _commandCancel ??= new RelayCommand<ILanguage>(async language =>
            await InteractiveInkServices.LanguageService.CancelAsync(language));

        public ICommand CommandInstall => _commandInstall ??= new RelayCommand<ILanguage>(async language =>
            await InteractiveInkServices.LanguageService.InstallAsync(language));

        public ICommand CommandPause => _commandPause ??= new RelayCommand<ILanguage>(async language =>
            await InteractiveInkServices.LanguageService.PauseAsync(language));

        public ICommand CommandResume => _commandResume ??= new RelayCommand<ILanguage>(async language =>
            await InteractiveInkServices.LanguageService.ResumeAsync(language));

        public ICommand CommandUninstall => _commandUninstall ??= new RelayCommand<ILanguage>(async language =>
            await InteractiveInkServices.LanguageService.UninstallAsync(language));
    }
}
