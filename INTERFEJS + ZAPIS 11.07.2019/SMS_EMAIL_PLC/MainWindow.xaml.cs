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
            Toolbar_Panel.Children.Add(new TextBlock { Height = 1, Background = Brushes.Black });
            Singleton.Instance.main_window = this;
            Singleton.Instance.SetTimer(100);
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
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
                Singleton.Instance.sql_manager.cnn.Close();
            }
            catch(Exception ex)
            { }
            try
            {
                Singleton.Instance.plc_manager.plc.Close();
            }
            catch (Exception ex)
            { }
            try
            {
                Singleton.Instance.sms_manager.Close();
            }
            catch(Exception ex)
            { }

            System.Windows.Application.Current.Shutdown();
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
                        formatter.Serialize(stream, Singleton.Instance.messages.Count);

                        foreach (User user in Singleton.Instance.users)
                            formatter.Serialize(stream, user);

                        foreach (KeyValuePair<string, Message> msg in Singleton.Instance.messages)
                        {
                            formatter.Serialize(stream, msg.Key);
                            formatter.Serialize(stream, msg.Value);
                        }

                        foreach (User user in Singleton.Instance.users)
                        {
                            foreach (KeyValuePair<string, Message> msg in Singleton.Instance.messages)
                                formatter.Serialize(stream, Singleton.Instance.configuration[user.Get_ID()][msg.Key]);
                        }
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


        private void Load_Settings_Click(Object sender, EventArgs e)
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
                        int msgs_count = (int)formatter.Deserialize(stream);

                        Singleton.Instance.Clear_Users();

                        for (int i = 0; i < users_count; i++)
                        {
                            User user = (User)formatter.Deserialize(stream);
                            Singleton.Instance.Add_User(user);
                        }

                        Singleton.Instance.Clear_Messages();

                        Dictionary<string, Message> new_msgs = new Dictionary<string, Message>();

                        for (int i = 0; i < msgs_count; i++)
                        {
                            string key = (string)formatter.Deserialize(stream);
                            Message new_msg = (Message)formatter.Deserialize(stream);
                            new_msgs[key] = new_msg;
                        }

                        Thread.Sleep(200);

                        foreach (KeyValuePair<string, Message> msg in new_msgs)
                        {
                            Singleton.Instance.Set_Message(msg.Key, msg.Value);
                        }

                        Singleton.Instance.configuration = new Dictionary<string, Dictionary<string, Configuration>>();

                        foreach (User user in Singleton.Instance.users)
                        {
                            Singleton.Instance.configuration[user.Get_ID()] = new Dictionary<string, Configuration>();
                        }


                        foreach (User user in Singleton.Instance.users)
                        {
                            foreach (KeyValuePair<string, Message> msg in new_msgs)
                            {
                                Configuration load = (Configuration)formatter.Deserialize(stream);
                                Singleton.Instance.Add_To_Config(user.Get_ID(), msg.Key, load);
                            }
                        }

                        Singleton.Instance.Add_Lines_To_Windows(new_msgs);

                        System.Windows.MessageBox.Show("wczytano pomyślnie");
                    }
                }


            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
