using System;

namespace DFA
{
    public class TimespanMilestone : Milestone
    {
        public TimeSpan timeSpan;
        public bool checkedToday;
        public string[] milestoneAchievedText;


        public TimespanMilestone(TimeSpan timeSpan)
        {
            this.timeSpan = timeSpan;
        }

        public TimespanMilestone(int h, int m, int s)
        {
            this.timeSpan = new TimeSpan(h, m, s);
        }

        public TimespanMilestone(int h, int m, int s, params string[] milestoneAchievedText)
        {
            this.timeSpan = new TimeSpan(h, m, s);
            this.milestoneAchievedText = milestoneAchievedText;
        }

        public override bool CheckIfPassed(TimeSpan currentActivatedTime)
        {
            if (timeSpan < currentActivatedTime)
                return true;
            return false;
        }



        public override string GetMessageMilestoneAchieved()
        {
            return milestoneAchievedText != null ?
                milestoneAchievedText[new Random().Next(milestoneAchievedText.Length)]
            : Milestone.GetDefaultMilestoneMessage();
        }
    }
}
