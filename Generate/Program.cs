using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Generate
{
    static class Program
    {
        public static NpcHelpers MappyHelper;
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MappyHelper = new NpcHelpers();
          
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static string GetMappyFileName()
        {
            return @"H:\xivapi_mappy_2019-12-12-06-49-19.csv";
        }

        public static string DownloadNewVersion()
        {
            string filename = $@"G:\MappyFiles\{DateTime.Today.ToString("dd-MM-yyyy")}.cvs";
            
            using (var client = new WebClient())
            {
                client.DownloadFile("https://xivapi.com/download?data=memory_data", filename);
            }

            return filename;
        }
    }
}