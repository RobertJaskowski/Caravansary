using System;
using System.Collections.Generic;

namespace DFA
{
    class TimespanMilestoneModule : IMilestoneModule
    {


        public static List<TimespanMilestone> timeMilestone = new List<TimespanMilestone>
        {
            new TimespanMilestone(0,0,3, "Nice start"),
            new TimespanMilestone(0,0,10 ),
            new TimespanMilestone(0,0,15 ),
            new TimespanMilestone(0,0,30 ),
            new TimespanMilestone(0,1,0 ),
            new TimespanMilestone(0,2,0 )

        };

        IMainWindow mainForm;
        public TimespanMilestoneModule(IMainWindow mainForm)
        {
            this.mainForm = mainForm;
        }

        private void MilestoneAchieved(TimespanMilestone currentMilestone)
        {
            mainForm.ShowNotification(new Notification(currentMilestone.GetMessageMilestoneAchieved(), false,TimeSpan.FromSeconds(2)));
        }

        public void Tick()
        {
            foreach (var currentMilestone in timeMilestone)
            {
                if (!currentMilestone.checkedToday)
                {
                    if (currentMilestone.CheckIfPassed(mainForm.GetActivatedTime()))
                    {
                        currentMilestone.checkedToday = true;
                        MilestoneAchieved(currentMilestone);
                    }
                }
            }
        }
    }
}
