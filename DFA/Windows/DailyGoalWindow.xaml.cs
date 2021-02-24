using DFA.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DFA.Windows
{
    /// <summary>
    /// Interaction logic for DailyGoalWindow.xaml
    /// </summary>
    public partial class DailyGoalWindow : Window
    {

        public TimeSpan returnTime;

        public DailyGoalWindow()
        {
            InitializeComponent();
            awaitingInputConfirmation = false;
            buttonAccept.Visibility = Visibility.Hidden;
        }




        public static void SaveDailyGoalTimespan(TimeSpan timeSpan)
        {



            RegistryKey key;
            key = Registry.CurrentUser.OpenSubKey("DFA", true);
            if (key == null)
                key = Registry.CurrentUser.CreateSubKey("DFA", true);

            int hours = timeSpan.Hours;
            int minutes = timeSpan.Minutes;
            key.SetValue("DFADailyGoalHour", hours);
            key.SetValue("DFADailyGoalMinutes", minutes);

            key.Close();



        }

        public static bool GetDailyGoalTimespan(out TimeSpan result)
        {


            int hours = Settings.Default.DailyGoalHour;
            int minutes = Settings.Default.DailyGoalMinutes;

                if (hours + minutes > 0)
                {

                    result = TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes);

                    return true;
                }


            result = TimeSpan.FromMilliseconds(0);
            return false;
        }



        private bool awaitingInputConfirmation = false;
    

        private void ValidateInput()
        {
            buttonAccept.Visibility = Visibility.Hidden;
            awaitingInputConfirmation = false;
            var text = textBoxInputTime.Text;



            bool success = TimeSpan.TryParse(text, out TimeSpan t);

            if (!success)
            {
                DisplayErrorMessage();
                return;
            }

            if (t.Days > 0)
            {
                label1.Text = t.Days + " " + t.TotalHours + " " + t.TotalMinutes + " \n You are setting your daily goal for more than a day!";
                return;
            }

            if (t.TotalMinutes < 1)
            {
                label1.Text = t.ToString() + "Your daily goal should be at least a minute!";
                return;
            }




            label1.Text = "You are setting your daily goal to " + t.ToString() + "\n Confirm your input";
            returnTime = t;

            buttonAccept.Visibility = Visibility.Visible;
            awaitingInputConfirmation = true;


        }



        private void DisplayErrorMessage()
        {
            label1.Text = "Input time incorrect format ex. \"1:30\" as of 1hour and 30minutes";
        }

      
        private void TextBoxInputTime_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            ValidateInput();

        }

       
        private void ButtonAccept_Click(object sender, RoutedEventArgs e)
        {
            if (awaitingInputConfirmation)
            {

                SaveDailyGoalTimespan(returnTime);
                this.DialogResult = true;
                label1.Text = "ace";
            }
        }
    }
}
