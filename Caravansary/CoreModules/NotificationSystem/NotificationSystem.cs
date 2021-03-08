using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Size = System.Windows.Size;

namespace Caravansary
{
    class NotificationSystem
    {
        public static NotificationSystem Instance;


        public LinkedList<Notification> notificationQueue;



        //public Keys keyToAccept = Keys.D4;
        //public Keys keyToDiscard = Keys.D3;

        //public bool checkForKey = false;


        private bool isShowing = false;
        //DispatcherTimer notificationTimer;

        public NotificationSystem()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            //this.mainWindow = mainForm;
            //notificationQueue = new LinkedList<Notification>();

            //mainForm.GetNotificationStackPanel().Height = mainForm.GetNotificationStackPanel().MinHeight;

        }
        public void ShowNotification(Notification notification)
        {

            InsertNotificationInQueue(notification);


        }

        private void InsertNotificationInQueue(Notification newNotification)
        {
            notificationQueue.AddLast(newNotification);
            HandleShowingNotification();
        }

        private void HandleShowingNotification()
        {
            if (isShowing)
                return;
            else
            {
                if (AreNotificationsAwaiting())
                    CreateNotification();
            }


            //if (notificationTimer != null)
            //    return;
            //else
            //{
            //    if (notificationQueue.Count>0)
            //        CreateNotification();

            //}
        }

        public bool AreNotificationsAwaiting()
        {
            if (notificationQueue.Count > 0)
                return true;
            else return false;
        }

        
        public Notification GetCurrentNotification()
        {
            return notificationQueue.Count > 0 ?  notificationQueue.First.Value : null;
        }


        private void CreateNotification()
        {
            isShowing = true;
            //mainWindow.GetNotificationTextbox().Content = GetCurrentNotification().message;





            //  CreateNotificationTimer();old
            //   CreateNotificationGraphicTimer();old


            //var singleAnimation = new DoubleAnimation((float)mainWindow.GetNotificationStackPanel().MaxHeight, TimeSpan.FromSeconds(1));
            //singleAnimation.Completed += OnDisplayingAnimationFinished;

            //Storyboard storyboard = new Storyboard();
            //storyboard.Children.Add(singleAnimation);


            //Storyboard.SetTargetName(singleAnimation, mainWindow.GetNotificationStackPanel().Name);
            //Storyboard.SetTargetProperty(singleAnimation, new PropertyPath("Height"));

            //storyboard.Begin(mainWindow);




            //desiredSize = new Size(mainWindow.GetNotificationStackPanel().Width, mainWindow.GetNotificationStackPanel().MaxHeight);old


        }

        private void OnDisplayingAnimationFinished(object sender, EventArgs e)
        {
            GetCurrentNotification().OnNotificationDisplayed();
        }

        DoubleAnimation hidingSingleAnimation;
        Storyboard hidingStoryBoard;

        public void StartHidingNotification()
        {

            // hidingSingleAnimation = new DoubleAnimation((float)mainWindow.GetNotificationStackPanel().MinHeight, TimeSpan.FromSeconds(1));

            //hidingStoryBoard = new Storyboard();
            //hidingStoryBoard.Children.Add(hidingSingleAnimation);


            //Storyboard.SetTargetName(hidingSingleAnimation, mainWindow.GetNotificationStackPanel().Name);
            //Storyboard.SetTargetProperty(hidingSingleAnimation, new PropertyPath("Height"));

            //hidingStoryBoard.Completed += OnHidingAnimationFinished;
            //hidingStoryBoard.Begin(mainWindow);
            
        }

        private void OnHidingAnimationFinished(object sender, EventArgs e)
        {
            GetCurrentNotification().OnNotificationHidden();
            hidingStoryBoard.Stop();

            isShowing = false;

            notificationQueue.RemoveFirst();
            HandleShowingNotification();
        }

        //public void StopNotificationTimer()
        //{
        //    if (notificationTimer != null)
        //    {
        //        notificationTimer.Stop();
        //        notificationTimer = null;

        //    }
        //}

        //public void CreateNotificationTimer()
        //{
        //    StopNotificationTimer();


        //    notificationTimer = new DispatcherTimer();
        //    notificationTimer.Interval = TimeSpan.FromSeconds(1);
        //    notificationTimer.Tick += new EventHandler(NotificationTimerTick);
        //    notificationTimer.Start();


        //}


        //private void NotificationTimerTick(object sender, EventArgs e)
        //{

        //    notificationQueue.First.Value.discardTimer -= TimeSpan.FromSeconds(1);
        //    if (notificationQueue.First.Value.discardTimer.TotalSeconds <= 1)
        //    {


        //        StartHidingNotification();

        //    }

        //}


        //public void StartHidingNotification()
        //{
        //    StopNotificationTimer();
        //    desiredSize = new Size(mainWindow.GetNotificationStackPanel().Width, mainWindow.GetNotificationStackPanel().MinWidth);
        //    var notification = notificationQueue.First.Value;
        //    notification.OnNotificationHidden();
        //    //CreateNotificationGraphicTimer();

        //}


        //public void FinishHidingNotification()
        //{

        //    StopNotificationTimer();
        //    notificationQueue.RemoveFirst();

        //    HandleShowingNotification();

        //}


        //private void StopNotificationGraphicTimer()
        //{
        //    if (notificationGraphicTimer != null)
        //    {
        //        notificationGraphicTimer.Stop();
        //    }
        //}
        //Size desiredSize;
        //DispatcherTimer notificationGraphicTimer;

        //private void CreateNotificationGraphicTimer()
        //{
        //    StopNotificationGraphicTimer();
        //    notificationGraphicTimer = new DispatcherTimer();
        //    notificationGraphicTimer.Interval = TimeSpan.FromMilliseconds( mainWindow.graphicalProgressBarUpdateInMiliseconds);
        //    notificationGraphicTimer.Tick += new EventHandler(NotificationGraphicTimerTick);
        //    notificationGraphicTimer.Start();

        //}


        //private float lerpTime = 0.1f;
        //private void NotificationGraphicTimerTick(object sender, EventArgs e)
        //{



        //    var startingW = mainWindow.GetNotificationStackPanel().Width;
        //    var startingH = mainWindow.GetNotificationStackPanel().Height;



        //    Size newSize;


        //    if (notificationQueue.First.Value.discardTimer.TotalSeconds <= 1)
        //    {

        //        newSize = new Size(
        //            (int)Math.Floor(Utils.Lerp((float)startingW, (float)desiredSize.Width, lerpTime)),
        //            (int)Math.Floor(Utils.Lerp((float)startingH, (float)desiredSize.Height, lerpTime))
        //            );
        //    }
        //    else
        //    {
        //        newSize = new Size(
        //                        (int)Math.Ceiling(Utils.Lerp((float)startingW, (float)desiredSize.Width, lerpTime)),
        //                        (int)Math.Ceiling(Utils.Lerp((float)startingH, (float)desiredSize.Height, lerpTime))
        //                        );
        //    }



        //    mainWindow.GetNotificationStackPanel().RenderSize = newSize;



        //    if (Math.Abs(desiredSize.Height - mainWindow.GetNotificationStackPanel().Height) < 0.1f)
        //    {
        //        mainWindow.GetNotificationStackPanel().RenderSize = desiredSize;
        //        StopNotificationGraphicTimer();
        //        if (mainWindow.GetNotificationStackPanel().Height <= mainWindow.GetNotificationStackPanel().MinHeight)
        //            FinishHidingNotification();
        //    }
        //}
    }
}
