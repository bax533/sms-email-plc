using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SMS_EMAIL_PLC
{
    public partial class Driver_Window : Window
    {

        DispatcherTimer timer = new DispatcherTimer();

        public Driver_Window()
        {
            InitializeComponent();
           // SetTimer(1000);
        }

        private void Save(Object sender, EventArgs e)
        {
            if (Singleton.Instance.plc_manager.connected)
            {
                try
                {
                    string dbw2 = DBW2_changeBox.Text;
                    int val_dbw2 = Int16.Parse(dbw2);
                    Singleton.Instance.plc_manager.Set_Int_Value("DB80.DBW2", val_dbw2);                    
                }
                catch(Exception ex)
                { }

                try
                {
                    string dbw0 = DBW0_changeBox.Text;
                    int val_dbw0 = Int16.Parse(dbw0);
                    Singleton.Instance.plc_manager.Set_Int_Value("DB80.DBW0", val_dbw0);
                }
                catch(Exception ex)
                { }
            }
        }

        private void SetTimer(int interval)
        {
            timer.Interval = new TimeSpan(0, 0, 1);
            //timer.Tick += new EventHandler(OnTimedEvent);
            timer.Start();
        }

        public void OnTimedEvent()
        {
            if(Singleton.Instance.plc_manager.connected)
            { 
                DBW0_box.Text = Singleton.Instance.plc_manager.Get_Int_Value("DB80.DBW0").ToString();
                DBW2_box.Text = Singleton.Instance.plc_manager.Get_Int_Value("DB80.DBW2").ToString();
            }
            
        }


        private void DriverWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Singleton.Instance.driver_window.Visibility = Visibility.Collapsed;
        }

        public void Set_0(string x)
        {
            try
            {
                DBW0_box.Text = x;
            }
            catch(Exception ex)
            { }
        }

        public void Set_2(string x)
        {
            try
            {
                DBW2_box.Text = x;
            }
            catch(Exception ex)
            { }
        }
    }
}
