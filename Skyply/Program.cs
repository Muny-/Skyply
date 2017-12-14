using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Skyply
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
            {
                if (args[0] == "updated")
                {
                    try
                    {
                        System.IO.File.Delete(args[1]);    
                    }
                    catch
                    {

                    }

                    Application.Run(new MainForm(true));
                }
                else
                    Application.Run(new MainForm(args[0]));
            }
            else
                Application.Run(new MainForm());
        }
    }
}
