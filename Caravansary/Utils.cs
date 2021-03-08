using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Windows.Controls;

namespace Caravansary
{
    public static class Utils
    {
        public static void DoubleBuffered(this Control control, bool enabled)
        {
            var prop = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            prop.SetValue(control, enabled, null);
        }
        public static StringBuilder Truncate(this StringBuilder value, int maxLength)
        {
            if (value.Length < 1) return value;
            if (value.Length <= maxLength)
                return value;
            value.Remove(maxLength  ,value.Length - maxLength);
            value = new StringBuilder(value.ToString());
            //StringBuilder v =  value.Length <= maxLength ? value : value.Remove(maxLength - (value.Length - maxLength), value.Length);
            return value;
        }
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
        public static int ValueToProgressBarProcent(float current, float min, float max)
        {
            var range = max - min;
            var correctedStartVal = current - min;
            return (int)((correctedStartVal * 10000) / range);
        }

        public static float ProcentToProgressBarValue(ProgressBar progressBar, float percent)
        {
            return (float)(progressBar.Maximum *  percent / 100);
        }



        public static int ToProgressBarProcent(ProgressBar progressBar, float current)
        {
            var range = progressBar.Maximum - progressBar.Minimum;
            var correctedStartVal = current - progressBar.Minimum;

            return (int)((correctedStartVal * progressBar.Maximum) / range);
        }

        
        public static float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat + (secondFloat - firstFloat) * by;
        }
        public static float ToProcentage(float current, float min, float max)
        {
            var range = max - min;
            var correctedStartVal = current - min;

            return (correctedStartVal * 100) / range;
        }

        internal static double ToProcentage(double value, double minimum, double maximum)
        {
            var range = maximum - minimum;
            var correctedStartVal = value - minimum;

            return (correctedStartVal * 100) / range;
        }

        internal static double ProcentToValue(double procent, double max)
        {



            return (max * procent / 100);
        }


        //public static System.Numerics.Vector2 Lerp(Vector2 firstVector, Vector2 secondVector, float by)
        //{
        //    float retX = Lerp(firstVector.X, secondVector.X, by);
        //    float retY = Lerp(firstVector.Y, secondVector.Y, by);
        //    return new Vector2(retX, retY);
        //}

    }
}
