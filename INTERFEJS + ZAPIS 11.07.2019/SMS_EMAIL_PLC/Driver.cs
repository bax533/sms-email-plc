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
        public bool already_acknowledged;
        public Driver(string name)
        {
            this.name = name;
            already_acknowledged = false;
        }
    }
}
