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
    public class Message_Add
    {
        private string _comment;
        private bool active;

        public Message_Add(string comment)
        {
            _comment = comment;
            active = true;
        }
        public Message_Add(string comment, bool active)
        {
            _comment = comment;
            this.active = active;
        }

        public string Comment
        {
            get
            {
                return _comment;
            }
        }

        public void SetActive(bool val)
        {
            active = val;
        }

        public bool IsActive()
        {
            return active;
        }
    }


    [Serializable]
    public class Message_Read
    {
        public string id;
        public string text;
        public string status;
        public Message_Read(string id, string text, string status)
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

        private string filePath = "";

        private DispatcherTimer timer = new DispatcherTimer();
        //private DispatcherTimer statusTimer = new DispatcherTimer();

        public SMS_Manager sms_manager = new SMS_Manager();
        public Email_Manager email_manager = new Email_Manager();
        public SQL_Manager sql_manager = new SQL_Manager();

        public MainWindow main_window;
        public Users_Page users_page = new Users_Page();
        public Configuration_Page configuration_page = new Configuration_Page();
        public Status_Page status_page = new Status_Page();
        public Messages_Page messages_page = new Messages_Page();
        public ImportExportPage importexport_page = new ImportExportPage();

        public List<User> users = new List<User>();
        public Dictionary<string, Message_Add> messages_dict = new Dictionary<string,Message_Add>();
        public Dictionary<string, Dictionary<string, Configuration>> configuration = new Dictionary<string, Dictionary<string, Configuration>>();


        public Thread Checker_Thread;
        public Thread Alarm_Thread;

        public string password = "admin1";
        public string backup_password = "1Admin10";
        public bool Admin = false;

        private Queue<Message_Read> messages_queue = new Queue<Message_Read>();

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
            else if(obj is CheckBox checkBox)
            {
                string nr = "";
                for (int i = 3; i < checkBox.Name.Length; i++)
                    nr += checkBox.Name[i];
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

            try
            {
                Checker_Thread.Abort();
            }
            catch(Exception ex)
            { }
            System.Windows.Application.Current.Shutdown();
        }

        public void ChangePassword(string newPassword)
        {
            password = newPassword;
        }


        public void Set_Start(int interval)
        {
            try
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
                        Load_Settings_With_Password();
                    }
                    catch (Exception ex)
                    { Singleton.Show_MessageBox(ex.Message + "nie można wczytać ustawień"); }
                }
            }
            catch(Exception eee)
            { }

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
                                messages_queue.Enqueue(new Message_Read(data[0], data[1], data[2]));
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
            File.Delete("default.cnf");
            try
            {
                IFormatter formatter = new BinaryFormatter();

                Stream stream = File.Open("default.cnf", FileMode.OpenOrCreate);

                formatter.Serialize(stream, password);
                formatter.Serialize(stream, users.Count);
                formatter.Serialize(stream, messages_dict.Count);

                foreach (User user in Singleton.Instance.users)
                    formatter.Serialize(stream, user);

                foreach (KeyValuePair<string, Message_Add> msg_in in Singleton.Instance.messages_dict)
                {
                    formatter.Serialize(stream, msg_in.Key);
                    formatter.Serialize(stream, msg_in.Value);
                }

                foreach (User user in Singleton.Instance.users)
                {
                    formatter.Serialize(stream, configuration[user.Get_ID()].Count);
                    foreach (KeyValuePair<string, Configuration> cnf in Singleton.Instance.configuration[user.Get_ID()])
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


        public void Load_Settings_With_Password()
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = File.Open("default.cnf", FileMode.Open);

                password = (string)formatter.Deserialize(stream);

                int users_count = (int)formatter.Deserialize(stream);
                int msgs_count  = (int)formatter.Deserialize(stream);

                Clear_Users();

                for (int i = 0; i < users_count; i++)
                {
                    User user = (User)formatter.Deserialize(stream);
                    Add_User(user);
                }

                Singleton.Instance.messages_dict = new Dictionary<string, Message_Add>();

                for (int i = 0; i < msgs_count; i++)
                {
                    string id = (string)formatter.Deserialize(stream);
                    Message_Add msg = (Message_Add)formatter.Deserialize(stream);
                    Add_Message(id, msg);
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

            messages_page.Window_Clear();
            foreach (KeyValuePair<string, Message_Add> msg in messages_dict)
                messages_page.Add_Line(msg.Key, msg.Value.Comment, msg.Value.IsActive());

            configuration_page.Refresh();
        }

        private void Set_Statuses()
        {
            if (sms_manager.connected)
            {
                status_page.SMS_Status = "true";
            }
            else
            {
                status_page.SMS_Status = "false";
            }

            if(email_manager.status)
            {
                status_page.Email_Status = "true";
            }
            else
            {
                status_page.Email_Status = "false";
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


        void Log_Msg(Message_Read msg)
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
            while (messages_queue.Count > 0)
            {
                Message_Read peek = messages_queue.Dequeue();
                string message_id = peek.id;
                string message_text = peek.text;
                string status = peek.status;

                Log_Msg(peek);

                bool up = status.Equals("2") ? false : true;


                if(messages_dict.ContainsKey(message_id))
                {
                    if (!messages_dict[message_id].IsActive())
                        continue;
                }
                else
                {
                    continue;
                }

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
                            status_page.Last_Message = "id: " + message_id + ", data: " + System.DateTime.Now.ToString();
                        }
                    }
                }
            }
        }

        public void Clear_Messages_Dict()
        {
            messages_dict.Clear();
        }

        public void Clear_Messages()
        {
            messages_queue.Clear();
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

        public bool Add_Message(string id, Message_Add msg)
        {
            if(messages_dict.ContainsKey(id))
            {
                Singleton.Show_MessageBox("id nie mogą się powtarzać");
                return false;
            }

            messages_dict[id] = msg;
            return true;
        }

        public void Remove_Message_From_Dict(string nr)
        {
            if (messages_dict.ContainsKey(nr))
                messages_dict.Remove(nr);
        }

        public void SetMessageActive(string id, bool active)
        {
            if (messages_dict.ContainsKey(id))
                messages_dict[id].SetActive(active);
            else
                Singleton.Show_MessageBox("nie ma takiej wiadomości");
        }

        
        public void Add_User(User user)
        {
            users.Add(user);
            configuration[user.Get_ID()] = new Dictionary<string, Configuration>();
        }

        public void Copy_Config(string from_id, string to_id)
        {
            if (Singleton.Instance.configuration.ContainsKey(from_id))
            {
                Singleton.Instance.configuration[to_id] = Singleton.Instance.configuration[from_id];
            }
            else
                Singleton.Show_MessageBox("Nie przypisano konfiguracji do użytkownika wyjściowego!");
        }

        public void Add_To_Config(string user_id, string key, Configuration config)
        {
            Singleton.Instance.configuration[user_id][key] = config;
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
