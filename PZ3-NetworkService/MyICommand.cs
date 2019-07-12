using System;
using System.Windows.Input;

namespace PZ3_NetworkService
{
    public class MyICommand : ICommand
    {
        private readonly Action _TargetExecuteMethod;
        private readonly Func<bool> _TargetCanExecuteMethod;

        public MyICommand(Action executeMethod)
        {
            this._TargetExecuteMethod = executeMethod;
        }

        public MyICommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            this._TargetExecuteMethod = executeMethod;
            this._TargetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (this._TargetCanExecuteMethod != null)
            {
                return this._TargetCanExecuteMethod();
            }

            if (this._TargetExecuteMethod != null)
            {
                return true;
            }

            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter)
        {
            if (this._TargetExecuteMethod != null)
            {
                this._TargetExecuteMethod();
            }
        }
    }

    public class MyICommand<T> : ICommand
    {
        private readonly Action<T> _TargetExecuteMethod;
        private readonly Func<T, bool> _TargetCanExecuteMethod;

        public MyICommand(Action<T> executeMethod)
        {
            this._TargetExecuteMethod = executeMethod;
        }

        public MyICommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            this._TargetExecuteMethod = executeMethod;
            this._TargetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        #region ICommand Members

        bool ICommand.CanExecute(object parameter)
        {
            if (this._TargetCanExecuteMethod != null)
            {
                var tparm = (T)parameter;
                return this._TargetCanExecuteMethod(tparm);
            }

            if (this._TargetExecuteMethod != null)
            {
                return true;
            }

            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter)
        {
            this._TargetExecuteMethod?.Invoke((T)parameter);
        }

        #endregion ICommand Members
    }
}