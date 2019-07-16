using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SMS_EMAIL_PLC
{
    [Serializable]
    public class Message
    {
        public string sms;
        public string email;
        public Message(string sms, string email)
        {
            this.sms = sms;
            this.email = email;
        }
        public Message()
        {
            sms = "";
            email = "";
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
        string id;
        string name;
        string phone_number;
        string email;
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
        private DispatcherTimer timer = new DispatcherTimer();
        private DispatcherTimer statusTimer = new DispatcherTimer();

        public List<Driver> toControlUP = new List<Driver>();
        public List<Driver> toControlDown = new List<Driver>();
        public SMS_Manager sms_manager = new SMS_Manager();
        public Email_Manager email_manager = new Email_Manager();
        public PLC_Manager plc_manager = new PLC_Manager();
        public SQL_Manager sql_manager = new SQL_Manager();


        public Users_Window users_window = new Users_Window();
        public Messages_Window messages_window = new Messages_Window();
        public Configuration_Window configuration_window = new Configuration_Window();
        public MainWindow main_window;
        public Driver_Window driver_window = new Driver_Window();

        public List<User> users = new List<User>();
        public Dictionary<string, Message> messages = new Dictionary<string, Message>();
        public Dictionary<string, Dictionary<string, Configuration>> configuration = new Dictionary<string, Dictionary<string, Configuration>>();
        public Dictionary<int, bool> already_alarmed = new Dictionary<int, bool>();

        public static int Get_Nr_From_Object(Object obj)
        {
            if(obj is Button)
            {
                Button button = ((Button)obj);
                string nr="";
                for (int i=3; i<button.Name.Length; i++)
                    nr += button.Name[i];
                return Int16.Parse(nr);
            }
            return 0;
        }

        public void SetTimer(int interval)
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, interval);
            timer.Tick += new EventHandler(OnTimedEvent);
            timer.Start();
        }

        public void OnTimedEvent(Object source, EventArgs e)
        {
            if (plc_manager.connected)
            {
                driver_window.OnTimedEvent();
                foreach (Driver driver in toControlUP)
                {
                    int value = plc_manager.Get_Int_Value(driver.name);

                    if (!already_alarmed.ContainsKey(value))
                        already_alarmed[value] = false;

                    if (!already_alarmed[value])
                    {
                        if (messages.ContainsKey(value.ToString()))
                        {
                            Send_Message(value.ToString(), true);
                            already_alarmed[value] = true;
                        }
                    }
                }

                foreach (Driver driver in toControlDown)
                {
                    int value = plc_manager.Get_Int_Value(driver.name);

                    if (!already_alarmed.ContainsKey(value))
                        already_alarmed[value] = false;

                    if (already_alarmed[value])
                    {
                        if (messages.ContainsKey(value.ToString()))
                        {
                            System.Windows.MessageBox.Show("wchodzi");
                            Send_Message(value.ToString(), false);
                            already_alarmed[value] = false;
                        }
                    }
                }
            }
        }

        private void Send_SMS(string message_id, string number, bool up)
        {
            string msg = messages[message_id].sms;
            if (!up) msg += " powrot";
            System.Windows.MessageBox.Show("Wysłano sms na nr: " + number + " o treści: " + msg);
            main_window.last_message_text.Text = "id wiadomości: " + message_id + ", treść: "+msg + ", data: " + System.DateTime.Now.ToString();
            sms_manager.Send(msg, number);
        }

        private void Send_Email(string message_id, string adress, bool up)
        {
            string msg = messages[message_id].email;
            if (!up) msg += " powrót";
            System.Windows.MessageBox.Show("Wysłano email na adres: " + adress + " o treści: " + msg);
        }


        public void Send_Message(string message_id, bool up)
        {
            foreach(User user in users)
            {
                if (configuration[user.Get_ID()].ContainsKey(message_id))
                {
                    if (up)
                    {
                        if (configuration[user.Get_ID()][message_id].sms_up)
                            Send_SMS(message_id, user.Get_Number(), up);
                        if (configuration[user.Get_ID()][message_id].email_up)
                            Send_Email(message_id, user.Get_Email(), up);
                    }
                    else
                    {
                        if (configuration[user.Get_ID()][message_id].sms_down)
                            Send_SMS(message_id, user.Get_Number(), up);
                        if (configuration[user.Get_ID()][message_id].email_down)
                            Send_Email(message_id, user.Get_Email(), up);
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


        public void Set_SMS(string number, string content)
        {
            if (messages.ContainsKey(number))
                messages[number].sms = content;
            else
            {
                messages[number] = new Message(content, "");
                //messages[number].sms = content;
            }
        }

        public string Get_SMS(string nr)
        {

            return messages[nr].sms;
        }

        public void Set_Email(string number, string content)
        {
            if (messages.ContainsKey(number))
                messages[number].email = content;
            else
            {
                messages[number] = new Message();
                messages[number].email = content;
            }
        }

        public string Get_Email(string nr)
        {
            return messages[nr].email;
        }

        
        public void Remove_From_Configuration(string key)
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

        public void Remove_Message(string key)
        {
            messages.Remove(key);
        }

        public void users_dbg()
        {
            string ret = "";
            for (int i = 0; i < users.Count; i++)
            {
                ret += users[i].ToString() + "\n";
            }
            System.Windows.MessageBox.Show(ret);
        }


        

        public void config_dbg()
        {
            string ret = "";
            foreach (User user in users)
            {
                ret += user.Get_ID() + ":\n";
                if (configuration.ContainsKey(user.Get_ID()))
                {
                    foreach (KeyValuePair<string, Configuration> config in configuration[user.Get_ID()])
                    {
                        bool su, sd, eu, ed;
                        su = config.Value.sms_up;
                        sd = config.Value.sms_down;
                        eu = config.Value.email_up;
                        ed = config.Value.email_down;
                        ret += su + " " + sd + " " + eu + " " + ed + "\n";
                    }
                }
                else
                    configuration[user.Get_ID()] = new Dictionary<string, Configuration>();
                ret += "--------------\n";
            }
            System.Windows.MessageBox.Show(ret);
        }

        public void Set_Message(string key, Message msg)
        {
            if (messages.ContainsKey(key))
                messages[key] = msg;
            else
            {
                messages[key] = new Message();
                messages[key] = msg;
            }
        }

        public void Create_Message(string key)
        {
            if(!messages.ContainsKey(key))
                messages[key] = new Message("", "");
        }

        public void msgs_dbg()
        {
            string ret = "";
            foreach (KeyValuePair<string, Message> msg in Singleton.Instance.messages)
            {
                //ret += msg.Key + " " + msg.Value.sms + ", " + msg.Value.email + "\n";
                ret += msg.Key + " " + Singleton.Instance.messages[msg.Key].sms + ", " + Singleton.Instance.messages[msg.Key].email + "\n";
            }
            System.Windows.MessageBox.Show(ret);
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
