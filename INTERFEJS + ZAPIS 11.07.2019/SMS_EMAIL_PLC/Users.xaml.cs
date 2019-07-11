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

    [Serializable]
    public class Message
    {
        public string sms;
        public string email;
        public Message(string sms, string email)
        {
            this.sms = sms;
            this.email = email;
        }
        public Message()
        {
            sms = "";
            email = "";
        }
    }


    [Serializable]
    public class Configuration
    {
        public bool sms_up;
        public bool email_up;
        public bool sms_down;
        public bool email_down;
        public Configuration(bool sms_up, bool email_up, bool sms_down, bool email_down)
        {
            this.sms_up = sms_up;
            this.email_up = email_up;
            this.sms_down = sms_down;
            this.email_down = email_down;
        }
        public Configuration()
        {
            this.sms_up = false;
            this.email_up = false;
            this.sms_down = false;
            this.email_down = false;
        }
    }

    [Serializable]
    class User
    {
        string id;
        string name;
        string phone_number;
        string email;
        public User(string id, string name, string phone_number, string email)
        {
            this.id = id;
            this.name = name;
            this.phone_number = phone_number;
            this.email = email;
        }

        override
        public string ToString()
        {
            return id + " " + name + " " + phone_number + " " + email;
        }

        public string Get_ID()
        {
            return id;
        }

        public string Get_Name()
        {
            return name;
        }

        public string Get_Number()
        {
            return phone_number;
        }

        public string Get_Email()
        {
            return email;
        }
    }

    public partial class Users_Window : Window
    {
        public Users_Window()
        {
            InitializeComponent();
        }

        private void DBG_Click(object sender, EventArgs e)
        {
            Singleton.Instance.users_dbg();
        }


        void UsersWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Singleton.Instance.users_window.Visibility = Visibility.Collapsed;
        }

        public void Window_Clear()
        {
            ID_Panel.Children.RemoveRange(1, ID_Panel.Children.Count - 1);
            Name_Panel.Children.RemoveRange(1, Name_Panel.Children.Count - 1);
            PhoneNumber_Panel.Children.RemoveRange(1, PhoneNumber_Panel.Children.Count - 1);
            Email_Panel.Children.RemoveRange(1, Email_Panel.Children.Count - 1);
            RemoveButtons_Panel.Children.RemoveRange(1, RemoveButtons_Panel.Children.Count - 1);
        }


        public void Add_Line(string id, string name, string phone_number, string email)
        {
            TextBox IDBox = new TextBox();
            IDBox.Text = id;
            IDBox.Width = 50;
            IDBox.Height = 20;
            IDBox.FontSize = 15;
            IDBox.TextAlignment = TextAlignment.Center;
            ID_Panel.Children.Add(IDBox);

            TextBox NameBox = new TextBox();
            NameBox.Text = name;
            NameBox.Width = 150;
            NameBox.Height = 20;
            NameBox.FontSize = 15;
            NameBox.TextAlignment = TextAlignment.Center;
            Name_Panel.Children.Add(NameBox);

            TextBox NrBox = new TextBox();
            NrBox.Text = phone_number;
            NrBox.Width = 100;
            NrBox.Height = 20;
            NrBox.FontSize = 15;
            NrBox.TextAlignment = TextAlignment.Center;
            PhoneNumber_Panel.Children.Add(NrBox);

            TextBox EmailBox = new TextBox();
            EmailBox.Text = email;
            EmailBox.Width = 150;
            EmailBox.Height = 20;
            EmailBox.FontSize = 15;
            EmailBox.TextAlignment = TextAlignment.Center;
            Email_Panel.Children.Add(EmailBox);

            Button removeButton = new Button();
            removeButton.Height = 20;
            removeButton.Name = "rmv" + RemoveButtons_Panel.Children.Count.ToString();
            removeButton.Content = "-";
            removeButton.Click += RemoveButton_Click; 
            RemoveButtons_Panel.Children.Add(removeButton);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            AddUser_Dialog dialog = new AddUser_Dialog();
            dialog.Show();
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            Button thisButton = (Button)sender;
            int it = Singleton.Get_Nr_From_Object(thisButton);
            ID_Panel.Children.RemoveAt(it);
            Name_Panel.Children.RemoveAt(it);
            PhoneNumber_Panel.Children.RemoveAt(it);
            Email_Panel.Children.RemoveAt(it);
            RemoveButtons_Panel.Children.RemoveAt(it);
        }

        private void Save(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (User user in Singleton.Instance.users)
            {
                Singleton.Instance.configuration[user.Get_ID()] = new Dictionary<string, Configuration>();
            }



            Dictionary<string, bool> visited = new Dictionary<string, bool>();

            for (int i = 1; i < ID_Panel.Children.Count; i++) //sprawdzam powtórzenia
            {
                string id = ((TextBox)ID_Panel.Children[i]).Text;
                if (visited.ContainsKey(id))
                {
                    System.Windows.MessageBox.Show("ID nie mogą się powtarzać!");
                    return;
                }
                else
                    visited[id] = true;
            }


            Singleton.Instance.configuration_window.Clear_Window();
            Singleton.Instance.Clear_Users();

            for (int i = 1; i < ID_Panel.Children.Count; i++)
            {

                string id = ((TextBox)ID_Panel.Children[i]).Text;
                string name = ((TextBox)Name_Panel.Children[i]).Text;
                string phone_number = ((TextBox)PhoneNumber_Panel.Children[i]).Text;
                string email = ((TextBox)Email_Panel.Children[i]).Text;

                Singleton.Instance.Add_User(new User(id, name, phone_number, email));
            }
            Singleton.Instance.configuration_window.Add_Users();
            System.Windows.MessageBox.Show("zapisano pomyślnie!");
            e.Handled = true;
        }

        private void LoadDatabase_Click(Object sender, EventArgs e)
        {

        }
    }
}
