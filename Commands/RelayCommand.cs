using System;
using System.Windows.Input;

namespace BookCollection.Commands
{
    internal class RelayCommand : ICommand
    {
        // HelpClass that implement ICommand, for real separation of UI and logic

        // Type of delegate - Action
        private readonly Action<object> _execute;
        // Type bool value
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> action, Predicate<object> canExecute = null)
        {
            _execute = action;
            //_action = _action ?? throw new ArgumentNullException(nameof(action));
            _canExecute = canExecute;
        }

        // Check if Action can be started
        // If the predicate value is true OR no parameter for predicate given, return true
        // Implement ICommand
        public bool CanExecute(object param) => _canExecute == null || _canExecute(param);

        // Execute the given action
        // Implement ICommand
        public void Execute(object param) => _execute(param);

        // WPF activate/deactivate the UI-Elemente automatically
        // If the ICommand status needs to be checked
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }

        }
    }
}
