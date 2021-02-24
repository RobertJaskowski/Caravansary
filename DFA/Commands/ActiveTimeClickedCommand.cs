using DFA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFA.Commands
{
    class ActiveTimeClickedCommand
    {
        private MainWindowViewModel mainWindowViewModel;

        public ActiveTimeClickedCommand(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
        }
    }
}
