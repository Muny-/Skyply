using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SKYPE4COMLib;

namespace Skyply
{
    public class NotificationManager
    {
        public List<Notification> Notifications = new List<Notification>();
        public MainForm parent;
        Timer relocateTimer = new Timer();
        public Screen rightScreen;

        public NotificationManager(MainForm parent)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (rightScreen != null)
                {
                    if (screen.Bounds.Right > rightScreen.Bounds.Right)
                        rightScreen = screen;
                }
                else
                {
                    rightScreen = screen;
                }
            }

            this.parent = parent;
            relocateTimer.Interval = 10;
            relocateTimer.Tick += relocateTimer_Tick;
            relocateTimer.Start();
        }

        void relocateTimer_Tick(object sender, EventArgs e)
        {
            SoftRelocateNotifications();
        }

        public Notification AddNotification(IChatMessage message)
        {
            Point loc;

            if (Notifications.Count == 0)
                loc = new Point(rightScreen.Bounds.Right + 438, rightScreen.Bounds.Height - 135);
            else
            {
                int highest_notif_loc = rightScreen.Bounds.Height;

                foreach (Notification new_notif in Notifications)
                {
                    if (new_notif.Location.Y < highest_notif_loc)
                        highest_notif_loc = new_notif.Location.Y;
                }

                loc = new Point(rightScreen.Bounds.Right + 438, highest_notif_loc - 135);
                if (!isPointYOnscreen(loc))
                {
                    Notifications[0].CloseNotification();
                    loc = Notifications[Notifications.Count - 1].Location;
                }
            }
            Notification notif = new Notification(loc, this, message);
            notif.FormClosed += notif_FormClosed;
            Notifications.Add(notif);
            return notif;
        }

        void notif_FormClosed(object sender, FormClosedEventArgs e)
        {
            Notification notif = (Notification)sender;
            int index = Notifications.IndexOf(notif);
            Notifications.Remove(notif);
        }

        public void SoftRelocateNotifications()
        {
            for (int i = 0; i < Notifications.Count; i++)
            {
                if (i == 0)
                    Notifications[i].MoveTo(new Point(rightScreen.Bounds.Right - 448, rightScreen.Bounds.Height - Notifications[i].Height - 10));
                else
                    Notifications[i].MoveTo(new Point(rightScreen.Bounds.Right - 448, Notifications[i - 1].Location.Y - Notifications[i].Height - 10));
            }
        }

        public void RemoveNotification(Notification notif)
        {
            notif.CloseNotification();
        }

        private bool isPointYOnscreen(Point p)
        {
            return p.Y >= rightScreen.Bounds.Location.Y && p.Y <= rightScreen.Bounds.Location.Y + rightScreen.Bounds.Height;
        }
    }
}
