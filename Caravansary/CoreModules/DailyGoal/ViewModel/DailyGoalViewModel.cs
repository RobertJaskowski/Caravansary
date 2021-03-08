using Caravansary.CoreModules.MainBar.ViewModel;
using System;
using System.Windows.Input;

namespace Caravansary.CoreModules.DailyGoal.ViewModel
{
    class DailyGoalViewModel : CoreModule
    {
        public override string ModuleName => "DailyGoal";

        private string _dailyGoalText;
        public string DailyGoalText
        {
            get
            {

                return string.IsNullOrEmpty(_dailyGoalText) ? "Daily goal" : _dailyGoalText ;
            }
            set
            {

                _dailyGoalText = value;
                OnPropertyChanged(nameof(DailyGoalText));
            }
        }



        private MainBarViewModel _mainBarModule;
        private MainBarViewModel MainBarModule
        {
            get
            {
                if (_mainBarModule == null)
                    _mainBarModule = (MainBarViewModel)ModuleManager.Instance.GetCoreModule("MainBar");
                return _mainBarModule;
            }
        }

        public DailyGoalViewModel()
        {
            LoadDailyGoal();

        }


        public void SetDailyGoal(TimeSpan time)
        {
            //label5.Content = "Daily goal: " + time.ToString();

            DailyGoalText = "Daily goal: " + time.ToString();

            MainBarModule.timeSecToFillTopBar = (int)time.TotalSeconds;
        }


        private ICommand _dailyGoalClicked;
        public ICommand DailyGoalClicked
        {
            get
            {
                if (_dailyGoalClicked == null)
                    _dailyGoalClicked = new RelayCommand(
                       (object o) =>
                       {

                           DailyGoalWindow dialog = new DailyGoalWindow();
                           dialog.DataContext = new DailyGoalSetterViewModel();
                           dialog.ShowDialog();
                           DailyGoalSetterViewModel.GetDailyGoalTimespan(out TimeSpan result);
                           SetDailyGoal(result);

                       },
                       (object o) =>
                       {
                           return true;
                       });

                return _dailyGoalClicked;

            }
        }

        private void LoadDailyGoal()
        {

            if (DailyGoalSetterViewModel.GetDailyGoalTimespan(out TimeSpan result))
                SetDailyGoal(result);
            else
                DailyGoalText = "Set daily goal! ";

        }

        public override void CloseModule()
        {
        }
    }
}
