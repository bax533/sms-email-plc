using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Logika interakcji dla klasy AddMessage_Dialog.xaml
    /// </summary>
    public partial class AddMessage_Dialog : Window, INotifyPropertyChanged
    {
        private string _id = "ID";
        private string _comment = "OPIS";
        public bool closed;

        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                RaisePropertyChanged("ID");
            }


        }
        public string Comment
        { 
            get
            {
                return _comment;
            }

            set
            {
                _comment = value;
                RaisePropertyChanged("Comment");
            }
        }

        public AddMessage_Dialog()
        {
            DataContext = this;
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            closed = false;
        }


        private void AddMessage_Dialog_Click(Object sender, EventArgs e)
        {
            this.Hide();
        }

        void MessageDialog_Closing(Object sender, EventArgs e)
        {
            closed = true;
        }


        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged(string propertyName)
        {
            var handlers = PropertyChanged;

            handlers(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
