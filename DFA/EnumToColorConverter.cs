using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DFA
{
    public enum PossibleValue
    {
        Value1,
        Value2
    }
    public class EnumToColorConverter : IValueConverter
    {

        public Color Visual1 { get; set; }
        public Color Visual2 { get; set; }



        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is PossibleValue))
                return null; //Or a some default Visual

            PossibleValue val = (PossibleValue)value;

            switch (val)
            {
                case PossibleValue.Value1:
                    return Visual1;
                default:
                    return Visual2;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
