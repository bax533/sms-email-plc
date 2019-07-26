using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
            Toolbar_Panel.Children.Add(new My_Toolbar());
        }

        void UsersWindow_Closing(object sender, CancelEventArgs e)
        {
            Singleton.Instance.application_shutdown = true;
            Singleton.Instance.Checker_Thread.Abort();

            foreach (Window window in Application.Current.Windows)
                try
                {
                    window.Close();
                }
                catch (Exception ex)
                {
                }

            try
            {
                Singleton.Instance.sms_manager.Close();
            }
            catch (Exception ex)
            { }

            System.Windows.Application.Current.Shutdown();
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
            TextBlock IDBox = new TextBlock
            {
                Text = id,
                Width = 50,
                Height = 20,
                FontSize = 15,
                TextAlignment = TextAlignment.Center
            };
            ID_Panel.Children.Add(IDBox);

            TextBlock NameBox = new TextBlock
            {
                Text = name,
                Width = 150,
                Height = 20,
                FontSize = 15,
                TextAlignment = TextAlignment.Center
            };
            Name_Panel.Children.Add(NameBox);

            TextBlock NrBox = new TextBlock
            {
                Text = phone_number,
                Width = 100,
                Height = 20,
                FontSize = 15,
                TextAlignment = TextAlignment.Center
            };
            PhoneNumber_Panel.Children.Add(NrBox);

            TextBlock EmailBox = new TextBlock
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
            string id = ((TextBlock)ID_Panel.Children[it]).Text;
            ID_Panel.Children.RemoveAt(it);
            Name_Panel.Children.RemoveAt(it);
            PhoneNumber_Panel.Children.RemoveAt(it);
            Email_Panel.Children.RemoveAt(it);
            RemoveButtons_Panel.Children.RemoveAt(it);

            for (int i = it; i < ID_Panel.Children.Count; i++)
            {
                ((Button)RemoveButtons_Panel.Children[i]).Name = "rmv" + i.ToString();
            }
            Singleton.Instance.Remove_User(id);
            Singleton.Instance.Remove_User_From_Configuration(id);
            Singleton.Instance.configuration_window.Refresh();
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
                string id = ((TextBlock)ID_Panel.Children[i]).Text;
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
                string id = ((TextBlock)ID_Panel.Children[i]).Text;
                string name = ((TextBlock)Name_Panel.Children[i]).Text;
                string phone_number = ((TextBlock)PhoneNumber_Panel.Children[i]).Text;
                string email = ((TextBlock)Email_Panel.Children[i]).Text;

                Singleton.Instance.Add_User(new User(id, name, phone_number, email));
            }
            Singleton.Instance.configuration_window.Add_Users();
        }

        private void LoadDatabase_Click(Object sender, EventArgs e)
        {
            Singleton.Instance.sql_manager.Load_Users();
            Save_Users();
            Singleton.Instance.Clear_Configuration();
            Singleton.Instance.configuration_window.Refresh();
        }

        private void SaveButton_Click(Object sender, EventArgs e)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "usr files (*.usr)|*.usr",
                    FilterIndex = 2,
                    RestoreDirectory = true
                };
                bool? result = saveFileDialog.ShowDialog();

                

                Stream stream;

                if (result == true)
                {
                    if ((stream = saveFileDialog.OpenFile()) != null)
                    {
                        formatter.Serialize(stream, Singleton.Instance.users.Count);

                        foreach (User user in Singleton.Instance.users)
                            formatter.Serialize(stream, user);

                        System.Windows.MessageBox.Show("zapisano pomyślnie!");
                        stream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message + "\nspróbuj ponownie za chwilę");
            }
        }

        private void LoadButton_Click(Object sender, EventArgs e)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();

                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = "c:\\Users\\Szymon\\Desktop",
                    Filter = "usr files (*.usr)|*.usr",
                    FilterIndex = 2,
                    RestoreDirectory = true
                };
                bool? result = openFileDialog.ShowDialog();

                Stream stream;

                if (result == true)
                {
                    if ((stream = openFileDialog.OpenFile()) != null)
                    {

                        int users_count = (int)formatter.Deserialize(stream);
                        Singleton.Instance.Clear_Users();

                        for (int i = 0; i < users_count; i++)
                        {
                            User user = (User)formatter.Deserialize(stream);
                            Singleton.Instance.Add_User(user);
                        }

                        Window_Clear();
                        foreach (User user in Singleton.Instance.users)
                            Add_Line(user.Get_ID(), user.Get_Name(), user.Get_Number(), user.Get_Email());

                        System.Windows.MessageBox.Show("wczytano pomyślnie");
                    }
                }

                Singleton.Instance.configuration = new Dictionary<string, Dictionary<string, Configuration>>();
                Singleton.Instance.configuration_window.Refresh();


            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}