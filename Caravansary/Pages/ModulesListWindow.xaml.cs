using Caravansary.Core;
using System.Windows;

namespace Caravansary.Views
{
    /// <summary>
    /// Interaction logic for ModulesListWindow.xaml
    /// </summary>
    public partial class ModulesListWindow : Window
    {
        public ModulesListWindow()
        {
            InitializeComponent();

            DataContext = IoC.Get<ModulesListViewModel>();
        }
    }
}