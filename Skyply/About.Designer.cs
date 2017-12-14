namespace Skyply
{
    partial class About
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.webControl1 = new Awesomium.Windows.Forms.WebControl(this.components);
            this.webSessionProvider1 = new Awesomium.Windows.Forms.WebSessionProvider(this.components);
            this.SuspendLayout();
            // 
            // webControl1
            // 
            this.webControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webControl1.Location = new System.Drawing.Point(0, 0);
            this.webControl1.Size = new System.Drawing.Size(536, 263);
            this.webControl1.Source = new System.Uri("asset://ui/about.html", System.UriKind.Absolute);
            this.webControl1.TabIndex = 0;
            this.webControl1.DocumentReady += new Awesomium.Core.UrlEventHandler(this.Awesomium_Windows_Forms_WebControl_DocumentReady);
            // 
            // webSessionProvider1
            // 
            webPreferences1.CanScriptsAccessClipboard = true;
            webPreferences1.EnableGPUAcceleration = true;
            webPreferences1.FileAccessFromFileURL = true;
            webPreferences1.SmoothScrolling = true;
            webPreferences1.UniversalAccessFromFileURL = true;
            webPreferences1.WebSecurity = false;
            this.webSessionProvider1.Preferences = webPreferences1;
            this.webSessionProvider1.Views.Add(this.webControl1);
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 263);
            this.Controls.Add(this.webControl1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About Skyply";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.About_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Awesomium.Windows.Forms.WebControl webControl1;
        private Awesomium.Windows.Forms.WebSessionProvider webSessionProvider1;
    }
}