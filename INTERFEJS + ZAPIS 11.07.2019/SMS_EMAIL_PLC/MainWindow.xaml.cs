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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Toolbar_Panel.Children.Add(new My_Toolbar());
            Singleton.Instance.main_window = this;
            Singleton.Instance.Set_Start(5000);
            this.Activate();
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Singleton.Instance.Close_Application();
        }

        

        private void DatabaseLogin_Click(Object sender, EventArgs e)
        {
            bool status = Singleton.Instance.sql_manager.Login(Login_Box.Text, Password_Box.Password.ToString(), DBServer_Box.Text, Base_Box.Text);
            sql_status_text.Foreground = Brushes.Black;
            sql_status_text.Text = status ? "Połączono" : "Niepołączono";
            sql_status_text.Background = status ? Brushes.LawnGreen : Brushes.Red;
        }

        private void Save_Settings_Click(Object sender, EventArgs e)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "config files (*.cnf)|*.cnf",
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


                        foreach (User user in Singleton.Instance.users)
                        {
                            formatter.Serialize(stream, Singleton.Instance.configuration[user.Get_ID()].Count);
                            foreach (KeyValuePair<string, Configuration> cnf in Singleton.Instance.configuration[user.Get_ID()])
                            {
                                formatter.Serialize(stream, cnf.Key);
                                formatter.Serialize(stream, cnf.Value);
                            }
                                
                        }
                        System.Windows.MessageBox.Show("zapisano pomyślnie!");
                        stream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        

        private void Load_Settings_Click(Object sender, EventArgs e)
        {
            if (Singleton.Instance.Admin)
            {
                try
                {
                    IFormatter formatter = new BinaryFormatter();

                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        InitialDirectory = "c:\\Users\\Szymon\\Desktop",
                        Filter = "config files (*.cnf)|*.cnf",
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

                            Singleton.Instance.configuration = new Dictionary<string, Dictionary<string, Configuration>>();

                            foreach (User user in Singleton.Instance.users)
                            {
                                Singleton.Instance.configuration[user.Get_ID()] = new Dictionary<string, Configuration>();
                            }


                            foreach (User user in Singleton.Instance.users)
                            {
                                int n = (int)formatter.Deserialize(stream);
                                for (int i = 0; i < n; i++)
                                {
                                    string msg_id = (string)formatter.Deserialize(stream);
                                    Configuration load = (Configuration)formatter.Deserialize(stream);
                                    Singleton.Instance.Add_To_Config(user.Get_ID(), msg_id, load);
                                }
                            }

                            Singleton.Instance.Add_Lines_To_Windows();

                            System.Windows.MessageBox.Show("wczytano pomyślnie");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
            else
                System.Windows.MessageBox.Show("Niewystarczające uprawnienia!");
        }

        private void Credential_Click(object sender, RoutedEventArgs e)
        {
            if(Credential_Box.Password == Singleton.Instance.password)
            {
                Singleton.Instance.Admin = true;
                CredentialStatus_Block.Text = "Administrator";
                CredentialStatus_Block.Foreground = Brushes.PaleGoldenrod;
            }
            else
            {
                Singleton.Instance.Admin = false;
                CredentialStatus_Block.Text = "Gość";
                CredentialStatus_Block.Foreground = Brushes.DarkGray;
            }
        }

        private void ChangePassword_Click(Object sender, EventArgs e)
        {
            if (Singleton.Instance.Admin)
            {
                string newPassword = Singleton.Get_Dialog("tu wpisz nowe hasło");
                Singleton.Instance.ChangePassword(newPassword);
            }
            else
                System.Windows.MessageBox.Show("Niewystarczające uprawnienia!");
        }
    }
}
