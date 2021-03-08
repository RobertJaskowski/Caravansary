using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Caravansary.CoreModules.MainBar.ViewModel
{
    class MainBarViewModel : CoreModule
    {

        #region Properties
        public override string ModuleName => "MainBar";

        private Color _topBarStateColor;
        public Color TopBarStateColor
        {
            get
            {

                return _topBarStateColor;
            }
            set
            {
                _topBarStateColor = value;
                OnPropertyChanged("TopBarStateColor");
            }
        }

        private double _progressTopBar;
        public double ProgressTopBar
        {
            get
            {
                return _progressTopBar;
            }
            set
            {
                _progressTopBar = value;
                OnPropertyChanged(nameof(ProgressTopBar));
            }
        }

        #endregion



        float topPercentFilled = 0;
        public int timeSecToFillTopBar = 0;//todo



        public void UpdateTopBar(double totalSeconds)
        {
            if (timeSecToFillTopBar == 0)
                return;
            

            float rest = (float)(totalSeconds % (timeSecToFillTopBar));
            topPercentFilled = Utils.ToProcentage(rest, 0, timeSecToFillTopBar);

            ProgressTopBar = topPercentFilled;

            //progressBarTopMost.SetValueWithAnimation(topPercentFilled, true);



        }


        public void SetBarColor(Color color)
        {
            TopBarStateColor = color;
        }

        public override void CloseModule()
        {
        }
    }
}
