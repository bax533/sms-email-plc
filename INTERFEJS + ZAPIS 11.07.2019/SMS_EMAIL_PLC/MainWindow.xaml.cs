using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SMS_EMAIL_PLC
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Singleton.Instance.main_window = this;
            Singleton.Instance.Set_Start(10000);
            this.Activate();
        }

        void Minimize_Click(Object sender, EventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        void Exit_Click(Object sender, EventArgs e)
        {
            Singleton.Instance.Close_Application();
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Singleton.Instance.Close_Application();
        }

        private void Main_Click(Object sender, RoutedEventArgs e)
        {
            Singleton.Instance.main_window.MainFrame.Content = Singleton.Instance.status_page;
            Singleton.Instance.Clear_PageToolbar(1);
        }

        private void Users_Click(object sender, EventArgs e)
        {
            Singleton.Instance.main_window.MainFrame.Content = Singleton.Instance.users_page;
            Singleton.Instance.Clear_PageToolbar(2);
        }

        private void Configuration_Click(object sender, EventArgs e)
        {
            Singleton.Instance.main_window.MainFrame.Content = Singleton.Instance.configuration_page;
            Singleton.Instance.Clear_PageToolbar(3);
        }

        private void Driver_Click(Object sender, EventArgs e)
        {
            
        }
    }
}
