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
        public bool application_shutdown = false;
        string last_message = "";

        private string filePath = "";

        private DispatcherTimer timer = new DispatcherTimer();
        private DispatcherTimer statusTimer = new DispatcherTimer();

        public List<Driver> toControlUP = new List<Driver>();
        public List<Driver> toControlDown = new List<Driver>();
        public SMS_Manager sms_manager = new SMS_Manager();
        public Email_Manager email_manager = new Email_Manager();
        //public PLC_Manager plc_manager = new PLC_Manager();
        public SQL_Manager sql_manager = new SQL_Manager();


        public Users_Window users_window = new Users_Window();
        //public Messages_Window messages_window = new Messages_Window();
        public Configuration_Window configuration_window = new Configuration_Window();
        public MainWindow main_window;
        //public Driver_Window driver_window = new Driver_Window();

        public List<User> users = new List<User>();
        public Dictionary<string, Dictionary<string, Configuration>> configuration = new Dictionary<string, Dictionary<string, Configuration>>();
        public Dictionary<int, bool> already_alarmed = new Dictionary<int, bool>();
        public Thread Checker_Thread;
        public Thread Alarm_Thread;

        private Queue<Message> messages = new Queue<Message>();

        public string port = "";

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
                    System.Windows.MessageBox.Show("BŁĄD WCZYTYWANIA USTAWIEŃ!", "FATAL ERROR");
                }

                try
                {
                    Load_Settings("default.cnf");
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
            port = main_window.COM_Box.Text;
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

        public void Load_Settings(string path)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = File.Open(path, FileMode.Open);

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

                //System.Windows.MessageBox.Show("wczytano pomyślnie");
                   
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }



        public void Add_Lines_To_Windows()
        {
            users_window.Window_Clear();
            foreach (User user in users)
                users_window.Add_Line(user.Get_ID(), user.Get_Name(), user.Get_Number(), user.Get_Email());

            configuration_window.Refresh();
        }

        private void Set_Statuses()
        {
            if (sms_manager.connected)
            {
                main_window.sms_status_text.Text = "POŁĄCZONO";
                main_window.sms_status_text.Background = Brushes.LawnGreen;
                main_window.sms_status_text.Foreground = Brushes.Black;
            }
            else
            {
                main_window.sms_status_text.Text = "NIE POŁĄCZONO";
                main_window.sms_status_text.Background = Brushes.Red;
                main_window.sms_status_text.Foreground = Brushes.Black;
            }
        }


        private void Status_Checker()
        {
            while (!application_shutdown)
            {
                sms_manager.Check_Connection(port);

                Thread.Sleep(5000);
            }
        }

        private void Send_SMS(string msg, string number)
        {
            //if (sms_manager.connected)
            //{
                System.Windows.MessageBox.Show("Wysłano sms na nr: " + number + " o treści: " + msg);
                last_message = "treść: " + msg + ", data: " + System.DateTime.Now.ToString();
                //sms_manager.Send(msg, number);
            //}
        }

        private void Send_Email(string msg, string adress)
        {
            System.Windows.MessageBox.Show("Wysłano email na adres: " + adress + " o treści: " + msg);
            email_manager.Send(adress, "ALERT", msg);
        }


        public void Send_Messages()
        {
            while (messages.Count > 0)
            {
                Message peek = messages.Dequeue();
                string message_id = peek.id;
                string message_text = peek.text;
                string status = peek.status;

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
