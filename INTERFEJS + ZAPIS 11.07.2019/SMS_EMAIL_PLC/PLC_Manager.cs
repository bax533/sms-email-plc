using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S7;
using S7.Net;

namespace SMS_EMAIL_PLC
{
    class PLC_Manager
    {
        public bool connected;
        public Plc plc;

        public bool Load_Plc(string Cputype, string ip, int rack, int slot)
        {
            CpuType nowy = new CpuType();
            bool unspupported = false;
            switch (Cputype)
            {
                case "S7300":
                case "S7-300":
                    nowy = CpuType.S7300;
                    break;
                case "S7200":
                case "S7-200":
                    nowy = CpuType.S7200;
                    break;
                case "S7400":
                case "S7-400":
                    nowy = CpuType.S7400;
                    break;
                case "S71200":
                case "S7-1200":
                    nowy = CpuType.S71200;
                    break;
                case "S71500":
                case "S7-1500":
                    nowy = CpuType.S71500;
                    break;
                default:
                    Console.WriteLine("unsupported");
                    unspupported = true;
                    break;
            }
            plc = new Plc(nowy, ip, (short)rack, (short)slot);

            if (!unspupported)
            {
                try
                {
                    plc.Open();
                }
                catch (Exception ex)
                {
                    /*if (ex is S7.Net.PlcException)
                        System.Windows.MessageBox.Show("invalid IP");
                    else
                        System.Windows.MessageBox.Show(ex.Message);*/
                    return false;
                }
                if (plc.IsConnected) return true;
                else return false;
            }
            else return false;
        }

        public int Get_Int_Value(string name)
        {
            return (UInt16)plc.Read(name);
        }

        public void Set_Int_Value(string name, int val)
        {
            plc.Write(name, (UInt16)val);
        }
    }
}
