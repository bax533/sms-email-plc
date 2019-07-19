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

        GsmCommMain comm = new GsmCommMain("COM5", 9600, 300);

        public bool connected = false;

        public SMS_Manager()
        {
        }

        public void Send(string message, string nr)
        {
            
            SmsSubmitPdu pdu = new SmsSubmitPdu(message, nr, "");

            try
            {
                if(!comm.IsOpen())
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

        public void Check_Connection()
        {
            if (!comm.IsConnected())
            {
                try
                {
                    comm.Open();
                    connected = true;
                }
                catch (Exception ex)
                {
                    connected = false;
                }
            }
            else
                connected = true;
        }

        public void Close()
        {
            comm.Close();
        }
    }
}
