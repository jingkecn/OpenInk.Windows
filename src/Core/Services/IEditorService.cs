using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using MyScript.IInk;
using MyScript.InteractiveInk.Annotations;
using MyScript.InteractiveInk.Enumerations;

namespace MyScript.OpenInk.Core.Services
{
    public partial interface IEditorService
    {
        [CanBeNull] Editor Editor { get; }

        void Initialize([CanBeNull] IRenderTarget target = null);
    }

    public partial interface IEditorService
    {
        [CanBeNull] ContentPart ContentPart { get; }
        bool CanGoBack { get; }
        bool CanGoForward { get; }

        #region Navigation

        void GoBack();
        void GoForward();
        void GoTo([NotNull] ContentPart part);

        #endregion
    }

    public partial interface IEditorService
    {
        bool IsEmpty { get; }

        bool CanAddBlockAt(Point position);
        bool CanRemoveBlock([NotNull] ContentBlock block);
        bool CanRemoveBlockAt(Point position);
        void AppendBlock(ContentType type, bool autoScroll = true);
        void AddBlockAt(Point position, ContentType type);
        void RemoveBlock([NotNull] ContentBlock block);
        void RemoveBlockAt(Point position);
    }

    public partial interface IEditorService
    {
        bool CanRedo { get; }
        bool CanUndo { get; }

        void Redo();
        void Undo();
    }

    public partial interface IEditorService
    {
        bool CanCopyAt(Point position);
        bool CanPasteAt(Point position);
        Task CopyAsync(Point position);
        Task CopyAsync([CanBeNull] ContentBlock block);
        Task PasteAsync(Point position);
        Task PasteAsync();
    }

    public partial interface IEditorService
    {
        void ResetView();
        void Typeset();
    }

    public partial interface IEditorService
    {
        event TypedEventHandler<Editor, ContentBlock> ActiveBlockChanged;
        event TypedEventHandler<Editor, IEnumerable<ContentBlock>> ContentChanged;
        event TypedEventHandler<Editor, Tuple<ContentBlock, string>> Error;
        event TypedEventHandler<Editor, ContentPart> PartChanged;
        event TypedEventHandler<Editor, IEnumerable<ContentBlock>> SelectionChanged;
    }
}
