using System;
using System.Threading;
using System.Windows.Forms;

namespace Keyword_Scraper_By_MasterC
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {      
            bool runed;
            using (Mutex mtx = new Mutex(true, "KeywordScraperDarkChiper", out runed))
            {
                if (!runed)
                {
                    MessageBox.Show("The Program is Running");
                    return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                FrmJoin frmJoinx = new FrmJoin();
                Application.Run(frmJoinx);

                if (frmJoinx.DialogResult == DialogResult.OK)
                {
                    Application.Run(new Form1());
                }
            }
        }
    }
}
