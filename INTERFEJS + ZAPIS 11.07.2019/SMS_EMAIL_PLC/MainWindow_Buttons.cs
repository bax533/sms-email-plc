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
        Button main_button, users_button, messages_button, configuration_button, driver_button;

        public My_Toolbar()
        {
            this.Orientation = Orientation.Horizontal;
            main_button = new Button
            {
                Content = "strona główna",
                Width = 100,
                Height = 25,
                FontSize = 15,
                Background = Brushes.Azure
            };
            main_button.Click += Main_Click;
            Children.Add(main_button);

            users_button = new Button
            {
                Content = "użytkownicy",
                Width = 100,
                Height = 25,
                FontSize = 15,
                Background = Brushes.Azure
            };
            users_button.Click += Users_Click;
            Children.Add(users_button);

            messages_button = new Button
            {
                Content = "wiadomości",
                Width = 100,
                Height = 25,
                FontSize = 15,
                Background = Brushes.Azure
            };
            messages_button.Click += Messages_Click;
            Children.Add(messages_button);

            configuration_button = new Button
            {
                Content = "konfiguracja",
                Width = 100,
                Height = 25,
                FontSize = 15,
                Background = Brushes.Azure
            };
            configuration_button.Click += Configure_Click;
            Children.Add(configuration_button);

            driver_button = new Button
            {
                Content = "sterownik",
                Width = 100,
                Height = 25,
                FontSize = 15,
            };
            driver_button.Click += Driver_Click;
            Children.Add(driver_button);
        }

        

        private void Main_Click(Object sender, RoutedEventArgs e)
        {
            Singleton.Instance.users_window.Visibility = Visibility.Collapsed;
            Singleton.Instance.messages_window.Visibility = Visibility.Collapsed;
            Singleton.Instance.configuration_window.Visibility = Visibility.Collapsed;

            if (Singleton.Instance.main_window.Visibility != Visibility.Visible)
                Singleton.Instance.main_window.Visibility = Visibility.Visible;
            else
                Singleton.Instance.users_window.Activate();
        }

        private void Users_Click(object sender, EventArgs e)
        {
            Singleton.Instance.main_window.Visibility = Visibility.Collapsed;
            Singleton.Instance.messages_window.Visibility = Visibility.Collapsed;
            Singleton.Instance.configuration_window.Visibility = Visibility.Collapsed;

            if (Singleton.Instance.users_window.Visibility != Visibility.Visible)
                Singleton.Instance.users_window.Visibility = Visibility.Visible;
            else
                Singleton.Instance.users_window.Activate();
        }

        private void Messages_Click(object sender, EventArgs e)
        {
            Singleton.Instance.main_window.Visibility = Visibility.Collapsed;
            Singleton.Instance.users_window.Visibility = Visibility.Collapsed;
            Singleton.Instance.configuration_window.Visibility = Visibility.Collapsed;

            if (Singleton.Instance.messages_window.Visibility != Visibility.Visible)
                Singleton.Instance.messages_window.Visibility = Visibility.Visible;
            else
                Singleton.Instance.messages_window.Activate();
        }

        private void Configure_Click(object sender, EventArgs e)
        {
            Singleton.Instance.main_window.Visibility = Visibility.Collapsed;
            Singleton.Instance.users_window.Visibility = Visibility.Collapsed;
            Singleton.Instance.messages_window.Visibility = Visibility.Collapsed;

            if (Singleton.Instance.configuration_window.Visibility != Visibility.Visible)
                Singleton.Instance.configuration_window.Visibility = Visibility.Visible;
            else
                Singleton.Instance.configuration_window.Activate();
        }

        private void Driver_Click(Object sender, EventArgs e)
        {
            if (Singleton.Instance.driver_window.Visibility != Visibility.Visible)
                Singleton.Instance.driver_window.Visibility = Visibility.Visible;
            else
                Singleton.Instance.driver_window.Activate();
        }

        
    }
}
