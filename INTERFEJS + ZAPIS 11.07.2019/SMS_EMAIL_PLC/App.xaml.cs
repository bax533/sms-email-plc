using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SMS_EMAIL_PLC
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            try
            {
                RunMethod();
            }
            catch (Exception e)
            {
                string fileName = "EXCEPTION.txt";
                using (FileStream fs = File.Create(fileName))
                {
                    byte[] author = new UTF8Encoding(true).GetBytes(e.Message);
                    fs.Write(author, 0, author.Length);
                }
            }
        }

        private static void RunMethod()
        {
            var app = new App();
            app.InitializeComponent();
            Singleton.Instance.Set_Start(5000);
            app.Run();
            
        }
    }
}
