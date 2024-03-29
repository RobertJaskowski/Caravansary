﻿using Caravansary.Core;
using System.Windows;

namespace Caravansary
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {

        public SettingsWindow()
        {
            InitializeComponent();
            
            DataContext = IoC.Get<SettingsWindowViewModel>();
        }

    }
}
