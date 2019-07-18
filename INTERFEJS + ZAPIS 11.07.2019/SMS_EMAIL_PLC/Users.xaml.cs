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

    public partial class Users_Window : Window
    {
        public Users_Window()
        {
            InitializeComponent();
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
            TextBox IDBox = new TextBox
            {
                Text = id,
                Width = 50,
                Height = 20,
                FontSize = 15,
                TextAlignment = TextAlignment.Center
            };
            ID_Panel.Children.Add(IDBox);

            TextBox NameBox = new TextBox
            {
                Text = name,
                Width = 150,
                Height = 20,
                FontSize = 15,
                TextAlignment = TextAlignment.Center
            };
            Name_Panel.Children.Add(NameBox);

            TextBox NrBox = new TextBox
            {
                Text = phone_number,
                Width = 100,
                Height = 20,
                FontSize = 15,
                TextAlignment = TextAlignment.Center
            };
            PhoneNumber_Panel.Children.Add(NrBox);

            TextBox EmailBox = new TextBox
            {
                Text = email,
                Width = 150,
                Height = 20,
                FontSize = 15,
                TextAlignment = TextAlignment.Center
            };
            Email_Panel.Children.Add(EmailBox);

            Button removeButton = new Button
            {
                Height = 20,
                Name = "rmv" + RemoveButtons_Panel.Children.Count.ToString(),
                Content = "-"
            };
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

            for (int i = it; i < ID_Panel.Children.Count; i++)
            {
                ((Button)RemoveButtons_Panel.Children[i]).Name = "rmv" + i.ToString();
            }
        }


        void Save_Users()
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
        }


        private void Save(object sender, ExecutedRoutedEventArgs e)
        {
            Save_Users();
            System.Windows.MessageBox.Show("zapisano pomyślnie!");
            e.Handled = true;
        }

        private void LoadDatabase_Click(Object sender, EventArgs e)
        {
            Singleton.Instance.sql_manager.Load_Users();
            Save_Users();
            Singleton.Instance.Clear_Configuration();
            Singleton.Instance.configuration_window.Refresh();
        }
    }
}