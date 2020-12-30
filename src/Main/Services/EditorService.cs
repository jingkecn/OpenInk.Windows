using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using MyScript.IInk;
using MyScript.InteractiveInk.Enumerations;
using MyScript.InteractiveInk.Extensions;
using MyScript.OpenInk.Core.Infrastructure.Patterns;
using MyScript.OpenInk.Core.Services;
using MyScript.OpenInk.Main.Configuration;

namespace MyScript.OpenInk.Main.Services
{
    public partial class EditorService
    {
        private static IEngineService EngineService => ServiceLocator.Current.Get<IEngineService>();
        public ContentType ContentType => ContentPart is { } part ? part.Type.ToPlatformContentType() : default;
        public ContentPart ContentPart => Editor.Part;
    }

    public partial class EditorService : Disposable
    {
        protected override void ReleaseManagedResources()
        {
            Editor?.RemoveListener(this);
        }
    }

    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class EditorService : IEditorListener
    {
        void IEditorListener.PartChanged(Editor editor)
        {
            OnPartChanged(editor, editor.Part);
        }

        void IEditorListener.OnError(Editor editor, string id, string message)
        {
            OnError(editor, new Tuple<ContentBlock, string>(editor.GetBlockById(id), message));
        }

        void IEditorListener.ContentChanged(Editor editor, string[] ids)
        {
            OnContentChanged(editor, ids?.Select(editor.GetBlockById));
        }
    }

    public partial class EditorService : IEditorListener2
    {
        void IEditorListener2.SelectionChanged(Editor editor, string[] ids)
        {
            OnSelectionChanged(editor, ids?.Select(editor.GetBlockById));
        }

        void IEditorListener2.ActiveBlockChanged(Editor editor, string id)
        {
            OnActiveBlockChanged(editor, editor.GetBlockById(id));
        }
    }

    public partial class EditorService : IEditorService
    {
        public event TypedEventHandler<Editor, ContentBlock> ActiveBlockChanged;
        public event TypedEventHandler<Editor, IEnumerable<ContentBlock>> ContentChanged;
        public event TypedEventHandler<Editor, Tuple<ContentBlock, string>> Error;
        public event TypedEventHandler<Editor, ContentPart> PartChanged;
        public event TypedEventHandler<Editor, IEnumerable<ContentBlock>> SelectionChanged;

        public void ResetView()
        {
            if (!(Editor is { } editor))
            {
                throw new InvalidOperationException("The editor is not Initialized");
            }

            if (!(editor.Renderer is { } renderer))
            {
                return;
            }

            renderer.ResetViewOffset();
            renderer.ResetViewScale();
        }

        public void Typeset()
        {
            if (!(Editor is { } editor))
            {
                throw new InvalidOperationException("The editor is not Initialized");
            }

            editor.Typeset();
        }

        public bool CanCopyAt(Point position)
        {
            return Editor is { } editor && editor.CanCopyAt(position);
        }

        public bool CanPasteAt(Point position)
        {
            return Editor is { } editor && editor.CanPasteAt(position);
        }

        public async Task CopyAsync(Point position)
        {
            if (!(Editor is { } editor))
            {
                throw new InvalidOperationException("The editor is not Initialized");
            }

            editor.CopyAt(position);
            await Task.CompletedTask;
        }

        public async Task CopyAsync(ContentBlock block)
        {
            if (!(Editor is { } editor))
            {
                throw new InvalidOperationException("The editor is not Initialized");
            }

            if (editor.CanCopy(block))
            {
                editor.Copy(block);
            }

            await Task.CompletedTask;
        }

        public async Task PasteAsync(Point position)
        {
            if (!(Editor is { } editor))
            {
                throw new InvalidOperationException("The editor is not Initialized");
            }

            editor.PasteAt(position);
            await Task.CompletedTask;
        }

        public async Task PasteAsync()
        {
            if (!(Editor is { } editor))
            {
                throw new InvalidOperationException("The editor is not Initialized");
            }

            editor.Paste();
            await Task.CompletedTask;
        }

        public bool CanRedo => Editor?.CanRedo() ?? false;
        public bool CanUndo => Editor?.CanUndo() ?? false;

        public void Redo()
        {
            if (!(Editor is { } editor))
            {
                throw new InvalidOperationException("The editor is not Initialized");
            }

            editor.Redo();
        }

        public void Undo()
        {
            if (!(Editor is { } editor))
            {
                throw new InvalidOperationException("The editor is not Initialized");
            }

            editor.Undo();
        }

        public bool IsEmpty => !(Editor is { } editor) || editor.IsEmpty(null);

        public bool CanAddBlockAt(Point position)
        {
            if (!(Editor is { } editor))
            {
                throw new InvalidOperationException("The editor is not Initialized");
            }

            return editor.CanAddBlockAt(position);
        }

        public bool CanRemoveBlock(ContentBlock block)
        {
            if (!(Editor is { } editor))
            {
                throw new InvalidOperationException("The editor is not Initialized");
            }

            return editor.CanRemoveBlock(block);
        }

        public bool CanRemoveBlockAt(Point position)
        {
            if (!(Editor is { } editor))
            {
                throw new InvalidOperationException("The editor is not Initialized");
            }

            return editor.CanRemoveBlockAt(position);
        }

        public void AppendBlock(ContentType type, bool autoScroll = true)
        {
            if (!(Editor is { } editor))
            {
                throw new InvalidOperationException("The editor is not Initialized");
            }

            editor.AppendBlock(type, autoScroll);
        }

        public void AddBlockAt(Point position, ContentType type)
        {
            if (!(Editor is { } editor))
            {
                throw new InvalidOperationException("The editor is not Initialized");
            }

            editor.AddBlockAt(position, type);
        }

        public void RemoveBlock(ContentBlock block)
        {
            if (!(Editor is { } editor))
            {
                throw new InvalidOperationException("The editor is not Initialized");
            }

            editor.RemoveBlock(block);
            block.Dispose();
        }

        public void RemoveBlockAt(Point position)
        {
            if (!(Editor is { } editor))
            {
                throw new InvalidOperationException("The editor is not Initialized");
            }

            editor.RemoveBlockAt(position);
        }

        public bool CanGoBack => Editor?.CanGoBack() ?? false;
        public bool CanGoForward => Editor?.CanGoForward() ?? false;

        public void GoBack()
        {
            if (!(Editor is { } editor))
            {
                throw new InvalidOperationException("The editor is not Initialized");
            }

            editor.GoBack();
        }

        public void GoForward()
        {
            if (!(Editor is { } editor))
            {
                throw new InvalidOperationException("The editor is not Initialized");
            }

            editor.GoForward();
        }

        public void GoTo(ContentPart part)
        {
            if (!(Editor is { } editor))
            {
                throw new InvalidOperationException("The editor is not Initialized");
            }

            editor.Part = part;
        }

        public Editor Editor { get; private set; }

        public void Initialize(IRenderTarget target)
        {
            Editor?.RemoveListener(this);
            Editor?.Dispose();
            Editor = null;
            Editor = EngineService.CreateEditor(target);
            Editor.AddListener(this);
        }

        protected virtual void OnActiveBlockChanged(Editor sender, ContentBlock args)
        {
            ActiveBlockChanged?.Invoke(sender, args);
        }

        protected virtual void OnContentChanged(Editor sender, IEnumerable<ContentBlock> args)
        {
            ContentChanged?.Invoke(sender, args);
        }

        protected virtual void OnError(Editor sender, Tuple<ContentBlock, string> args)
        {
            Error?.Invoke(sender, args);
        }

        protected virtual void OnPartChanged(Editor sender, ContentPart args)
        {
            PartChanged?.Invoke(sender, args);
        }

        protected virtual void OnSelectionChanged(Editor sender, IEnumerable<ContentBlock> args)
        {
            SelectionChanged?.Invoke(sender, args);
        }
    }
}
