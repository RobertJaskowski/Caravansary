using Caravansary.Pages;
using Microsoft.Xaml.Behaviors;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Caravansary
{
    public class DragBehavior : Behavior<UIElement>
    {
        private Point elementStartPosition;
        private Point mouseStartPosition;
        private TranslateTransform transform = new TranslateTransform();
        private Window parent = Application.Current.MainWindow;

        private Point lastCaptured;

        protected override void OnAttached()
        {
            //  parent = IoC.Get<NodeWindow>();

            AssociatedObject.RenderTransform = transform;

            AssociatedObject.MouseLeftButtonDown += (sender, e) =>
            {
                lastCaptured = e.GetPosition(parent);
                elementStartPosition = AssociatedObject.TranslatePoint(new Point(), parent);
                mouseStartPosition = e.GetPosition(parent);
                AssociatedObject.CaptureMouse();

                Debug.WriteLine("down msp" + mouseStartPosition + " esp " + elementStartPosition + " t:" + transform.X + " " + transform.Y);
            };

            AssociatedObject.MouseLeftButtonUp += (sender, e) =>
            {
                AssociatedObject.ReleaseMouseCapture();
            };

            AssociatedObject.MouseMove += (sender, e) =>
            {
                Debug.WriteLine("" + transform.X + " " + transform.Y);

                //Vector diff = e.GetPosition(parent) - mouseStartPosition;
                Vector diff = e.GetPosition(parent) - lastCaptured;
                if (AssociatedObject.IsMouseCaptured)
                {
                    transform.X += diff.X;
                    transform.Y += diff.Y;
                    lastCaptured = e.GetPosition(parent);
                }
                // Debug.WriteLine("msp" + mouseStartPosition + " esp " + elementStartPosition + " t:" + transform.X + " " + transform.Y);
                Debug.WriteLine("msp" + mouseStartPosition + " esp " + elementStartPosition + " t:" + transform.X + " " + transform.Y + " diff:" + diff.X + " " + diff.Y);
            };
        }
    }
}