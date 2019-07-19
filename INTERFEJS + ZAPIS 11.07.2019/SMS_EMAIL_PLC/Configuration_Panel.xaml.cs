using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public partial class Configuration_Panel : Window
    {
        string user_id;
        public Configuration_Panel(string id, Dictionary<string, Configuration> config)
        {
            InitializeComponent();
            this.user_id = id;
            this.Title = "użytkownik:" + id;
            Add_Lines(config);
        }

        void Add_Lines(Dictionary<string, Configuration> config)
        {
            foreach(KeyValuePair<string, Message> msg in Singleton.Instance.messages)
            {
                bool already_in = false;
                if (config.ContainsKey(msg.Key))
                {
                    already_in = true;
                }

                TextBlock idBlock = new TextBlock();
                idBlock.Text = msg.Key;
                idBlock.HorizontalAlignment = HorizontalAlignment.Center;
                idBlock.Width = 100;
                idBlock.Height = 20;
                ID_Panel.Children.Add(idBlock);

                CheckBox sms_upBox = new CheckBox();
                if (already_in)
                    sms_upBox.IsChecked = (bool)config[msg.Key].sms_up;
                sms_upBox.Name = "sup" + SMS_UP_Panel.Children.Count;
                sms_upBox.Height = 20;
                sms_upBox.HorizontalAlignment = HorizontalAlignment.Center;
                SMS_UP_Panel.Children.Add(sms_upBox);

                CheckBox sms_downBox = new CheckBox();
                if(already_in)
                    sms_downBox.IsChecked = (bool)config[msg.Key].sms_down;
                sms_downBox.Name = "sdn" + SMS_DOWN_Panel.Children.Count;
                sms_downBox.Height = 20;
                sms_downBox.HorizontalAlignment = HorizontalAlignment.Center;
                SMS_DOWN_Panel.Children.Add(sms_downBox);

                /*CheckBox email_upBox = new CheckBox();
                if (already_in)
                    email_upBox.IsChecked = (bool)config[msg.Key].email_up;
                email_upBox.Name = "eup" + EMAIL_UP_Panel.Children.Count;
                email_upBox.Height = 20;
                email_upBox.HorizontalAlignment = HorizontalAlignment.Center;
                EMAIL_UP_Panel.Children.Add(email_upBox);

                CheckBox email_downBox = new CheckBox();
                if (already_in)
                    email_downBox.IsChecked = (bool)config[msg.Key].email_down;
                email_downBox.Name = "edn" + EMAIL_DOWN_Panel.Children.Count;
                email_downBox.Height = 20;
                email_downBox.HorizontalAlignment = HorizontalAlignment.Center;
                EMAIL_DOWN_Panel.Children.Add(email_downBox);*/
            }
        }
        private void SaveButton_Click(Object sender, EventArgs e)
        {
            for (int it = 1; it < ID_Panel.Children.Count; it++)
            {
                bool sms_up = (bool)((CheckBox)SMS_UP_Panel.Children[it]).IsChecked;
                bool sms_down = (bool)((CheckBox)SMS_DOWN_Panel.Children[it]).IsChecked;
                bool email_up = false;//(bool)((CheckBox)EMAIL_UP_Panel.Children[it]).IsChecked;
                bool email_down = false;//(bool)((CheckBox)EMAIL_DOWN_Panel.Children[it]).IsChecked;


                Configuration config = new Configuration(sms_up, email_up, sms_down, email_down);
                string msg_id = ((TextBlock)ID_Panel.Children[it]).Text;
                Singleton.Instance.configuration[user_id][msg_id] = config;
            }
            this.Close();
        }
    }
}
