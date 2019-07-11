using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMS_EMAIL_PLC
{
    class SMS_Manager
    {
        private char CtrlZ = (char)26;
        private char CR = (char)13;

        public bool Send(string message, List<string> nrs, SerialPort port, string PIN)
        {
            try
            {
                if (PIN != "*")
                {
                    port.WriteLine($"AT+CPIN=\"{PIN}\"" + "\r");
                    Thread.Sleep(1000);
                }
                port.WriteLine("AT+CMGF=1" + "\r");
                Thread.Sleep(1000);
                foreach (string nr in nrs)
                {
                    port.WriteLine($"AT+CMGS=\"{nr}\"" + CR + message + CtrlZ + "\r");
                    Thread.Sleep(1000);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Check_Response(SerialPort port)
        {
            string response = port.ReadExisting();
            char poprz = response[0];
            foreach (char c in response)
            {
                if (poprz == 'O' && c == 'K')
                    return true;
                poprz = c;
            }
            return false;
        }

        public bool Check_Status(SerialPort port)
        {
            port.WriteLine("AT" + "\r");
            return Check_Response(port);
        }

        public bool Check_PIN(SerialPort port)
        {
            port.WriteLine("AT+CPIN?" + "\r");
            return Check_Response(port);
        }
    }
}
