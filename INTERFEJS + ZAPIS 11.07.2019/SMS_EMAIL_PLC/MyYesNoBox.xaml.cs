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
    /// Logika interakcji dla klasy MyYesNoBox.xaml
    /// </summary>
    public partial class MyYesNoBox : Window
    {
        private bool _result;
        public MyYesNoBox(string text)
        {
            InitializeComponent();
            WindowStyle = WindowStyle.None;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Textblock.Text = text;
            Title.Text = "";
            Title_Bar.Height = 0;
        }
        public MyYesNoBox(string text, string title)
        {
            InitializeComponent();
            WindowStyle = WindowStyle.None;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Textblock.Text = text;
            Title.Text = title;
        }

        public bool Result
        {
            get { return _result; }
        }


        private void Yes_Click(Object sender, EventArgs e)
        {
            _result = true;
            this.Hide();
        }

        private void No_Click(Object sender, EventArgs e)
        {
            _result = false;
            this.Hide();
        }
    }
}
