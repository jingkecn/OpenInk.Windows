using System;
using MyScript.OpenInk.Core.Infrastructure.Patterns;

namespace MyScript.OpenInk.Main.Infrastructure.ViewModels
{
    public abstract class DisposableViewModel : ObservableAsync, IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void ReleaseManagedResources();

        protected virtual void ReleaseUnmanagedResources()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                ReleaseManagedResources();
            }
        }

        ~DisposableViewModel()
        {
            Dispose(false);
        }
    }
}
