using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Caravansary
{
    class BoolToVisibilityConverter : IValueConverter
    {
        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    if (value is Visibility)
        //    {
        //        Visibility v = (Visibility)value;
        //        if (v == Visibility.Visible) 
        //            return true;
        //        else 
        //            return false;
        //    }
        //    else if (value is bool)
        //    {
        //        bool v = (bool)value;
        //        if (v)
        //            return Visibility.Visible;
        //        else
        //            return Visibility.Collapsed;
        //    }

        //    return value;
        //}

        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    if (value is Visibility)
        //    {
        //        Visibility v = (Visibility)value;
        //        if (v == Visibility.Visible)
        //            return true;
        //        else 
        //            return false;
        //    }
        //    else if (value is bool)
        //    {
        //        bool v = (bool)value;
        //        if (v)
        //            return Visibility.Visible;
        //        else
        //            return Visibility.Collapsed;
        //    }

        //    return value;
        //}
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (bool)value;
            if (v == true)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Visibility)value == Visibility.Visible)
                return true;
            else
                return false;
        }
    }
}
