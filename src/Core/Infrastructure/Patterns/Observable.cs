using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MyScript.InteractiveInk.Annotations;

namespace MyScript.OpenInk.Core.Infrastructure.Patterns
{
    public abstract partial class Observable
    {
        protected void Set<TValue>(ref TValue storage, TValue value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TValue>.Default.Equals(value, storage))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }
    }

    public abstract partial class Observable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
