using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Microsoft.Win32;

namespace SMS_EMAIL_PLC
{
    public partial class MainWindow
    {
        private void Connect_Click(object sender, EventArgs e)
        {
            try
            {
                string type = type_box.Text;
                string ip = ip_box.Text;
                int slot = Int16.Parse(slot_box.Text);
                int rack = Int16.Parse(rack_box.Text);
                plc_status_text.Foreground = Brushes.Black;
                if (Singleton.Instance.plc_manager.Load_Plc(type, ip, rack, slot))
                {
                    plc_status_text.Text = "połączono";
                    plc_status_text.Background = Brushes.Green;
                    Singleton.Instance.plc_manager.connected = true;
                }
                else
                {
                    plc_status_text.Background = Brushes.Red;
                    plc_status_text.Text = "nie połączono";
                    Singleton.Instance.plc_manager.connected = false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void DatabaseLogin_Click(Object sender, EventArgs e)
        {
                Singleton.Instance.sql_manager.Login(Login_Box.Text, Password_Box.Password.ToString(), DBServer_Box.Text, Base_Box.Text);
        }



        private void Users_Click(object sender, EventArgs e)
        {
            Singleton.Instance.users_window.Visibility = Visibility.Visible;
        }

        private void Messages_Click(object sender, EventArgs e)
        {
            Singleton.Instance.messages_window.Visibility = Visibility.Visible;
        }

        private void Configure_Click(object sender, EventArgs e)
        {
            Singleton.Instance.configuration_window.Visibility = Visibility.Visible;
        }

        private void Driver_Click(Object sender, EventArgs e)
        {
            Singleton.Instance.driver_window.Visibility = Visibility.Visible;
        }

        private void Save_Settings_Click(Object sender, EventArgs e)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                bool? result = saveFileDialog.ShowDialog();

                saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 2;
                saveFileDialog.RestoreDirectory = true;

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
                System.Windows.MessageBox.Show("spróbuj ponownie za chwilę");
            }
        }


        private void Load_Settings_Click(Object sender, EventArgs e)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();


                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.InitialDirectory = "c:\\Users\\Szymon\\Desktop";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
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

                        Add_Lines_To_Windows(new_msgs);

                        System.Windows.MessageBox.Show("wczytano pomyślnie");
                    }
                }
                

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}
