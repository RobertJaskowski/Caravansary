
namespace DFA.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for TopWindow.xaml
    /// </summary>
    public partial class TopWindow : Window, IView
    {
        Window IView.window
        {
            get
            {
                return window;
            }
        }
    }



}
