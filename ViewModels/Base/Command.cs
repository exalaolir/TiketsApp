using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TiketsApp.ViewModels.Base
{
    internal class Command : ICommand
    {
        private readonly Action execute;
        private readonly Func<object, bool>? canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public Command ( Action execute, Func<object, bool>? canExecute = null )
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public bool CanExecute ( object? parameter )
        {
            return this.canExecute == null || this.canExecute(parameter!);
        }

        public void Execute ( object? parameter )
        {
            this.execute();
        }
    }

    internal class Command<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Func<object, bool>? canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public Command ( Action<T> execute, Func<object, bool>? canExecute = null )
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public bool CanExecute ( object? parameter )
        {
            return this.canExecute == null || this.canExecute(parameter!);
        }

        public void Execute ( object? parameter )
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            this.execute((T)parameter!);
        }
    }
}
