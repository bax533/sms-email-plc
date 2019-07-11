using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_EMAIL_PLC
{
    internal class Driver
    {
        public string name;
        public int val;
        public bool already_alarmed;
        public Driver(string name, int val)
        {
            this.name = name;
            this.val = val;
            already_alarmed = false;
        }
    }
}
