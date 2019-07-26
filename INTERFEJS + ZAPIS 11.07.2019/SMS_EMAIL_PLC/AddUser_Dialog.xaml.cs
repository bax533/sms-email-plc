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
    /// Logika interakcji dla klasy AddUser_Dialog.xaml
    /// </summary>
    public partial class AddUser_Dialog : Window
    {
        public AddUser_Dialog()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }


        private void AddUser_Dialog_Click(object sender, EventArgs e)
        {
            string id = ID_text.Text;
            if (!Singleton.Instance.Is_User_ID_Repeated(id))
            {
                string name = Name_text.Text;
                string phone_number = Nr_text.Text;
                string email = Email_text.Text;
                Singleton.Instance.users_window.Add_Line(id, name, phone_number, email);
                Singleton.Instance.Add_User(new User(id, name, phone_number, email));
                Singleton.Instance.configuration_window.Refresh();
                this.Close();
            }
            else
                System.Windows.MessageBox.Show("ID nie mogą się powtarzać!");
        }
    }
}
