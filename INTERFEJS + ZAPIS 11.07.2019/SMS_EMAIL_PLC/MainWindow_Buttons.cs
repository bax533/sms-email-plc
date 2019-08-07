using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Microsoft.Win32;

namespace SMS_EMAIL_PLC
{
    public class My_Toolbar : StackPanel
    {

        public My_Toolbar()
        {

        }

        public void Users_dbg()
        {
            string ret = "";
            for (int i = 0; i < Singleton.Instance.users.Count; i++)
            {
                ret += Singleton.Instance.users[i].ToString() + "\n";
            }
            Singleton.Show_MessageBox(ret);
        }


        public void Config_dbg()
        {
            string ret = "";
            foreach (User user in Singleton.Instance.users)
            {
                ret += user.Get_ID() + ":\n";
                if (Singleton.Instance.configuration.ContainsKey(user.Get_ID()))
                {
                    foreach (KeyValuePair<string, Configuration> config in Singleton.Instance.configuration[user.Get_ID()])
                    {
                        bool su, sd, eu, ed;
                        su = config.Value.sms_up;
                        sd = config.Value.sms_down;
                        eu = config.Value.email_up;
                        ed = config.Value.email_down;
                        ret += su + " " + sd + " " + eu + " " + ed + "\n";
                    }
                }
                else
                    Singleton.Instance.configuration[user.Get_ID()] = new Dictionary<string, Configuration>();
                ret += "--------------\n";
            }
            Singleton.Show_MessageBox(ret);
        }

        private void dbg_Click(Object sender, EventArgs e)
        {
            Users_dbg();
            Config_dbg();
        }

    }
}
