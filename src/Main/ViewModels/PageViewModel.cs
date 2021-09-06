using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.BackgroundTransfer;
using Windows.UI.Core.Preview;
using MyScript.IInk;
using MyScript.InteractiveInk.Annotations;
using MyScript.OpenInk.Core.Models;
using MyScript.OpenInk.Core.ViewModels;
using MyScript.OpenInk.Main.Extensions;
using MyScript.OpenInk.Main.Views.Dialogs;

namespace MyScript.OpenInk.Main.ViewModels
{
    public partial class PageViewModel
    {
        private Editor _editor;
        private ContentBlock _selection;

        public Editor Editor
        {
            get => _editor;
            set => Set(ref _editor, value, nameof(Editor));
        }

        public ContentBlock Selection
        {
            get => _selection;
            set => Set(ref _selection, value, nameof(Selection));
        }
    }

    public partial class PageViewModel : InteractiveInkViewModel
    {
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            InfrastructureServices.ContextService.CloseRequested += OnCloseRequested;
            InteractiveInkServices.EngineService.PackageChanged += OnPackageChanged;
            InteractiveInkServices.LanguageService.Initialized += OnLanguageInitialized;
            await Task.CompletedTask;
        }

        private async void OnCloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            var deferral = e.GetDeferral();
            e.Handled = await RequireSaveAsync();
            deferral.Complete();
        }

        private async void OnLanguageInitialized(ILanguage sender, BackgroundDownloadProgress args)
        {
            if (sender.Id != Book.Language?.Id)
            {
                return;
            }

            await InfrastructureServices.ContextService.RunAsync(async () => await RequireLanguageAsync(sender));
        }

        private async void OnPackageChanged(ContentPackage current, ContentPackage previous)
        {
            Book = current is { } package ? await package.ToPlatformAsync() : Book;
        }

        private async void OnContentChanged(Editor editor, IEnumerable<ContentBlock> args)
        {
            await InfrastructureServices.ContextService.RunAsync(() =>
            {
                CanRedo = InteractiveInkServices.EditorService.CanRedo;
                CanUndo = InteractiveInkServices.EditorService.CanUndo;
            });
        }

        private async void OnError(Editor editor, Tuple<ContentBlock, string> args)
        {
            var (block, message) = args;
            await InfrastructureServices.ContextService.RunAsync(() =>
            {
                InfoMessage = message;
                InfoTitle = block.Id;
                IsInfoBarOpen = true;
            });
        }

        private async void OnSelectionChanged(Editor sender, IEnumerable<ContentBlock> args)
        {
            Debug.WriteLine($"{nameof(PageViewModel)}.{nameof(OnSelectionChanged)}:");
            var block = args?.FirstOrDefault();
            Debug.WriteLine($"\tblock: {block?.Id}");
            await InfrastructureServices.ContextService.RunAsync(() => Selection = block);
        }

        protected override void ReleaseManagedResources()
        {
            InfrastructureServices.ContextService.CloseRequested -= OnCloseRequested;
            InteractiveInkServices.EditorService.ContentChanged -= OnContentChanged;
            InteractiveInkServices.EditorService.Error -= OnError;
            InteractiveInkServices.EditorService.SelectionChanged -= OnSelectionChanged;
            InteractiveInkServices.EngineService.PackageChanged -= OnPackageChanged;
            InteractiveInkServices.LanguageService.Initialized -= OnLanguageInitialized;
            InteractiveInkServices.EngineService.Save();
        }
    }

    public partial class PageViewModel : IPageViewModel
    {
        private IBook _book;
        private bool _canAddContent;
        private bool _canCopyContent;
        private bool _canPasteContent;
        private bool _canRedo;
        private bool _canRemoveContent;
        private bool _canUndo;
        private IPage _page;

        private LanguageRequiringDialog LanguageRequiringDialog { get; set; }

        public bool CanAddContent
        {
            get => _canAddContent;
            set => Set(ref _canAddContent, value, nameof(CanAddContent));
        }

        public bool CanCopyContent
        {
            get => _canCopyContent;
            set => Set(ref _canCopyContent, value, nameof(CanCopyContent));
        }

        public bool CanPasteContent
        {
            get => _canPasteContent;
            set => Set(ref _canPasteContent, value, nameof(CanPasteContent));
        }

        public bool CanRemoveContent
        {
            get => _canRemoveContent;
            set => Set(ref _canRemoveContent, value, nameof(CanRemoveContent));
        }

        public bool CanRedo
        {
            get => _canRedo;
            set => Set(ref _canRedo, value, nameof(CanRedo));
        }

        public bool CanUndo
        {
            get => _canUndo;
            set => Set(ref _canUndo, value, nameof(CanUndo));
        }

        public IBook Book
        {
            get => _book;
            set => Set(ref _book, value, nameof(Book));
        }

        public IPage Page
        {
            get => _page;
            set => Set(ref _page, value, nameof(Page));
        }

        public void Initialize(Point position)
        {
            CanAddContent = InteractiveInkServices.EditorService.CanAddBlockAt(position);
            CanCopyContent = InteractiveInkServices.EditorService.CanCopyAt(position);
            CanPasteContent = InteractiveInkServices.EditorService.CanPasteAt(position);
            CanRemoveContent = InteractiveInkServices.EditorService.CanRemoveBlockAt(position);
            InteractiveInkCommands.ContentCommands.Initialize(position);
        }

        public async Task InitializeAsync([NotNull] IPage page)
        {
            Page = page;
            InteractiveInkServices.EditorService.GoTo(Page.ToNative());
            if (Page?.ToNative()?.Package is not { } package)
            {
                return;
            }

            Book = await package.ToPlatformAsync();
            await RequireLanguageAsync(Book.Language);
        }

        public async Task InitializeAsync(IRenderTarget target)
        {
            InteractiveInkServices.EditorService.Initialize(target);
            Editor = InteractiveInkServices.EditorService.Editor;
            InteractiveInkServices.EditorService.ContentChanged += OnContentChanged;
            InteractiveInkServices.EditorService.Error += OnError;
            InteractiveInkServices.EditorService.SelectionChanged += OnSelectionChanged;
            await Task.CompletedTask;
        }

        public async Task RequireLanguageAsync(ILanguage language)
        {
            var service = InteractiveInkServices.LanguageService;
            if (service.IsInstalled(language))
            {
                return;
            }

            if (LanguageRequiringDialog != null)
            {
                return;
            }

            LanguageRequiringDialog = new LanguageRequiringDialog { DataContext = this };
            await LanguageRequiringDialog.ShowAsync();
            LanguageRequiringDialog = null;
        }
    }
}
