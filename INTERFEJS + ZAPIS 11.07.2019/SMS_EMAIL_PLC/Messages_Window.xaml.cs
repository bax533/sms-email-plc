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
using System.Windows.Shapes;


namespace SMS_EMAIL_PLC
{
    
    
    public partial class Messages_Window : Window
    {
        public Messages_Window()
        {
            InitializeComponent();
            Toolbar_Panel.Children.Add(new My_Toolbar());
        }

        void MessagesWindow_Closing(object sender, CancelEventArgs e)
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
        
        public string Get_Dialog(string starting)
        {
            SMS_Dialog dialog = new SMS_Dialog(starting);
            dialog.ShowDialog();
            return dialog.Result;
        }

        private void SMS_Click(object sender, EventArgs e)
        {
            int it = Singleton.Get_Nr_From_Object(sender);
            string number = ((TextBox)Number_Panel.Children[it]).Text;
            //string content = Get_Dialog(Singleton.Instance.Get_SMS(number));

            /*if (content.Length > 160)
            {
                System.Windows.MessageBox.Show("przekroczono limit (160 znaków)");
            }
            else
            {
                ((TextBlock)((StackPanel)Messages_Panel.Children[it]).Children[1]).Text = content;
               // Singleton.Instance.Set_SMS(number, content);
            }*/
            //System.Windows.MessageBox.Show(it.ToString());
        }

        private void Email_Click(object sender, EventArgs e)
        {
            int it = Singleton.Get_Nr_From_Object(sender);
            string number = ((TextBox)Number_Panel.Children[it]).Text;
            //string content = Get_Dialog(Singleton.Instance.Get_Email(number));

            //Singleton.Instance.Set_Email(number, content);
            //System.Windows.MessageBox.Show(it.ToString());
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            string content = Get_Dialog("podaj numer wiadomości");
            Add_Line(content);
        }

        public void Add_Line(string nr)
        {
           // Singleton.Instance.Create_Message(nr);

            TextBox NumberBox = new TextBox
            {
                IsReadOnly = true,
                Text = nr,
                Width = 50,
                Height = 20,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Number_Panel.Children.Add(NumberBox);

            StackPanel Buttons_Panel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
                Button SMS_Button = new Button
                {
                    Name = "SMS" + Messages_Panel.Children.Count.ToString(),
                    Content = "SMS",
                    Height = 20,
                    Width = 75
                };
                SMS_Button.Click += SMS_Click;

                TextBlock Message_Block = new TextBlock
                {
                    Name = "MSG" + Messages_Panel.Children.Count.ToString(),
                    //Text = Singleton.Instance.messages[nr].sms,
                    Width = 125,
                    Height = 20
                };
                
                Button Email_Button = new Button
                {
                    Name = "EML" + Messages_Panel.Children.Count.ToString(),
                    Content = "Email",
                    Height = 20,
                    Width = 75
                };
                Email_Button.Click += Email_Click;
                Buttons_Panel.Children.Add(SMS_Button);
                Buttons_Panel.Children.Add(Message_Block);
                //Buttons_Panel.Children.Add(Email_Button);
            Messages_Panel.Children.Add(Buttons_Panel);

            Button removeButton = new Button
            {
                Height = 20,
                Name = "rmv" + RemoveButtons_Panel.Children.Count.ToString(),
                Content = "-"
            };
            removeButton.Click += RemoveButton_Click;
            RemoveButtons_Panel.Children.Add(removeButton);
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            Button thisButton = (Button)sender;
            int it = Singleton.Get_Nr_From_Object(thisButton);

            string nr = ((TextBox)Number_Panel.Children[it]).Text;

            //Singleton.Instance.Remove_Message(nr);
            Singleton.Instance.Remove_Msg_From_Configuration(nr);

            Number_Panel.Children.RemoveAt(it);
            //Description_Panel.Children.RemoveAt(it);
            Messages_Panel.Children.RemoveAt(it);
            RemoveButtons_Panel.Children.RemoveAt(it);

            for (int i = it; i < Number_Panel.Children.Count; i++)
            {
                ((Button)RemoveButtons_Panel.Children[i]).Name = "rmv" + i.ToString();
                ((Button)((StackPanel)Messages_Panel.Children[i]).Children[0]).Name = "SMS" + i.ToString();
               //((Button)((StackPanel)Messages_Panel.Children[i]).Children[1]).Name = "EML" + i.ToString();
            }
        }

        public void Window_Clear()
        {
            Number_Panel.Children.RemoveRange(1, Number_Panel.Children.Count - 1);
            //Description_Panel.Children.RemoveRange(1, Description_Panel.Children.Count - 1);
            Messages_Panel.Children.RemoveRange(1, Messages_Panel.Children.Count - 1);
            RemoveButtons_Panel.Children.RemoveRange(1, RemoveButtons_Panel.Children.Count - 1);
        }

        private void LoadBase_Click(Object sender, EventArgs e)
        {
            Singleton.Instance.Clear_Configuration();
            Singleton.Instance.sql_manager.Load_Messages();
        }

        private void SaveButton_Click(Object sender, EventArgs e)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "mes files (*.mes)|*.mes",
                    FilterIndex = 2,
                    RestoreDirectory = true
                };

                bool? result = saveFileDialog.ShowDialog();

                Stream stream;

                if (result == true)
                {
                    if ((stream = saveFileDialog.OpenFile()) != null)
                    {
                       // formatter.Serialize(stream, Singleton.Instance.messages.Count);

                        /*foreach (KeyValuePair<string, Message> msg in Singleton.Instance.messages)
                        {
                            formatter.Serialize(stream, msg.Key);
                            formatter.Serialize(stream, msg.Value);
                        }*/

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
                    Filter = "mes files (*.mes)|*.mes",
                    FilterIndex = 2,
                    RestoreDirectory = true
                };
                bool? result = openFileDialog.ShowDialog();

                Stream stream;

                if (result == true)
                {
                    if ((stream = openFileDialog.OpenFile()) != null)
                    {
                        int msgs_count = (int)formatter.Deserialize(stream);

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
                            //Singleton.Instance.Set_Message(msg.Key, msg.Value);
                        }

                        Singleton.Instance.configuration = new Dictionary<string, Dictionary<string, Configuration>>();

                        Window_Clear();
                        foreach (KeyValuePair<string, Message> msg in new_msgs)
                            Add_Line(msg.Key);


                        Singleton.Instance.configuration_window.Refresh();
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
