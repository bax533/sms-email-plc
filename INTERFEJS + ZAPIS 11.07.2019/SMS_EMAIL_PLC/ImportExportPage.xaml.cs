using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SMS_EMAIL_PLC
{
    public partial class ImportExportPage : Page
    {
        public ImportExportPage()
        {
            InitializeComponent();
        }

        void ImportUsers_Click(Object sender, EventArgs e)
        {
            if (Singleton.Instance.Admin)
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

                            Singleton.Instance.users_page.Window_Clear();
                            foreach (User user in Singleton.Instance.users)
                                Singleton.Instance.users_page.Add_Line(user.Get_ID(), user.Get_Name(), user.Get_Number(), user.Get_Email());

                            Singleton.Show_MessageBox("wczytano pomyślnie");
                        }
                    }

                    Singleton.Instance.configuration = new Dictionary<string, Dictionary<string, Configuration>>();
                    Singleton.Instance.configuration_page.Refresh();


                }
                catch (Exception ex)
                {
                    Singleton.Show_MessageBox(ex.Message);
                }
            }
            else
                Singleton.Show_MessageBox("Niewystarczające uprawnienia!");
        }

        void ExportUsers_Click(Object sender, EventArgs e)
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
                        {
                            formatter.Serialize(stream, user);
                        }

                        Singleton.Show_MessageBox("zapisano pomyślnie!");
                        stream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Singleton.Show_MessageBox(ex.Message + "\nspróbuj ponownie za chwilę");
            }
        }

        void ImportMessages_Click(Object sender, EventArgs e)
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

                        Dictionary<string, Message_Add> new_msgs = new Dictionary<string, Message_Add>();

                        for (int i = 0; i < msgs_count; i++)
                        {
                            string key = (string)formatter.Deserialize(stream);
                            Message_Add new_msg = (Message_Add)formatter.Deserialize(stream);
                            new_msgs[key] = new_msg;
                        }


                        foreach (KeyValuePair<string, Message_Add> msg in new_msgs)
                        {
                            Singleton.Instance.Add_Message(msg.Key, msg.Value);
                        }

                        Singleton.Instance.configuration = new Dictionary<string, Dictionary<string, Configuration>>();

                        Singleton.Instance.messages_page.Window_Clear();

                        foreach (KeyValuePair<string, Message_Add> msg in new_msgs)
                            Singleton.Instance.messages_page.Add_Line(msg.Key, msg.Value.Comment, msg.Value.IsActive());


                        Singleton.Instance.configuration_page.Refresh();
                        Singleton.Show_MessageBox("wczytano pomyślnie");
                    }
                }
            }
            catch (Exception ex)
            {
                Singleton.Show_MessageBox(ex.Message);
            }
        }

        void ExportMessages_Click(Object sender, EventArgs e)
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
                        formatter.Serialize(stream, Singleton.Instance.messages_dict.Count);

                        foreach (KeyValuePair<string, Message_Add> msg in Singleton.Instance.messages_dict)
                        {
                            formatter.Serialize(stream, msg.Key);
                            formatter.Serialize(stream, msg.Value);
                        }

                        Singleton.Show_MessageBox("zapisano pomyślnie!");
                        stream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Singleton.Show_MessageBox(ex.Message + "\nspróbuj ponownie za chwilę");
            }
        }

        void ImportConfig_Click(Object sender, EventArgs e)
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
                            int messages_count = (int)formatter.Deserialize(stream);

                            Singleton.Instance.Clear_Users();
                            for (int i = 0; i < users_count; i++)
                            {
                                User user = (User)formatter.Deserialize(stream);
                                Singleton.Instance.Add_User(user);
                            }

                            Singleton.Instance.Clear_Messages_Dict();
                            for (int i = 0; i < messages_count; i++)
                            {
                                string id = (string)formatter.Deserialize(stream);
                                Message_Add msg = (Message_Add)formatter.Deserialize(stream);
                                Singleton.Instance.messages_dict[id] = msg;
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

                            Singleton.Show_MessageBox("wczytano pomyślnie");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Singleton.Show_MessageBox(ex.Message);
                }
            }
            else
                Singleton.Show_MessageBox("Niewystarczające uprawnienia!");
        }

        void ExportConfig_Click(Object sender, EventArgs e)
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
                        formatter.Serialize(stream, Singleton.Instance.messages_dict.Count);

                        foreach (User user in Singleton.Instance.users)
                            formatter.Serialize(stream, user);

                        foreach (KeyValuePair<string, Message_Add> msg_in in Singleton.Instance.messages_dict)
                        {
                            formatter.Serialize(stream, msg_in.Key);
                            formatter.Serialize(stream, msg_in.Value);
                        }


                        foreach (User user in Singleton.Instance.users)
                        {
                            formatter.Serialize(stream, Singleton.Instance.configuration[user.Get_ID()].Count);
                            foreach (KeyValuePair<string, Configuration> cnf in Singleton.Instance.configuration[user.Get_ID()])
                            {
                                formatter.Serialize(stream, cnf.Key);
                                formatter.Serialize(stream, cnf.Value);
                            }

                        }
                        Singleton.Show_MessageBox("zapisano pomyślnie!");
                        stream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Singleton.Show_MessageBox(ex.Message);
            }
        }
    }
}
