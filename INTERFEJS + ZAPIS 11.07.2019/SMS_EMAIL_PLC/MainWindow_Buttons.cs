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
        Button main_button, users_button, messages_button, configuration_button, driver_button, dbg_button;

        public My_Toolbar()
        {
            this.HorizontalAlignment = HorizontalAlignment.Center;
            int fontsize = 12;
            int height = 15;
            int width = 120;
            Thickness thickness = new Thickness(0, 3, 0, 0);

            main_button = new Button
            {
                Content = "strona główna",
                Width = width,
                FontSize = fontsize,
                Margin = thickness
            };
            main_button.Click += Main_Click;
            Children.Add(main_button);

            users_button = new Button
            {
                Content = "odbiorcy",
                Width = width,
                FontSize = fontsize,
                Margin = thickness
            };
            users_button.Click += Users_Click;
            Children.Add(users_button);
            /*messages_button = new Button
            {
                Content = "wiadomości",
                Width = 100,
                Height = height,
                FontSize = fontsize,
                Background = Brushes.Azure
            };
            messages_button.Click += Messages_Click;
            Children.Add(messages_button);*/

            configuration_button = new Button
            {
                Content = "konfiguracja",
                Width = width,
                FontSize = fontsize,
                Margin = thickness
            };
            configuration_button.Click += Configure_Click;
            Children.Add(configuration_button);
            /*driver_button = new Button
            {
                Content = "sterownik",
                Width = 100,
                Height = height,
                FontSize = fontsize,
            };
            driver_button.Click += Driver_Click;
            Children.Add(driver_button);*/

            dbg_button = new Button
            {
                Content = "debug",
                Width = width,
                FontSize = fontsize,
                Margin = thickness
            };
            dbg_button.Click += dbg_Click;
        }

        

        private void Main_Click(Object sender, RoutedEventArgs e)
        {
            Singleton.Instance.users_window.Visibility = Visibility.Collapsed;
            //Singleton.Instance.messages_window.Visibility = Visibility.Collapsed;
            Singleton.Instance.configuration_window.Visibility = Visibility.Collapsed;

            if (Singleton.Instance.main_window.Visibility != Visibility.Visible)
                Singleton.Instance.main_window.Visibility = Visibility.Visible;
            else
                Singleton.Instance.users_window.Activate();
        }

        private void Users_Click(object sender, EventArgs e)
        {
            Singleton.Instance.main_window.Visibility = Visibility.Collapsed;
            //Singleton.Instance.messages_window.Visibility = Visibility.Collapsed;
            Singleton.Instance.configuration_window.Visibility = Visibility.Collapsed;

            if (Singleton.Instance.users_window.Visibility != Visibility.Visible)
                Singleton.Instance.users_window.Visibility = Visibility.Visible;
            else
                Singleton.Instance.users_window.Activate();
        }

        /*private void Messages_Click(object sender, EventArgs e)
        {
            Singleton.Instance.main_window.Visibility = Visibility.Collapsed;
            Singleton.Instance.users_window.Visibility = Visibility.Collapsed;
            Singleton.Instance.configuration_window.Visibility = Visibility.Collapsed;

            if (Singleton.Instance.messages_window.Visibility != Visibility.Visible)
                Singleton.Instance.messages_window.Visibility = Visibility.Visible;
            else
                Singleton.Instance.messages_window.Activate();
        }*/

        private void Configure_Click(object sender, EventArgs e)
        {
            Singleton.Instance.main_window.Visibility = Visibility.Collapsed;
            Singleton.Instance.users_window.Visibility = Visibility.Collapsed;
            //Singleton.Instance.messages_window.Visibility = Visibility.Collapsed;

            if (Singleton.Instance.configuration_window.Visibility != Visibility.Visible)
                Singleton.Instance.configuration_window.Visibility = Visibility.Visible;
            else
                Singleton.Instance.configuration_window.Activate();
        }

        private void Driver_Click(Object sender, EventArgs e)
        {
            /*if (Singleton.Instance.driver_window.Visibility != Visibility.Visible)
                Singleton.Instance.driver_window.Visibility = Visibility.Visible;
            else
                Singleton.Instance.driver_window.Activate();*/
        }


        public void Users_dbg()
        {
            string ret = "";
            for (int i = 0; i < Singleton.Instance.users.Count; i++)
            {
                ret += Singleton.Instance.users[i].ToString() + "\n";
            }
            System.Windows.MessageBox.Show(ret);
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
            System.Windows.MessageBox.Show(ret);
        }

        private void dbg_Click(Object sender, EventArgs e)
        {
            Users_dbg();
            Config_dbg();
        }

    }
}
