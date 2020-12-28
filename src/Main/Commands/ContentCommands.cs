using System.Windows.Input;
using Windows.Foundation;
using MyScript.IInk;
using MyScript.InteractiveInk.Enumerations;
using MyScript.OpenInk.Core.Commands;
using MyScript.OpenInk.Core.Infrastructure.Commands;
using MyScript.OpenInk.Core.Services;
using MyScript.OpenInk.Main.Configuration;

namespace MyScript.OpenInk.Main.Commands
{
    public class ContentCommands : IContentCommands
    {
        private ICommand _commandAdd;
        private ICommand _commandAppend;
        private ICommand _commandCopy;
        private ICommand _commandCopySelection;
        private ICommand _commandDeleteSelection;
        private ICommand _commandPaste;
        private ICommand _commandPasteSelection;
        private ICommand _commandRedo;
        private ICommand _commandRemove;
        private ICommand _commandTypeset;
        private ICommand _commandUndo;

        private static IEditorService EditorService => InteractiveInkServices.EditorService;

        private static IInteractiveInkServices InteractiveInkServices =>
            ServiceLocator.Current.Get<IInteractiveInkServices>();

        public Point ContextPosition { get; set; }

        public void Initialize(Point position)
        {
            ContextPosition = position;
        }

        public ICommand CommandTypeset =>
            _commandTypeset ??= new RelayCommand(_ => EditorService.Typeset());

        public ICommand CommandCopy =>
            _commandCopy ??= new RelayCommand(async _ => await EditorService.CopyAsync(ContextPosition));

        public ICommand CommandCopySelection => _commandCopySelection ??=
            new RelayCommand<ContentBlock>(async block => await EditorService.CopyAsync(block));

        public ICommand CommandPaste =>
            _commandPaste ??= new RelayCommand(async _ => await EditorService.PasteAsync(ContextPosition));

        public ICommand CommandPasteSelection =>
            _commandPasteSelection ??= new RelayCommand(async _ => await EditorService.PasteAsync());

        public ICommand CommandRedo => _commandRedo ??= new RelayCommand(_ => EditorService.Redo());
        public ICommand CommandUndo => _commandUndo ??= new RelayCommand(_ => EditorService.Undo());

        public ICommand CommandAdd => _commandAdd ??=
            new RelayCommand<ContentType>(type => EditorService.AddBlockAt(ContextPosition, type));

        public ICommand CommandAppend =>
            _commandAppend ??= new RelayCommand<ContentType>(type => EditorService.AppendBlock(type));

        public ICommand CommandDelete =>
            _commandRemove ??= new RelayCommand(_ => EditorService.RemoveBlockAt(ContextPosition));

        public ICommand CommandDeleteSelection => _commandDeleteSelection ??=
            new RelayCommand<ContentBlock>(block => EditorService.RemoveBlock(block));
    }
}
