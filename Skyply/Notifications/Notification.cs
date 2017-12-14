using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using Awesomium.Core.Data;
using Awesomium;
using Awesomium.Core;
using SKYPE4COMLib;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.Web;

namespace Skyply
{
    public partial class Notification : Form
    {
        public string Title = "";
        public string HtmlDescription = "";
        public Point TargetLocation;
        private Point startedLocation;
        private NotificationManager notificationManager;

        public IChatMessage message;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        protected override bool ShowWithoutActivation { get { return true; } }

        private IntPtr owner;

        protected override CreateParams CreateParams
        {
            get
            {
                //make sure Top Most property on form is set to false
                //otherwise this doesn't work
                int WS_EX_TOPMOST = 0x00000008;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TOPMOST;
                return cp;
            }
        }

        int rand = 0;

        public Notification(Point location, NotificationManager notificationManager, IChatMessage message)
        {
            this.rand = new Random().Next();
            this.notificationManager = notificationManager;

            string title = "";
            if (message.Chat.Topic != "")
                title = message.Chat.Topic;
            else
                title = message.Sender.FullName;

            this.Title = "Message from <strong>" + title + "</strong>";
            if (message.Chat.Type == TChatType.chatTypeDialog)
                this.HtmlDescription = HttpUtility.HtmlEncode(ParseQuotes(message.Body));
            else
                this.HtmlDescription = "<strong>" + HttpUtility.HtmlEncode(message.Sender.FullName) + "</strong>: " + HttpUtility.HtmlEncode(ParseQuotes(message.Body));

            this.message = message;

            InitializeComponent();
            this.Location = location;
            ease.EasingMode = EasingMode.EaseIn;
            ease2.EasingMode = EasingMode.EaseInOut;
            this.Resize += Notification_Resize;

            webControl1.ConsoleMessage += webControl1_ConsoleMessage;

            if (message.Chat.Type == TChatType.chatTypeDialog)
            {
                this.owner = GetForegroundWindow();
                SKYPE4COMLib.Command command0 = new SKYPE4COMLib.Command();
                command0.Command = string.Format("GET USER {0} AVATAR 1 {1}", message.Sender.Handle, (new FileInfo(Path.GetTempFileName()).Directory).FullName.Replace("\\", "/") + "/" + message.Sender.Handle + rand + ".jpg");
                notificationManager.parent.skype.SendCommand(command0);
            }

            this.Show();
        }

        string ParseQuotes(string input)
        {
            return input.Replace("\r\n\r\n<<< ", "");
        }

        void webControl1_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            //MessageBox.Show(e.Message + "\n\n" + e.Source + ":" + e.LineNumber);
        }

        void Notification_Resize(object sender, EventArgs e)
        {
            notificationManager.SoftRelocateNotifications();
        }

        public void CloseNotification()
        {
            hideNotificationTimer.Start();
        }

        public void MoveTo(Point point)
        {
            duration2 = 0.3;
            startedLocation = this.Location;
            TargetLocation = point;
            moveTimer.Start();
            //this.Location = point;
        }

        QuinticEase ease = new QuinticEase();
        double duration = 1;

        private void showNotificationTimer_Tick(object sender, EventArgs e)
        {
            if (duration > 0.01)
            {
                duration -= 0.02;
                this.Location = new Point((int)Math.Round(ease.Ease(duration) * notificationManager.rightScreen.Bounds.Right, 0) + notificationManager.rightScreen.Bounds.Right - this.Width - 10, this.Location.Y);
            }
            else
            {
                showNotificationTimer.Stop();
            }
        }

        private void hideNotificationTimer_Tick(object sender, EventArgs e)
        {
            if (this.Opacity > 0)
            {
                this.Opacity -= 0.15;
            }
            else
            {
                hideNotificationTimer.Stop();
                System.IO.File.Delete((new FileInfo(Path.GetTempFileName()).Directory).FullName + "/" + message.Sender.Handle + rand + ".jpg");
                this.Close();
            }
        }

        ExponentialEase ease2 = new ExponentialEase();
        double duration2 = 0;

        private void moveTimer_Tick(object sender, EventArgs e)
        {
            if (duration2 < 1)
            {
                duration2 += 0.02;

                if (TargetLocation.Y > startedLocation.Y)
                {
                    this.Location = new Point(this.Location.X, (int)Math.Round(ease2.Ease(duration2) * (TargetLocation.Y - startedLocation.Y), 0) + startedLocation.Y);
                }
                else
                {
                    this.Location = new Point(this.Location.X, startedLocation.Y - (int)Math.Round(ease2.Ease(duration2) * (startedLocation.Y - TargetLocation.Y), 0));
                }
            }
            else
            {
                moveTimer.Stop();
                duration2 = 0;
            }
        }

        private void Awesomium_Windows_Forms_WebControl_MouseEnter(object sender, EventArgs e)
        {
            CloseNotificationTimer.Stop();
        }

        private void CloseNotificationTimer_Tick(object sender, EventArgs e)
        {
            CloseNotification();
        }

        private void Awesomium_Windows_Forms_WebControl_MouseLeave(object sender, EventArgs e)
        {
            CloseNotificationTimer.Start();
        }

        private void Notification_Load(object sender, EventArgs e)
        {
            try
            {
                webControl1.WebSession.AddDataSource("ui", new DirectoryDataSource("ui"));
                webControl1.Reload(true);
            }
            catch { }
        }

        bool isFirstLoad = true;

        private void Awesomium_Windows_Forms_WebControl_DocumentReady(object sender, Awesomium.Core.UrlEventArgs e)
        {
            if (isFirstLoad)
                isFirstLoad = false;
            else
            {
                using (JSObject programInterface = webControl1.CreateGlobalJavascriptObject("programInterface"))
                {
                    programInterface.Bind("updateSize", false, (s, ee) =>
                    {
                        this.Height = Convert.ToInt32(ee.Arguments[0].ToString());
                    });

                    programInterface.Bind("closeForm", false, (s, ee) =>
                    {
                        CloseNotification();
                    });

                    programInterface.Bind("stopHideTimer", false, (s, ee) =>
                    {
                        CloseNotificationTimer.Stop();
                    });

                    programInterface.Bind("startHideTimer", false, (s, ee) =>
                    {
                        CloseNotificationTimer.Start();
                    });

                    programInterface.Bind("sendReply", false, (s, ee) =>
                    {
                        message.Seen = true;
                        message.Chat.SendMessage(ee.Arguments[0].ToString());
                        CloseNotification();
                    });

                    programInterface.Bind("openChat", false, (s, ee) =>
                    {
                        CloseNotification();
                        message.Chat.OpenWindow();
                    });

                    programInterface.Bind("seeMessage", false, (s, ee) =>
                    {
                        message.Seen = true;
                    });

                    programInterface.Bind("openURL", false, (s, ee) =>
                    {
                        string browserPath = GetBrowserPath();
                        if (browserPath == string.Empty)
                            browserPath = "iexplore";
                        Process process = new Process();
                        process.StartInfo = new ProcessStartInfo(browserPath);
                        process.StartInfo.Arguments = "\"" + Uri.EscapeUriString(ee.Arguments[0]) + "\"";
                        process.Start();
                    });
                }
                try
                {
                    string avatar = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADcAAAA3CAYAAACo29JGAAAABmJLR0QAIgC7ALk59N3CAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH3gETFgU7T/vEnQAAAAxpVFh0Q29tbWVudAAAAAAAvK6ymQAACEBJREFUaN7lmkuPFNcVgL9bVd1VXf2a7nm/hzEGg3GUyAFMyOBxSGSBjB1HjoRkS5bi2CtWWcTZeOVNyCIrfgSJyEMZFCcL5DFDQmwvIiU4YBYDBgZmhp5Xz4Oq7qq6WUzX0N10V3fPw5DJkRDSqO+t89U5dR73XNjGIjZ7w1fGRpsMoXw4qIdP1btm3M6dsaT3wfmh4fknDu7E2Gh7SlUvdoRCuza612Q+f33OdY+MDA1PPVa4V8dG9z9jGJ8V/01Kyazr8unKCiuerLmHqQgOmiZpVUWIUnWuWdaBPw0Nf/61wp0YG92xxzDGi4GmHZex5eUNe8FQNEqbVgp61bIGR4aGb2w53BuXPvnloK6/70N9adtcsexNDwb7DJ3dur4GOW7bp89998VfbAncK2OjTbt0fVYrPC3jOIwuLW95xBuORWnRNAAcKeV1207XG3hEnW64b49h/LvwAEYWsrhfY0hXgRPJBFrBilct67mRoeErG4YrDhrLrsdHi4uPLW8di8eJqkrdwUbUa7E5x+XC0tJjT8xHYzFSmlqXBUXQN7bXMOaeJLBKgP+xrFS1b1CptsEuXZ/1XfFJAgO4sLTEsuuV6Fm35fxw70jJHxeyNR+mAM8m4nTqEQTgSI8vFheZtO0tWefLDwtBplqaEEEJ+g/zCzWjYls4zKsdnZzs7qHHNFGAvOtyeW6OsxO3Gc1kcDZxXXkUfb0pWTXRa+ULfLCM49QEi2saP+nr48ddPTSFww8fqmm81NpKd8RAVzU+mprE24R15eIW9GzRNPYYxvhImbGU8rDvVx41E7SUvNbewVvdvSUKlny3sThvdnezOxbb+LoqMrq0jJSyRP+KcH4++7IOn2/RdV5uayMSCgX+7puJJEPNLRteFyS+vuVFvFLctvhWq6dWDAlBd8Ss+Ttd0+jQjQ2vC5Irlr1mPZ+jBC6lqhcBph2X/0Xx9fY5SuD8RrPetsUFZnO5mr9zXJf5fA4pJVJKHCkbWlev+HoXN8yKX434LlmvzNg2H9+fJh+wRkrJlewCY9PTkM9DPs/MygoXJu9hO8GB/uryEn+bnW3Ier7+Po8CYAjlQ4BZt36XdIHf3rvH7+9NsFJBUc/zmMhmOTt+A9d+wPcTSX6QSPJiLM6FiTv85uY4i7ZV8YVOWRa/u3uXf2UXGoLz9fd5BMDPP/+HBPhzNlvX0UCx9EcivN3bx2sdnWuhXUrJPzMZzt28QVwLcfKpp+iJRlerEM/j79PT/OX2LQbjcd7YMUhzJLLWlH6RzXJ24g7n7k6Qk43pYiqC44kEAL/a/4IoSeKNgoUVhVZdJy89ZvM5koXwvmxZ3MkucLilhe90dpEyHkY9TVX5Xnc3ffE4l+/e5XZ2gaZwGE3TsB2HeSdHWBF0GRG+erBCIxqV66+tNzqlQiF+2j/Ay61t9JsmSuHNe54Hnsez8QS96TRhrfIjdiYSRAHHtvE8DykluqZxON3C/qY0L7XMcXbiDn+dngqsUoJEWy/Yu/0DvN3bh64oJYc5UkrsfJ6YYVQF8yVtmmTyeVzXJVSU1MOKwuF0mp5IhIiqMjJ5LzBwBRXmDYmhKLzXP8CbPb0YFY7jpJSoUOKKQYk6pmmr1i6v6IVgwDR5p6+fQ+n0uizXMNzzTU283tlFTFu3Rzckz8TjnOzuoaVKHbppcKaqcry9g1Zdr35uIQRSCBbqTNRLjvOI9Su90ANNqa2F64+YvJAKdhEhBKqqsriyglPjO5mzHpBzXVRVDS7SwzpDzc1rp19bAtdnRkjXqOaFEBgFF7o+m8GuUhjM2zb3l5aJ6zqaptW03s5ojLYAj9kw3J5YHL3GW/Yt1x6NkluxuJXJkC1z0amlJaZmZkgpColIBEWprUZPJEJfJLJ+OFMJfntdhkGoxhuWUuJ5Hq7r0hkOYypgOfm1Mst2HDw3T0xVCCsKjuNUjJaV0k86FK5ZoTyS58bt3JlBPXzqoGnycUAHfnFmhkxAoJBS4jkOSi7HNxJJdqXTtJZZRtc0OpMpnJjLVwvzXJqY4FY+j6aHUQK8QgI3H6wEwh00V/vEcTt3Zg3Okt4HwKl0DZc7PzXJ+anJ6hW569LkebzbP8C3OjowAr4lTVUZTKVRVI2L164ycuM+jqYhaugQWBQU1hZ4Vt3SP9QUYgPjOs9Dd13eG9jBW0/vIhIK1dxPCMGOZJJ3dj7NoXgC6boNtV2V9ivmWfOXyXz+OqzOxxoV32rPx+L8aGCAeIMJd29zMyf7B2hRFPDWV0n6evscJXBzrnsEoE1T12W1CHC8q4s2M7ou5b7d2sr+xPqt5+vtc5TA+TNoIQT7DL1R09Gv6xxqa1+3S7WaJkda29E8DxqE22c8HFIWz9JLUsE1yzoAsLuBZCmlBM+jTzdINZhkK7VBbZrWMJyvr69/RTh/3iWEYDgWbQhwTzKJscFiujcWoy9iQqG/q0eGY9E1q5XP6x4pDa5a1iBAi6ahNuCWXdEo4Q2EcYCUrpNuIBipBT2L9Q5sVkeGhm/olz45Pajr759IJuqa8ghV5WImQ8ayGnap0npJ4aZtQ1kDXE1OJBOFpG2frnTboeoOP/vssqcJIeoZFUvPA9dd/V9KWA+fWIUTqloXnD9CdqSUvz5wSGnomOG6baf3GsZcVFU4GosFDiCFoiCF2JTrSPVY7GgstjYbv27b6Ya7gvNDw/NXLes5gJSmcrTGxEUIsSn/6gErnokHXdsI7DVGhoav+OE1pakci8d5nHIsHl8Du2ZZB2pd1/j/vofiy7a9QVQs2/LuV5mbbs9be8WyLe9bVrDk9rspWy3wPCl3nLe1/BeqGe4pkvkIHAAAAABJRU5ErkJggg==";

                    if (message.Chat.Type == TChatType.chatTypeDialog)
                        avatar = "data:image/jpeg;base64," + Serialize((new FileInfo(Path.GetTempFileName()).Directory).FullName + "\\" + this.message.Sender.Handle + rand + ".jpg");

                    webControl1.ExecuteJavascript("notify('" + this.Title.CleanForJavascript() + "', '" + this.HtmlDescription.CleanForJavascript() + "', '" + avatar + "');");
                }
                catch { }
            }
        }

        public static string Serialize(string fileName)
        {
            using (FileStream reader = new FileStream(fileName, FileMode.Open))
            {
                byte[] buffer = new byte[reader.Length];
                reader.Read(buffer, 0, (int)reader.Length);
                return Convert.ToBase64String(buffer);
            }
        }

        private static string GetBrowserPath()
        {
            string browser = string.Empty;
            RegistryKey key = null;

            try
            {
                // try location of default browser path in XP
                key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

                // try location of default browser path in Vista
                if (key == null)
                {
                    key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http", false); ;
                }

                if (key != null)
                {
                    //trim off quotes
                    browser = key.GetValue(null).ToString().ToLower().Replace("\"", "");
                    if (!browser.EndsWith("exe"))
                    {
                        //get rid of everything after the ".exe"
                        browser = browser.Substring(0, browser.LastIndexOf(".exe") + 4);
                    }

                    key.Close();
                }
            }
            catch
            {
                return string.Empty;
            }

            return browser;
        }

        private void Notification_Shown(object sender, EventArgs e)
        {
            SetForegroundWindow(owner);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            CloseNotification();
        }
    }
}
