namespace Hospital.Utils
{
    using System;
    using System.Windows.Input;

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Predicate<T>? canExecute;

        public RelayCommand(Action<T> execute, Predicate<T>? canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            if (canExecute == null) return true;
            return parameter is T t && canExecute(t);
        }

        public void Execute(object? parameter)
        {
            if (parameter is T t)
            {
                execute(t);
            }
        }
    }
}
