
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace Caravansary
{
    class CloseWindowOnKey : Behavior<Window>
    {
        public Key Key { get; set; }

        protected override void OnAttached()
        {

            Window window = this.AssociatedObject;
            if (window != null)
                window.PreviewKeyDown += Window_PreviewKeyDown;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Window window = (Window)sender;
            if (e.Key == Key)
                window.Close();
        }
    }
}
