using System;
using System.Windows.Input;
using MyScript.InteractiveInk.Annotations;

namespace MyScript.OpenInk.Core.Infrastructure.Commands
{
    /// <summary>
    ///     A command whose sole purpose is to relay its functionality
    ///     to other objects by invoking delegates.
    ///     The default return value for the CanExecute method is 'true'.
    ///     OnCanExecuteChanged needs to be called whenever
    ///     CanExecute is expected to return a different value.
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        /// <summary>
        ///     Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic. If the value is null, the command can always execute.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            Execute = execute;
            CanExecute = canExecute;
        }

        [NotNull] private Action<T> Execute { get; }

        [CanBeNull] private Predicate<T> CanExecute { get; }

        /// <summary>
        ///     Determines whether this RelayCommand can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command. If the command does not require data to be passed,
        ///     this object can be set to null.
        /// </param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        bool ICommand.CanExecute([CanBeNull] object parameter)
        {
            return CanExecute == null || (parameter is T typed && CanExecute(typed));
        }

        /// <summary>
        ///     Executes the RelayCommand on the current command target.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command. If the command does not require data to be passed,
        ///     this object can be set to null.
        /// </param>
        void ICommand.Execute([CanBeNull] object parameter)
        {
            Execute((T)parameter);
        }

        /// <summary>
        ///     Raised when <code>OnCanExecuteChanged</code> is called.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        ///     Method used to raise the CanExecuteChanged event
        ///     to indicate that the return value of the CanExecute
        ///     method has changed.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null) : base(execute, canExecute)
        {
        }
    }
}
