using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFA.ViewModels
{
    using DFA.Commands;
    using Models;
    using System.Windows.Input;

    class ActiveTimeViewModel
    {
        public ActiveTimeViewModel()
        {
            _activeTimeModel = new ArtistModel(TimeSpan.FromSeconds(0));
           // UpdateCommand = new ActiveTimeUpdateCommand(this);
        }
        public bool CanUpdate
        {
            get
            {
                return true;
            }
            
        }

        private ArtistModel _activeTimeModel;
        public ArtistModel ActiveTimeModel
        {
            get
            {
                return _activeTimeModel;
            }

        }

        public ICommand UpdateCommand
        {
            get;
            private set;
        }

        public void SaveChanges()
        {

        }

    }


}
