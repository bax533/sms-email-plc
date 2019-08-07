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
        private readonly int baudrate = 9600;
        private readonly int timeout = 300;
        public string port = "COM5";

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
                    Singleton.Show_MessageBox("błąd portu");
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
                Singleton.Show_MessageBox(ex.Message);
                return;
            }
            //Singleton.Show_MessageBox("wysyłam " + message + "na numer: " + nr);
        }

        public void Check_Connection( string port )
        {

            if (port.Length < 4)
                return;

            if (comm.IsOpen())
                comm.Close();
            comm = new GsmCommMain(port, baudrate, timeout);

            if (!comm.IsConnected())
            {
                try
                {
                    comm.Open();
                    connected = true;
                    return;
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
