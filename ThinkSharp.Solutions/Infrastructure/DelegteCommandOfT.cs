using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ThinkSharp.Solutions.Infrastructure
{
    public class DelegateCommand<T> : ICommand where T : class
    {
        Action<T> _execute;
        Func<T, bool> _canExecute;

        public DelegateCommand(Action<T> execute)
            : this(execute, a => true)
        {
        }

        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            this._execute = execute;
            this._canExecute = canExecute;

            CommandManager.RequerySuggested += (s, e) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute(parameter as T);

            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            this._execute(parameter as T);
        }
    }
}
