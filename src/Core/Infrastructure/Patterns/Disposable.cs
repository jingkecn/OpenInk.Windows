using System;

namespace MyScript.OpenInk.Core.Infrastructure.Patterns
{
    public abstract class Disposable : IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void ReleaseManagedResources() { }

        protected virtual void ReleaseUnmanagedResources() { }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                ReleaseManagedResources();
            }
        }

        ~Disposable()
        {
            Dispose(false);
        }
    }
}
