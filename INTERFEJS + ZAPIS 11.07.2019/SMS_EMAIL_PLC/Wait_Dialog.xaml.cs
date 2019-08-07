using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SMS_EMAIL_PLC
{
    /// <summary>
    /// Logika interakcji dla klasy Wait_Dialog.xaml
    /// </summary>
    public partial class Wait_Dialog : Window
    {
        DispatcherTimer _timer;
        string initial_text;

        public Wait_Dialog(string initial_text)
        {
            InitializeComponent();

            Waiting_Text.Text = initial_text;
            this.initial_text = initial_text;
            this.WindowStyle = WindowStyle.None;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Tick += new EventHandler(Timer_Tick);
            _timer.Start();
        }



        void Timer_Tick(Object sender, EventArgs e)
        {
            if (Waiting_Text.Text.Equals(initial_text+"..."))
                Waiting_Text.Text = initial_text;
            else
                Waiting_Text.Text += ".";
        }
    }
}
