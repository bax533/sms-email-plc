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

namespace SMS_EMAIL_PLC
{
    /// <summary>
    /// Logika interakcji dla klasy MyMessageBox.xaml
    /// </summary>
    public partial class MyMessageBox : Window
    {
        public MyMessageBox(string text)
        {
            InitializeComponent();
            WindowStyle = WindowStyle.None;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Textblock.Text = text;
            Title.Text = "";
            Title_Bar.Height = 0;
        }
        public MyMessageBox(string text, string title)
        {
            InitializeComponent();
            WindowStyle = WindowStyle.None;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Textblock.Text = text;
            Title.Text = title;
        }

        private void OK_Click(Object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
