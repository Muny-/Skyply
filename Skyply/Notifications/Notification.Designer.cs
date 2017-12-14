namespace Skyply
{
    partial class Notification
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Awesomium.Core.WebPreferences webPreferences1 = new Awesomium.Core.WebPreferences(true);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Notification));
            this.webControl1 = new Awesomium.Windows.Forms.WebControl(this.components);
            this.showNotificationTimer = new System.Windows.Forms.Timer(this.components);
            this.hideNotificationTimer = new System.Windows.Forms.Timer(this.components);
            this.moveTimer = new System.Windows.Forms.Timer(this.components);
            this.CloseNotificationTimer = new System.Windows.Forms.Timer(this.components);
            this.webSessionProvider1 = new Awesomium.Windows.Forms.WebSessionProvider(this.components);
            this.SuspendLayout();
            // 
            // webControl1
            // 
            this.webControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webControl1.Location = new System.Drawing.Point(0, 0);
            this.webControl1.Size = new System.Drawing.Size(322, 123);
            this.webControl1.Source = new System.Uri("asset://ui/notification.html", System.UriKind.Absolute);
            this.webControl1.TabIndex = 0;
            this.webControl1.DocumentReady += new Awesomium.Core.UrlEventHandler(this.Awesomium_Windows_Forms_WebControl_DocumentReady);
            this.webControl1.MouseEnter += new System.EventHandler(this.Awesomium_Windows_Forms_WebControl_MouseEnter);
            this.webControl1.MouseLeave += new System.EventHandler(this.Awesomium_Windows_Forms_WebControl_MouseLeave);
            // 
            // showNotificationTimer
            // 
            this.showNotificationTimer.Enabled = true;
            this.showNotificationTimer.Interval = 1;
            this.showNotificationTimer.Tick += new System.EventHandler(this.showNotificationTimer_Tick);
            // 
            // hideNotificationTimer
            // 
            this.hideNotificationTimer.Interval = 1;
            this.hideNotificationTimer.Tick += new System.EventHandler(this.hideNotificationTimer_Tick);
            // 
            // moveTimer
            // 
            this.moveTimer.Interval = 1;
            this.moveTimer.Tick += new System.EventHandler(this.moveTimer_Tick);
            // 
            // CloseNotificationTimer
            // 
            this.CloseNotificationTimer.Enabled = true;
            this.CloseNotificationTimer.Interval = 8000;
            this.CloseNotificationTimer.Tick += new System.EventHandler(this.CloseNotificationTimer_Tick);
            // 
            // webSessionProvider1
            // 
            webPreferences1.EnableGPUAcceleration = true;
            webPreferences1.FileAccessFromFileURL = true;
            webPreferences1.UniversalAccessFromFileURL = true;
            webPreferences1.WebSecurity = false;
            this.webSessionProvider1.Preferences = webPreferences1;
            this.webSessionProvider1.Views.Add(this.webControl1);
            // 
            // Notification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 123);
            this.ControlBox = false;
            this.Controls.Add(this.webControl1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Notification";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Load += new System.EventHandler(this.Notification_Load);
            this.Shown += new System.EventHandler(this.Notification_Shown);
            this.MouseEnter += new System.EventHandler(this.Awesomium_Windows_Forms_WebControl_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.Awesomium_Windows_Forms_WebControl_MouseLeave);
            this.ResumeLayout(false);

        }

        #endregion

        private Awesomium.Windows.Forms.WebControl webControl1;
        private System.Windows.Forms.Timer showNotificationTimer;
        private System.Windows.Forms.Timer hideNotificationTimer;
        private System.Windows.Forms.Timer moveTimer;
        private System.Windows.Forms.Timer CloseNotificationTimer;
        private Awesomium.Windows.Forms.WebSessionProvider webSessionProvider1;
    }
}