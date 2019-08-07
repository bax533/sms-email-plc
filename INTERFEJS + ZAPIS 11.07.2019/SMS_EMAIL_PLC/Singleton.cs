using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.ComponentModel;

namespace SMS_EMAIL_PLC
{
    [Serializable]
    public class Message
    {
        public string id;
        public string text;
        public string status;
        public Message(string id, string text, string status)
        {
            this.id = id;
            this.text = text;
            this.status = status;
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
        readonly string id;
        readonly string name;
        readonly string phone_number;
        readonly string email;
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

    class Singleton
    {
        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public bool application_shutdown = false;
        string last_message = "";

        private string filePath = "";

        private DispatcherTimer timer = new DispatcherTimer();
        private DispatcherTimer statusTimer = new DispatcherTimer();

        public List<Driver> toControlUP = new List<Driver>();
        public List<Driver> toControlDown = new List<Driver>();
        public SMS_Manager sms_manager = new SMS_Manager();
        public Email_Manager email_manager = new Email_Manager();
        public SQL_Manager sql_manager = new SQL_Manager();

        public MainWindow main_window;
        public Users_Page users_page = new Users_Page();
        public Configuration_Page configuration_page = new Configuration_Page();
        public Status_Page status_page = new Status_Page();

        public List<User> users = new List<User>();
        public Dictionary<string, Dictionary<string, Configuration>> configuration = new Dictionary<string, Dictionary<string, Configuration>>();
        public Dictionary<int, bool> already_alarmed = new Dictionary<int, bool>();
        public Thread Checker_Thread;
        public Thread Alarm_Thread;

        public string password = "admin1";
        public string backup_password = "95FPTM7XTXVD";
        public bool Admin = false;

        private Queue<Message> messages = new Queue<Message>();

        public string port = "COM1";

        

        public static void Show_MessageBox(string text)
        {
            MyMessageBox messageBox = new MyMessageBox(text);
            messageBox.Show();
        }
        public static void Show_MessageBox(string text, string title)
        {
            MyMessageBox messageBox = new MyMessageBox(text, title);
            messageBox.Show();
        }
        public static bool Show_MessageBox(string text, bool bol)
        {
            MyYesNoBox yesnoBox = new MyYesNoBox(text);
            yesnoBox.ShowDialog();
            return yesnoBox.Result;
        }

        public static string Get_Dialog(string starting)
        {
            SMS_Dialog dialog = new SMS_Dialog(starting);
            dialog.ShowDialog();
            return dialog.Result;
        }

        public static bool IsFileReady(string filename)
        {
            try
            {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static int Get_Nr_From_Object(Object obj)
        {
            if (obj is Button button)
            {
                string nr = "";
                for (int i = 3; i < button.Name.Length; i++)
                    nr += button.Name[i];
                return Int16.Parse(nr);
            }
            return 0;
        }

        public void Close_Application()
        {
            Save_Settings_With_Password();
            lock (Instance)
            {
                application_shutdown = true;
            }

            foreach (Window window in Application.Current.Windows)
            {
                try
                {
                    window.Close();
                }
                catch (Exception ex)
                {
                }
            }

            try
            {
                sms_manager.Close();
            }
            catch (Exception ex)
            { }

            Thread thread = new Thread(() =>
            {
                Wait_Dialog w = new Wait_Dialog("zamykanie");
                w.Show();

                w.Closed += (sender2, e2) =>
                w.Dispatcher.InvokeShutdown();

                System.Windows.Threading.Dispatcher.Run();
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();


            Thread.Sleep(5000);

            thread.Abort();

            try
            {
                Checker_Thread.Abort();
            }
            catch(Exception ex)
            { }
            System.Windows.Application.Current.Shutdown();
        }

        public void Clear_PageToolbar(int page)
        {
            main_window.PageToolbar_Panel.Children.RemoveRange(1, main_window.PageToolbar_Panel.Children.Count);
            ToolTip tol = new ToolTip();
            switch(page)
            {
                case 1:
                    status_page.AddToolbarButtons();
                    break;
                case 2:
                    users_page.AddToolbarButtons();
                    break;
            }
        }

        public void ChangePassword(string newPassword)
        {
            password = newPassword;
        }


        public void Set_Start(int interval)
        {
            using (StreamReader sr = new StreamReader("settings.txt"))
            {
                try
                {
                    filePath = sr.ReadLine();
                }
                catch (Exception ex)
                {
                    Singleton.Show_MessageBox("BŁĄD WCZYTYWANIA USTAWIEŃ!", "FATAL ERROR");
                }

                try
                {
                    Load_Settings_With_Password(Path.Combine("default.cnf"));
                }
                catch(Exception ex)
                { }
            }

            timer.Interval = new TimeSpan(0, 0, 0, 0, interval);
            timer.Tick += new EventHandler(OnTimedEvent);
            timer.Start();

            Checker_Thread = new Thread(Status_Checker);
            Checker_Thread.Start();

            Alarm_Thread = new Thread(Alarm);
            Alarm_Thread.Start();
        }

        public void OnTimedEvent(Object source, EventArgs e)
        {
            Set_Statuses();
            Send_Messages();
        }



        private void Alarm()
        {
            while (!application_shutdown)
            {
                //string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                try
                {
                    while (!IsFileReady(filePath) && !application_shutdown)
                    { }
                    if (application_shutdown)
                        return;

                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        string ln;
                        while ((ln = sr.ReadLine()) != null)
                        {
                            
                            if (ln.Equals(""))
                                continue;

                            int symbols = 0;
                            for (int i = 0; i < ln.Length; i++)
                                if (ln[i] == '<')
                                    symbols++;
                            if (symbols != 3)
                                continue;

                            string[] data = new string[3];  //0 - msg_id, 1 - treść, 2 - status;
                            int it = 0;                     //format pliku wejściowego: id<treść<status<

                            
                            try
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    do
                                    {
                                        if (it < ln.Length && ln[it] != '<')
                                            data[i] += ln[it];
                                        it++;
                                    }
                                    while (it < ln.Length && ln[it] != '<');
                                }
                                messages.Enqueue(new Message(data[0], data[1], data[2]));
                            }
                            catch(Exception ex)
                            { }
                        }
                        sr.Close();
                    }
                    using (StreamWriter outputFile = new StreamWriter(filePath))
                    {
                        outputFile.WriteLine("");
                        outputFile.Close();
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Thread.Sleep(10000);
            }
        }

        public void Save_Settings_With_Password()
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();

                Stream stream = File.Open(Path.Combine(docPath, "SMS_EMAIL_PLC\\default.cnf"), FileMode.Open);

                formatter.Serialize(stream, password);
                formatter.Serialize(stream, users.Count);

                foreach (User user in users)
                    formatter.Serialize(stream, user);


                foreach (User user in users)
                {
                    formatter.Serialize(stream, configuration[user.Get_ID()].Count);
                    foreach (KeyValuePair<string, Configuration> cnf in configuration[user.Get_ID()])
                    {
                        formatter.Serialize(stream, cnf.Key);
                        formatter.Serialize(stream, cnf.Value);
                    }

                }
                stream.Close();
            }
            catch (Exception ex)
            {
                Singleton.Show_MessageBox(ex.Message);
            }
        }


        public void Load_Settings_With_Password(string path)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = File.Open(path, FileMode.Open);

                password = (string)formatter.Deserialize(stream);

                int users_count = (int)formatter.Deserialize(stream);

                Clear_Users();

                for (int i = 0; i < users_count; i++)
                {
                    User user = (User)formatter.Deserialize(stream);
                    Add_User(user);
                }

                configuration = new Dictionary<string, Dictionary<string, Configuration>>();

                foreach (User user in users)
                {
                    configuration[user.Get_ID()] = new Dictionary<string, Configuration>();
                }


                foreach (User user in users)
                {
                    int n = (int)formatter.Deserialize(stream);
                    for (int i = 0; i < n; i++)
                    {
                        string msg_id = (string)formatter.Deserialize(stream);
                        Configuration load = (Configuration)formatter.Deserialize(stream);
                        Add_To_Config(user.Get_ID(), msg_id, load);
                    }
                }

                Add_Lines_To_Windows();

                //Singleton.Show_MessageBox("wczytano pomyślnie");
                   
            }
            catch (Exception ex)
            {
                Singleton.Show_MessageBox(ex.Message);
            }
        }



        public void Add_Lines_To_Windows()
        {
            users_page.Window_Clear();
            foreach (User user in users)
                users_page.Add_Line(user.Get_ID(), user.Get_Name(), user.Get_Number(), user.Get_Email());

            configuration_page.Refresh();
        }

        private void Set_Statuses()
        {
            if (sms_manager.connected)
            {
                status_page.sms_status_text.Text = "Połączono";
                status_page.sms_status_text.Background = Brushes.LawnGreen;
                status_page.sms_status_text.Foreground = Brushes.Black;
            }
            else
            {
                status_page.sms_status_text.Text = "Niepołączono";
                status_page.sms_status_text.Background = Brushes.Red;
                status_page.sms_status_text.Foreground = Brushes.Black;
            }

            if(email_manager.status)
            {
                status_page.email_status_text.Text = "Połączono";
                status_page.email_status_text.Background = Brushes.LawnGreen;
                status_page.email_status_text.Foreground = Brushes.Black;
            }
            else
            {
                status_page.email_status_text.Text = "Niepołączono";
                status_page.email_status_text.Background = Brushes.Red;
                status_page.email_status_text.Foreground = Brushes.Black;
            }
        }


        private void Status_Checker()
        {
            while (!application_shutdown)
            {
                sms_manager.Check_Connection(port);
                email_manager.Handshake();

                Thread.Sleep(5000);
            }
        }

        private void Send_SMS(string msg, string number)
        {
            //if (sms_manager.connected)
            //{
                Singleton.Show_MessageBox("Wysłano sms na nr: " + number + " o treści: " + msg);
            
            //sms_manager.Send(msg, number);
            //}
        }

        private void Send_Email(string msg, string adress)
        {
            Singleton.Show_MessageBox("Wysłano email na adres: " + adress + " o treści: " + msg);
            //email_manager.Send(adress, "ALERT", msg);
        }


        void Log_Msg(Message msg)
        {
            
            try
            {
                //while (!IsFileReady(Path.Combine(docPath,"DEBUG_LOG.txt")) && !application_shutdown)
                //{ }
                //if (application_shutdown)
                  //  return;
                File.AppendAllText("DEBUG_LOG.txt", msg.id + " " + msg.text + Environment.NewLine);
            }
            catch(Exception ex)
            {

            }
        }

        public void Send_Messages()
        {
            while (messages.Count > 0)
            {
                Message peek = messages.Dequeue();
                string message_id = peek.id;
                string message_text = peek.text;
                string status = peek.status;

                Log_Msg(peek);

                bool up = status.Equals("2") ? false : true;

                foreach (User user in users)
                {
                    if (configuration.ContainsKey(user.Get_ID()))
                    {
                        if (configuration[user.Get_ID()].ContainsKey(message_id))
                        {
                            if (up)
                            {
                                if (configuration[user.Get_ID()][message_id].sms_up)
                                    Send_SMS(message_text, user.Get_Number());
                                if (configuration[user.Get_ID()][message_id].email_up)
                                    Send_Email(message_text, user.Get_Email());
                            }
                            else
                            {
                                if (configuration[user.Get_ID()][message_id].sms_down)
                                    Send_SMS(message_text, user.Get_Number());
                                if (configuration[user.Get_ID()][message_id].email_down)
                                    Send_Email(message_text, user.Get_Email());
                            }
                            last_message = "treść: " + message_id + ", data: " + System.DateTime.Now.ToString();
                        }
                    }
                }
            }
        }

        public void Clear_Messages()
        {
            messages.Clear();
        }


        public void Clear_Users()
        {
            users = new List<User>();
        }

        public void Clear_Configuration()
        {
            foreach (User user in users)
                configuration[user.Get_ID()] = new Dictionary<string, Configuration>();
        }


        public void Add_User(User user)
        {
            users.Add(user);
        }

        

        public void Add_To_Config(string user_id, string key, Configuration config)
        {
            configuration[user_id][key] = config;
        }

        public bool Is_User_ID_Repeated(string id)
        {
            foreach(User user in users)
            {
                if (user.Get_ID() == id)
                    return true;
            }
            return false;
        }


        public void Remove_Msg_From_Configuration(string key)
        {
            foreach(User user in users)
            {
                if (configuration.ContainsKey(user.Get_ID()))
                {
                    if (configuration[user.Get_ID()].ContainsKey(key))
                        configuration[user.Get_ID()].Remove(key);
                }
            }
        }

        public void Remove_User_From_Configuration(string user_id)
        {
            if(configuration.ContainsKey(user_id))
            {
                configuration.Remove(user_id);
            }
        }

        public void Remove_User(string user_id)
        {
            for(int i=0; i<users.Count; i++)
            {
                if (users[i].Get_ID() == user_id)
                {
                    users.RemoveAt(i);
                    return;
                }
            }
        }

        private static Singleton m_oInstance = null;

        public static Singleton Instance
        {
            get
            {
                if (m_oInstance == null)
                {
                    m_oInstance = new Singleton();
                }
                return m_oInstance;
            }
        }

        private Singleton()
        {
           
        }
    }
}
