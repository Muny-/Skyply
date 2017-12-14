using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Elysium;
using System.Windows.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Net;
using System.IO;
using System.IO.Compression;
using Microsoft.Win32;
using IWshRuntimeLibrary;

namespace SkyplySetup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Elysium.Controls.Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (SkyplySetup.Properties.Settings.Default.isUninstalling)
            {
                agreementsCanvas.Visibility = System.Windows.Visibility.Hidden;
                uninstallCanvas.Visibility = System.Windows.Visibility.Visible;
                uninstallationTextBox.Text = SkyplySetup.Properties.Settings.Default.installationLocation;
            }
            //this.VisualTextRenderingMode = TextRenderingMode.Grayscale;
        }

        Thread installThread;

        public void SetImages(BitmapImage image)
        {
            
        }

        public delegate void SetImagesDelegate(BitmapImage img);

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }

        private static IntPtr LogonUser()
        {
            IntPtr accountToken = WindowsIdentity.GetCurrent().Token;

            return accountToken;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            agreementsCanvas.Visibility = System.Windows.Visibility.Hidden;
            installingCanvas.Visibility = System.Windows.Visibility.Visible;

            installThread = new Thread(Install);
            installThread.Start();
        }

        string temp_zip_file = "";

        WebClient installWC = new WebClient();

        private void Install()
        {
            installWC.DownloadProgressChanged += wc_DownloadProgressChanged;
            installWC.DownloadFileCompleted += wc_DownloadFileCompleted;
            temp_zip_file = System.IO.Path.GetTempFileName() + ".Skyply_install";
            installWC.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            installWC.DownloadFileAsync(new Uri("http://cdn.azuru.me/apps/skyply/install.zip"), temp_zip_file);
        }

        void wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {
                string folder = "";
                this.Dispatcher.Invoke(delegate()
                {
                    folder = installationTextBox.Text;
                    currentStatusLabel.Content = "Creating installation folder...";
                });
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                this.Dispatcher.Invoke(delegate()
                {
                    currentStatusLabel.Content = "Extracting...";
                });
                try
                {
                    ZipFile.ExtractToDirectory(temp_zip_file, folder);
                }
                catch { }
                this.Dispatcher.Invoke(delegate()
                {
                    currentStatusLabel.Content = "Deleting temporary files...";
                });
                System.IO.File.Delete(temp_zip_file);
                bool doCreateShortcut = false;
                this.Dispatcher.Invoke(delegate()
                {
                    if (shortCutCheckBox.IsChecked.Value)
                    {
                        currentStatusLabel.Content = "Creating desktop shortcut...";
                        doCreateShortcut = true;
                    }
                });
                if (doCreateShortcut)
                    createShortcut(folder + "\\Skyply.exe");
                this.Dispatcher.Invoke(delegate()
                {
                    currentStatusLabel.Content = "Creating uninstaller...";
                });
                CreateUninstaller(doCreateShortcut);

                this.Dispatcher.Invoke(delegate()
                {
                    currentStatusLabel.Content = "Creating startup registry key....";
                });
                RegistryKey Key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                Key.SetValue("Skyply", folder + "\\Skyply.exe");
                this.Dispatcher.Invoke(delegate()
                {
                    currentStatusLabel.Content = "Finishing...";
                    installingCanvas.Visibility = System.Windows.Visibility.Hidden;
                    finishedCanvas.Visibility = System.Windows.Visibility.Visible;
                });
            }
            catch { }
        }

        private void createShortcut(string app)
        {
            string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            WshShell shell = (WshShell)Activator.CreateInstance(System.Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")));
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(deskDir + "\\Skyply.lnk");
            shortcut.TargetPath = app;
            string icon = app.Replace('\\', '/');
            shortcut.Description = "Launch Skyply";
            shortcut.IconLocation = icon;
            shortcut.Save();
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            try
            {
                this.Dispatcher.Invoke(delegate()
                {
                    downloadProgress.Value = e.ProgressPercentage;
                });
            }
            catch {  }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Select a folder to install Skyply to:";
                dlg.SelectedPath = installationTextBox.Text;
                dlg.ShowNewFolderButton = true;
                DialogResult result = dlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    installationTextBox.Text = dlg.SelectedPath;
                }
            }
        }

        private void LicenseTermsLabel_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Windows.Controls.Label label = (System.Windows.Controls.Label)sender;
            label.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 20, 171, 255));
        }

        private void LicenseTermsLabel_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Windows.Controls.Label label = (System.Windows.Controls.Label)sender;
            label.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 151, 255));
        }

        private void LicenseTermsLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Label label = (System.Windows.Controls.Label)sender;
            label.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 121, 235));
        }

        private void LicenseTermsLabel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Label label = (System.Windows.Controls.Label)sender;
            label.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 151, 255));
            System.Diagnostics.Process.Start("http://azuru.me/terms/skyply");
        }

        private void PrivacyStatementLabel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Label label = (System.Windows.Controls.Label)sender;
            label.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 151, 255));
            System.Diagnostics.Process.Start("http://azuru.me/privacy/skyply");
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            installButton.IsEnabled = true;
        }

        private void agreeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            installButton.IsEnabled = false;
        }

        private void shieldImage_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            runSkyplyCheckBox.IsChecked = true;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(installationTextBox.Text + "\\Skyply.exe", "\"" + installationTextBox.Text + "\"");
            }
            catch { }
            installThread.Abort();
            installThread.Join();
            canClose = true;
            this.Close();
        }

        bool canClose = false;
        Canvas cancelledCanvas;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!canClose)
            {
                try
                {
                    e.Cancel = true;
                    try
                    {
                        if (installThread != null)
                            installThread.Suspend();

                        if (agreementsCanvas.Visibility == System.Windows.Visibility.Visible)
                            cancelledCanvas = agreementsCanvas;
                        else if (installingCanvas.Visibility == System.Windows.Visibility.Visible)
                            cancelledCanvas = installingCanvas;
                        else if (finishedCanvas.Visibility == System.Windows.Visibility.Visible)
                            cancelledCanvas = finishedCanvas;

                        cancelledCanvas.Visibility = System.Windows.Visibility.Hidden;

                        askCancelCanvas.Visibility = System.Windows.Visibility.Visible;
                    }
                    catch {
                        e.Cancel = false;
                    }
                }
                catch
                {
                    canClose = true;
                    this.Close();
                }
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (installThread != null)
            {
                installThread.Abort();
                installThread.Join();
            }
            SkyplySetup.Properties.Settings.Default.installationLocation = installationTextBox.Text;
            SkyplySetup.Properties.Settings.Default.Save();
            yes.IsEnabled = false;
            no.IsEnabled = false;
            new Thread(delegate()
            {
                Uninstall();
                canClose = true;
                this.Dispatcher.Invoke(delegate()
                {
                    this.Close();
                });
            }).Start();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            askCancelCanvas.Visibility = System.Windows.Visibility.Hidden;
            cancelledCanvas.Visibility = System.Windows.Visibility.Visible;
            if (installThread != null)
                installThread.Resume();
        }

        private void CreateUninstaller(bool doCreateShortcut)
        {
            string folder = "";
            this.Dispatcher.Invoke(delegate()
            {
                folder = installationTextBox.Text;
            });

            using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(
                         @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true))
            {
                if (parent == null)
                {
                    throw new Exception("Uninstall registry key not found.");
                }
                try
                {
                    RegistryKey key = null;

                    try
                    {
                        key = parent.OpenSubKey("Skyply", true) ??
                              parent.CreateSubKey("Skyply");

                        if (key == null)
                        {
                            throw new Exception("Unable to create uninstaller");
                        }
                        WebClient wc = new WebClient();
                        string version = wc.DownloadString("http://picbox.us/program/skyply/version.php");
                        key.SetValue("DisplayName", "Skyply");
                        key.SetValue("ApplicationVersion", version);
                        key.SetValue("Publisher", "Azuru Networks");
                        key.SetValue("DisplayIcon", folder + "\\Skyply.exe");
                        key.SetValue("DisplayVersion", version);
                        key.SetValue("URLInfoAbout", "http://azuru.me/skyply");
                        key.SetValue("Contact", "support@azuru.me");
                        key.SetValue("InstallLocation", folder);
                        key.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"));
                        key.SetValue("CreatedShortcut", doCreateShortcut);
                        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Azuru_Networks\\SkyplyUninstaller");
                        string location = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Azuru_Networks\\SkyplyUninstaller\\SkyplySetup.exe";
                        if (System.IO.File.Exists(location))
                            System.IO.File.Delete(location);
                        try
                        {
                            System.IO.File.Copy(System.Reflection.Assembly.GetExecutingAssembly().Location, location);
                            key.SetValue("UninstallString", "\"" + location + "\" uninstall \"" + folder + "\"");
                        }
                        catch {
                            this.Dispatcher.Invoke(delegate()
                            {
                                currentStatusLabel.Content = "Failed to create uninstaller!";
                            });
                            Thread.Sleep(2000);
                        }
                    }
                    finally
                    {
                        if (key != null)
                        {
                            key.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Exception: " + ex.Message);
                    /*throw new Exception(
                        "An error occurred writing uninstall information to the registry.  The service is fully installed but can only be uninstalled manually through the command line.",
                        ex);*/
                }
            }
        }

        Thread uninstallThread;

        private void uninstallButton_Click(object sender, RoutedEventArgs e)
        {
            uninstallCanvas.Visibility = System.Windows.Visibility.Hidden;
            uninstallingCanvas.Visibility = System.Windows.Visibility.Visible;
            uninstallThread = new Thread(Uninstall);
            uninstallThread.Start();
        }

        private void Uninstall()
        {
            this.Dispatcher.Invoke(delegate()
            {
                downloadProgress.Value = 100;
            });
            if (System.Diagnostics.Process.GetProcessesByName("Skyply").Length > 0)
            {
                this.Dispatcher.Invoke(delegate()
                {
                    uninstallCurrentStatusLabel.Content = "Stopping Skyply instances...";
                });
                foreach (System.Diagnostics.Process proc in System.Diagnostics.Process.GetProcessesByName("Skyply"))
                {
                    proc.CloseMainWindow();
                    proc.Kill();
                    proc.WaitForExit();
                }
                this.Dispatcher.Invoke(delegate()
                {
                    uninstallCurrentStatusLabel.Content = "Deleting files...";
                });
            }
            bool tryDeleteAgain = false;
            try
            {
                Directory.Delete(SkyplySetup.Properties.Settings.Default.installationLocation, true);
            }
            catch { tryDeleteAgain = true; }
            this.Dispatcher.Invoke(delegate()
            {
                downloadProgress.Value = 85.5;
                uninstallCurrentStatusLabel.Content = "Deleting registry keys...";
            });
            bool deleteShortcut = false;
            try
            {
                using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true))
                {
                    deleteShortcut = Convert.ToBoolean(parent.OpenSubKey("Skyply").GetValue("CreatedShortcut", false));
                    this.Dispatcher.Invoke(delegate()
                    {
                        downloadProgress.Value = 71;
                    });
                    parent.DeleteSubKey("Skyply", false);
                    this.Dispatcher.Invoke(delegate()
                    {
                        downloadProgress.Value = 56.5;
                    });
                }
                RegistryKey Key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                Key.DeleteValue("Skyply", false);
                this.Dispatcher.Invoke(delegate()
                {
                    downloadProgress.Value = 42;
                });
            }
            catch { }
            if (deleteShortcut)
            {
                this.Dispatcher.Invoke(delegate()
                {
                    uninstallCurrentStatusLabel.Content = "Deleting desktop shortcut...";
                });
                try
                {
                    System.IO.File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Skyply.lnk");
                }
                catch { }
                this.Dispatcher.Invoke(delegate()
                {
                    downloadProgress.Value = 27.5;
                });
            }
            if (tryDeleteAgain)
            {
                Thread.Sleep(1000);
                try
                {
                    Directory.Delete(SkyplySetup.Properties.Settings.Default.installationLocation, true);
                    this.Dispatcher.Invoke(delegate()
                    {
                        downloadProgress.Value = 13;
                    });
                }
                catch { }
            }
            this.Dispatcher.Invoke(delegate()
            {
                downloadProgress.Value = 0;
                uninstallCurrentStatusLabel.Content = "Finishing...";
                uninstallingCanvas.Visibility = System.Windows.Visibility.Hidden;
                installingCanvas.Visibility = System.Windows.Visibility.Hidden;
                finishedCanvas.Visibility = System.Windows.Visibility.Hidden;
                uninstalledCanvas.Visibility = System.Windows.Visibility.Visible;
            });
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            canClose = true;
            this.Close();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            if (installThread != null)
            {
                installThread.Abort();
                installThread.Join();
                installThread = null;
            }
            installWC.CancelAsync();
            installWC.Dispose();
            installWC = null;
            finishedCanvas.Width = 0;
            finishedCanvas.Height = 0;
            finishedCanvas.IsEnabled = false;
            finishedCanvas.Opacity = 0;
            SkyplySetup.Properties.Settings.Default.installationLocation = installationTextBox.Text;
            SkyplySetup.Properties.Settings.Default.Save();
            uninstallationCompleteLabel.Content = "Installation Cancelled";
            uninstallationDescriptionLabel.Content = "The Skyply setup process has been cancelled and all changes made have\r\nbeen rolled back.";
            new Thread(delegate()
            {
                Uninstall();
                canClose = true;
            }).Start();
        }
    }
}
