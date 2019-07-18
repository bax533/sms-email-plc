using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using GsmComm.GsmCommunication;
using GsmComm.PduConverter;

namespace SMS_EMAIL_PLC
{
    class SMS_Manager
    {
        private readonly string PIN = "0410";
        private readonly string port = "COM5";

        DispatcherTimer timer = new DispatcherTimer();
        GsmCommMain comm = new GsmCommMain("COM5", 9600, 300);

        public SMS_Manager()
        {
            timer.Interval = new TimeSpan(0, 0, 30);
            timer.Tick += new EventHandler(OnTimedEvent);
            timer.Start();
        }

        public void Send(string message, string nr)
        {
            
            SmsSubmitPdu pdu = new SmsSubmitPdu(message, nr, "");

            try
            {
                comm.Open();

                if (!comm.IsConnected())
                {
                    System.Windows.MessageBox.Show("błąd portu");
                    return;
                }

                /*if (comm.GetPinStatus() != PinStatus.Ready) {
                    comm.EnterPin(PIN);
                    Thread.Sleep(1000);
                }*/

                comm.SendMessage(pdu);
                Thread.Sleep(1000);

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
            //System.Windows.MessageBox.Show("wysyłam " + message + "na numer: " + nr);
        }

        private void OnTimedEvent(object sender, EventArgs e)
        {
            if (Check_Connection())
            {
                Singleton.Instance.main_window.sms_status_text.Text = "GOTOWY";
                Singleton.Instance.main_window.sms_status_text.Background = Brushes.LawnGreen;
                Singleton.Instance.main_window.sms_status_text.Foreground = Brushes.Black;
            }
            else
            {
                Singleton.Instance.main_window.sms_status_text.Text = "BRAK";
                Singleton.Instance.main_window.sms_status_text.Background = Brushes.Red;
                Singleton.Instance.main_window.sms_status_text.Foreground = Brushes.Black;
            }
        }

        public bool Check_Connection()
        {
            if (!comm.IsConnected())
            {
                try
                {
                    comm.Open();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else return true;
        }

        public void Close()
        {
            comm.Close();
        }
    }
}
