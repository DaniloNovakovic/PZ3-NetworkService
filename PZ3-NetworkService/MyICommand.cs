using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PZ3_NetworkService
{
    public class MyICommand : ICommand
    {
        Action _TargetExecuteMethod;
        Func<bool> _TargetCanExecuteMethod;

        public MyICommand(Action executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
        }

        public MyICommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {

            if (_TargetCanExecuteMethod != null)
            {
                return _TargetCanExecuteMethod();
            }

            if (_TargetExecuteMethod != null)
            {
                return true;
            }

            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter)
        {
            if (_TargetExecuteMethod != null)
            {
                _TargetExecuteMethod();
            }
        }
    }
    public class MyICommand<T> : ICommand
    {
        private Action<T> _TargetExecuteMethod;
        private Func<T, bool> _TargetCanExecuteMethod;

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
                T tparm = (T)parameter;
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

        #endregion
    }
}
