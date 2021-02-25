using DFA.Properties;
using Microsoft.Win32;
using System;
using System.Windows;

namespace DFA
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {

        private IWindow requestedWindow;
        public SettingsWindow(IWindow window)
        {
            requestedWindow = window;
            InitializeComponent();
        }

    }
}
