using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace SMS_EMAIL_PLC
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Singleton.Instance.main_window = this;
            Singleton.Instance.SetTimer(500);
            Driver driver1 = new Driver("DB80.DBW0", 121);
            Driver driver2 = new Driver("DB80.DBW2", 122);
            Singleton.Instance.toControl.Add(driver1);
            Singleton.Instance.toControl.Add(driver2);
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
                try
                {
                    window.Close();
                }
                catch (Exception ex)
                {
                }
            try
            {
                Singleton.Instance.sql_manager.cnn.Close();
            }
            catch(Exception ex)
            { }
            
            System.Windows.Application.Current.Shutdown();
        }

        public void Add_Lines_To_Windows(Dictionary<string, Message> msgs)
        {
            Singleton.Instance.users_window.Window_Clear();
            foreach (User user in Singleton.Instance.users)
                Singleton.Instance.users_window.Add_Line(user.Get_ID(), user.Get_Name(), user.Get_Number(), user.Get_Email());

            Singleton.Instance.messages_window.Window_Clear();

            foreach (KeyValuePair<string, Message> msg in msgs)
            {
                Singleton.Instance.messages_window.Add_Line(msg.Key, "opis");
            }

            Singleton.Instance.configuration_window.Refresh();
        }
    }
}
