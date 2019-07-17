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


        public SMS_Manager()
        {
            timer.Interval = new TimeSpan(0, 0, 30);
            timer.Tick += new EventHandler(OnTimedEvent);
            timer.Start();
        }

        public void Send(string message, string nr)
        {
            GsmCommMain comm = new GsmCommMain(port, 9600, 300);
            SmsSubmitPdu pdu = new SmsSubmitPdu(message, nr, "");

            try
            {
                comm.Open();

                if (!comm.IsConnected())
                {
                    System.Windows.MessageBox.Show("błąd portu");
                    return;
                }

                if (comm.GetPinStatus() != PinStatus.Ready) {
                    comm.EnterPin(PIN);
                    Thread.Sleep(1000);
                }

                comm.SendMessage(pdu);
                //Thread.Sleep(1000);

                comm.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                if (comm.IsConnected())
                    comm.Close();
                return;
            }
            //System.Windows.MessageBox.Show("wysyłam " + message + "na numer: " + nr);
        }

        private void OnTimedEvent(object sender, EventArgs e)
        {
            if (Check_Connection())
            {
                Singleton.Instance.main_window.sms_status_text.Text = "GOTOWY";
                Singleton.Instance.main_window.sms_status_text.Background = Brushes.Green;
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
            GsmCommMain comm = new GsmCommMain(port, 9600, 300);
            try
            {
                comm.Open();
                if (comm.GetPinStatus() != PinStatus.Ready)
                    comm.EnterPin(PIN);

                if (comm.IsConnected())
                {
                    comm.Close();
                    return true;
                }
                else
                {
                    if (comm.IsOpen())
                        comm.Close();
                    return false;
                }
            }
            catch(Exception ex)
            {
                if (comm.IsOpen())
                    comm.Close();
                return false;
            }
        }
    }
}
