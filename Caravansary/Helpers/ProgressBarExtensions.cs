using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Caravansary
{
    public static class ProgressBarExtensions
    {




        public static void StopAnimation(this ProgressBar progressBar)
        {
            progressBar.BeginAnimation(ProgressBar.ValueProperty, null);
        }

        public static void SetValueWithAnimation(this ProgressBar progressBar, double value, TimeSpan duration)
        {

            DoubleAnimation animation = new DoubleAnimation(value, duration);
            progressBar.BeginAnimation(ProgressBar.ValueProperty, animation);
        }

        public static void SetPercentValueWithAnimation(this ProgressBar progressBar, double percent, TimeSpan duration)
        {

            double newValue = (progressBar.Maximum * percent / 100);

            DoubleAnimation animation = new DoubleAnimation(newValue, duration);
            progressBar.BeginAnimation(ProgressBar.ValueProperty, animation);
        }

        //public static void SetValueWithAnimation(this ProgressBar progressBar, double value)
        //{

        //    DoubleAnimation animation = new DoubleAnimation(value, duration);
        //    progressBar.BeginAnimation(ProgressBar.ValueProperty, animation);
        //}



        public static void SetValueWithAnimation(this ProgressBar progressBar, double value, bool jumpOnLowerValue)
        {
            TimeSpan ts = TimeSpan.FromMilliseconds((100 - value) * 10);

            DoubleAnimation animation = new DoubleAnimation(value, ts);
            progressBar.BeginAnimation(ProgressBar.ValueProperty, animation, HandoffBehavior.Compose);

            //if (!jumpOnLowerValue)
            //{
            //    SetValueWithAnimation(progressBar, value);
            //}
            //else
            //{
            //    if (value < progressBar.Value)
            //    {
            //        DoubleAnimation animation = new DoubleAnimation(value, TimeSpan.FromMilliseconds(0));
            //        progressBar.BeginAnimation(ProgressBar.ValueProperty, animation);
            //    }
            //    else
            //        SetValueWithAnimation(progressBar, value);

            //}
        }
    }
}
