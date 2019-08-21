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
    /// <summary>
    /// Logika interakcji dla klasy Status_Page.xaml
    /// </summary>
    public partial class Status_Page : Page, INotifyPropertyChanged
    {
        public bool sms_status = false;
        public bool email_status = false;
        public bool sql_status = false;
        public string _last_message;
       
        public string Last_Message
        {
            get { return _last_message; }
            set
            {
                _last_message = value;
                RaisePropertyChanged("Last_Message");
            }
        }

        public string SMS_Status
        {
            get { return sms_status ? "Połączono" : "Niepołączono"; }
            set
            {
                sms_status = value == "true";
                RaisePropertyChanged("SMS_Status");
                if (sms_status)
                    sms_status_text.Background = Brushes.Green;
                else
                    sms_status_text.Background = Brushes.Red;
            }
        }

        public string Email_Status
        {
            get { return email_status ? "Połączono" : "Niepołączono"; }
            set
            {
                email_status = value == "true";
                RaisePropertyChanged("Email_Status");
                if (email_status)
                    email_status_text.Background = Brushes.Green;
                else
                    email_status_text.Background = Brushes.Red;
            }
        }

        public string SQL_Status
        {
            get { return sql_status ? "Połączono" : "Niepołączono"; }
            set
            {
                sql_status = value == "true";
                RaisePropertyChanged("SQL_Status");
                if (sql_status)
                    sql_status_text.Background = Brushes.Green;
                else
                    sql_status_text.Background = Brushes.Red;
            }
        }

             
        public Status_Page()
        {
            DataContext = this;
            InitializeComponent();
        }
        

        private void CredentialLogin_Click(object sender, RoutedEventArgs e)
        {
            if (Credential_Box.Password == Singleton.Instance.password || Credential_Box.Password == Singleton.Instance.backup_password)
            {
                Singleton.Instance.Admin = true;
                CredentialStatus_Block.Text = "Administrator";
                CredentialStatus_Block.FontWeight = FontWeights.Bold;
                CredentialStatus_Block.Foreground = Brushes.PaleGoldenrod;
            }
            else
            {
                Singleton.Instance.Admin = false;
                CredentialStatus_Block.Text = "Gość";
                CredentialStatus_Block.FontWeight = FontWeights.Bold;
                CredentialStatus_Block.Foreground = Brushes.DarkGray;
                Singleton.Show_MessageBox("Niepoprawne hasło!");
            }
        }

        private void CredentialLogout_Click(object sender, RoutedEventArgs e)
        {
            Singleton.Instance.Admin = false;
            CredentialStatus_Block.Text = "Gość";
            CredentialStatus_Block.FontWeight = FontWeights.Bold;
            CredentialStatus_Block.Foreground = Brushes.DarkGray;
        }

        private void ChangePassword_Click(Object sender, EventArgs e)
        {
            if (Singleton.Instance.Admin)
            {
                string newPassword = Singleton.Get_Dialog("tu wpisz nowe hasło");
                Singleton.Instance.ChangePassword(newPassword);
            }
            else
                Singleton.Show_MessageBox("Niewystarczające uprawnienia!");
        }

        private void SetPort_Click(Object sender, EventArgs e)
        {
            Singleton.Instance.port = COM_Box.Text;
            Singleton.Show_MessageBox("zmieniono port na: " + COM_Box.Text);
        }

        private void Save_Settings_Click(Object sender, EventArgs e)
        {
            
        }


        private void Load_Settings_Click(Object sender, EventArgs e)
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged(string propertyName)
        {
            var handlers = PropertyChanged;

            handlers(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
