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
    public partial class SMS_Dialog : Window
    {
        private string _result = "";
        string starting;

        public SMS_Dialog(string starting)
        {
            InitializeComponent();
            this.starting = starting;
            _result = starting;
            Input_Box.Text = starting;
        }

        
        public string Result
        {
            get { return _result; }
            set { _result = value; }
        }


        private void Return_Click(object sender, EventArgs e)
        {
            Result = Input_Box.Text;
            this.Close();
        }

        void Dialog_Closing(Object sender, EventArgs e)
        {
            if (Result == starting)
                Result = Singleton.Instance.password;
        }
    }
}
