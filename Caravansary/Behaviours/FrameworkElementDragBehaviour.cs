using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Caravansary
{
    public class FrameworkElementDragBehavior : Behavior<FrameworkElement>
    {
        private bool isMouseClicked = false;

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseLeftButtonDown +=
                 new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonDown);
            this.AssociatedObject.MouseLeftButtonUp +=
                 new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonUp);
            this.AssociatedObject.MouseLeave +=
                 new MouseEventHandler(AssociatedObject_MouseLeave);
        }

        private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isMouseClicked = true;
        }

        private void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMouseClicked = false;
        }

        private void AssociatedObject_MouseLeave(object sender, MouseEventArgs e)
        {
            if (isMouseClicked)
            {
                if (this.AssociatedObject == null) return;
                //set the item's DataContext as the data to be transferred
                IDragable dragObject = this.AssociatedObject.DataContext as IDragable;
                if (dragObject != null)
                {
                    DataObject data = new DataObject();
                    data.SetData(dragObject.DataType, this.AssociatedObject.DataContext);
                    System.Windows.DragDrop.DoDragDrop(this.AssociatedObject, data, DragDropEffects.Move);
                }
            }
            isMouseClicked = false;
        }
    }
}