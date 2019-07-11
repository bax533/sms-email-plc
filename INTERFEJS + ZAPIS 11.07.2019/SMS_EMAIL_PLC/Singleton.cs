using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;

namespace SMS_EMAIL_PLC
{
    class Singleton
    {
        private System.Timers.Timer timer;

        public List<Driver> toControl = new List<Driver>();
        public SMS_Manager sms_manager = new SMS_Manager();
        public Email_Manager email_manager = new Email_Manager();
        public PLC_Manager plc_manager = new PLC_Manager();
        public SQL_Manager sql_manager = new SQL_Manager();

        public Users_Window users_window = new Users_Window();
        public Messages_Window messages_window = new Messages_Window();
        public Configuration_Window configuration_window = new Configuration_Window();
        public MainWindow main_window;

        public List<User> users = new List<User>();
        
        public Dictionary<string, Dictionary<string, Configuration>> configuration = new Dictionary<string, Dictionary<string, Configuration>>();


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
            timer = new System.Timers.Timer(interval);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        public void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            foreach (Driver driver in toControl)
            {
                //Console.WriteLine((int)plc_manager.Get_Int_Value(driver.name)+ " = "+ driver.val);
                try
                {
                    if (plc_manager.Get_Int_Value(driver.name) == driver.val && !driver.already_alarmed)
                    {
                        Console.WriteLine("ALERT " + driver.name);
                        driver.already_alarmed = true;
                    }
                    else if (driver.already_alarmed && plc_manager.Get_Int_Value(driver.name) != driver.val)
                    {
                        driver.already_alarmed = false;
                        Console.WriteLine("KONIEC " + driver.name);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
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

        public Dictionary<string, Message> messages = new Dictionary<string, Message>();

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
