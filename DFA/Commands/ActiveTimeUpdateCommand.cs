
namespace DFA.Commands
{
    using DFA.ViewModels;
    using System;
    using System.Windows.Input;

    internal class ActiveTimeUpdateCommand : ICommand
    {
        public ActiveTimeUpdateCommand(MainWindowViewModel activeTimeViewModel)
        {
            _ViewModel = activeTimeViewModel;
        }

        private MainWindowViewModel _ViewModel;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            //return _ViewModel.CanUpdate;
            return true;
        }

        public void Execute(object parameter)
        {
            //_ViewModel.SaveChanges();
        }
    }
}
