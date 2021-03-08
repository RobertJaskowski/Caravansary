using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DFA.CoreModules.DailyGoal.ViewModel
{
    class DailyGoalViewModel : CoreModule
    {

        //private string _dailyGoalText;
        //public string DailyGoalText
        //{
        //    get
        //    {

        //        return _dailyGoalText;
        //    }
        //    set
        //    {

        //        _dailyGoalText = value;
        //        OnPropertyChanged(nameof(DailyGoalText));
        //    }
        //}

        //public void SetDailyGoal(TimeSpan time)
        //{
        //    //label5.Content = "Daily goal: " + time.ToString();

        //    DailyGoalText = "Daily goal: " + time.ToString();

        //    timeSecToFillTopBar = (int)time.TotalSeconds;
        //}

        //private ICommand _dailyGoalClicked;
        //public ICommand DailyGoalClicked
        //{
        //    get
        //    {
        //        if (_dailyGoalClicked == null)
        //            _dailyGoalClicked = new RelayCommand(
        //               (object o) =>
        //               {

        //                   DailyGoalWindow dialog = new DailyGoalWindow();
        //                   dialog.DataContext = new DailyGoalViewModel();
        //                   dialog.ShowDialog();
        //                   DailyGoalViewModel.GetDailyGoalTimespan(out TimeSpan result);
        //                   SetDailyGoal(result);

        //               },
        //               (object o) =>
        //               {
        //                   return true;
        //               });

        //        return _dailyGoalClicked;

        //    }
        //}

        //private void LoadDailyGoal()
        //{

        //    if (DailyGoalViewModel.GetDailyGoalTimespan(out TimeSpan result))
        //        SetDailyGoal(result);
        //    else
        //        DailyGoalText = "Set daily goal! ";

        //}
        public override string ModuleName => throw new NotImplementedException();
    }
}
