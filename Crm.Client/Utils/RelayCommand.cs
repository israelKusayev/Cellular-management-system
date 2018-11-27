using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Crm.Client.Utils
{
    public delegate bool CanExecuteEventManager();
    public class RelayCommand : ICommand
    {
        readonly Action _act;
        private readonly CanExecuteEventManager _canExecute;
        public RelayCommand(Action act, CanExecuteEventManager canExecute)
        {
            _act = act;
            _canExecute = canExecute;
        }
        public RelayCommand(Action act) : this(act, null)
        {
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public void Execute(object parameter)
        {
            _act();
        }
    }
}
