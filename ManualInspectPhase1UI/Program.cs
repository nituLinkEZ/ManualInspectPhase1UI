using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManualInspectPhase1UI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            GetServerIpAndPort();
            Application.Run(new MATS_Login());
        }
        private static void GetServerIpAndPort()
        {
            string[] str = File.ReadAllLines("setup.cfg");
            if (str[0].Contains("IP_ADDRESS:"))
            {
                string[] data = str[0].Split(':');
                GlobalVar.SERVER_IP_ADDRESS = data[1];
            }
            if (str[1].Contains("PORT"))
            {
                string[] data = str[1].Split(':');
                GlobalVar.SERVER_PORT = data[1];
            }
            if (str.Length > 2 && str[2].Contains("TEST_DELAY"))
            {
                string[] data = str[2].Split(':');
                try
                {
                    GlobalVar.TEST_DELAY = int.Parse(data[1]);
                }
                catch (Exception)
                {
                    MessageBox.Show("Please Verify the setup.cfg For the Test Delay");
                }
            }
        }
    }
}
