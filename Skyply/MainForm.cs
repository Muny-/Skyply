using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Awesomium.Core;
using SKYPE4COMLib;
using System.Net;
using System.IO;

namespace Skyply
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            Init(true);
        }

        public MainForm(string InstallLocation)
        {
            Skyply.Properties.Settings.Default.InstallLocation = InstallLocation;
            Skyply.Properties.Settings.Default.Save();
            Init(false);
            Welcome welc = new Welcome();
            welc.Show();
        }

        public MainForm(bool wasUpdated)
        {
            Init(false);
            Changelog cl = new Changelog();
            cl.Show();
        }

        public void Init(bool cu)
        {
            instance = this;
            WebConfig wc = new WebConfig();
            wc.RemoteDebuggingPort = 7777;
            wc.AutoUpdatePeriod = 1;
            wc.LogLevel = LogLevel.None;
            WebCore.Initialize(wc);
            InitializeComponent();
            if (cu)
                CheckForUpdate(false);

            Directory.SetCurrentDirectory(Skyply.Properties.Settings.Default.InstallLocation);
        }

        public Skype skype = new Skype();
        public NotificationManager notificationManager;
        About about = new About();
        public static MainForm instance;

        public const double Version = 1.1;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Visible = false;
            notificationManager = new NotificationManager(this);
            skype.MessageStatus += skype_MessageStatus;
            skype.Attach(8, false);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        void skype_MessageStatus(ChatMessage pMessage, TChatMessageStatus Status)
        {
            if (Status == TChatMessageStatus.cmsReceived && skype.CurrentUserStatus != TUserStatus.cusDoNotDisturb && System.Diagnostics.Process.GetProcessesByName("Skype")[0].MainWindowHandle != GetForegroundWindow())
            {
                notificationManager.AddNotification(pMessage);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (about.IsDisposed)
                about = new About();

            if (!about.Visible)
                about.Show();
        }

        private void updateCheck_Tick(object sender, EventArgs e)
        {
            CheckForUpdate(false);
        }

        public void CheckForUpdate(bool userInitiated)
        {
            new System.Threading.Thread(delegate()
            {
                WebClient wc = new WebClient();
                double latest_ver = Convert.ToDouble(wc.DownloadString("http://picbox.us/program/skyply/version.php"));

                if (latest_ver > Version)
                {
                    UpdateProg();
                }
                else
                {

                }
            }).Start();
        }

        public void UpdateProg()
        {
            try
            {
                WebClient wc = new WebClient();
                DirectoryInfo di = new FileInfo(Path.GetTempFileName()).Directory;
                wc.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                wc.DownloadFile("http://cdn.azuru.me/apps/skyply/SkyplyUpdate.exe", di.FullName + "/SkyplyUpdate.exe");
                System.Diagnostics.Process.Start(di.FullName + "/SkyplyUpdate.exe", "\"" + Skyply.Properties.Settings.Default.InstallLocation + "\" " + System.Diagnostics.Process.GetCurrentProcess().Id.ToString());
            }
            catch
            {
                MessageBox.Show("The update process was cancelled!");
            }
        }
    }
}
