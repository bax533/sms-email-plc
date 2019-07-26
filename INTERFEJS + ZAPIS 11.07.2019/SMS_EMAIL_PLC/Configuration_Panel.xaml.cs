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
            foreach(KeyValuePair<string, Configuration> cnf in config)
            {
                string msg_id = cnf.Key;

                TextBlock idBlock = new TextBlock
                {
                    Text = msg_id,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 100,
                    Height = 20
                };
                ID_Panel.Children.Add(idBlock);

                CheckBox sms_upBox = new CheckBox
                {
                    IsChecked = cnf.Value.sms_up,
                    Name = "sup" + SMS_UP_Panel.Children.Count,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    IsHitTestVisible = false,
                    Focusable = false
                };
                SMS_UP_Panel.Children.Add(sms_upBox);

                CheckBox sms_downBox = new CheckBox
                {
                    IsChecked = cnf.Value.sms_down,
                    Name = "sdn" + SMS_DOWN_Panel.Children.Count,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    IsHitTestVisible = false,
                    Focusable = false
                };
                SMS_DOWN_Panel.Children.Add(sms_downBox);


                CheckBox email_upBox = new CheckBox
                {
                    IsChecked = cnf.Value.email_up,
                    Name = "eup" + EMAIL_UP_Panel.Children.Count,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    IsHitTestVisible = false,
                    Focusable = false
                };
                EMAIL_UP_Panel.Children.Add(email_upBox);

                CheckBox email_downBox = new CheckBox
                {
                    IsChecked = cnf.Value.email_down,
                    Name = "edn" + EMAIL_DOWN_Panel.Children.Count,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    IsHitTestVisible = false,
                    Focusable = false
                };
                EMAIL_DOWN_Panel.Children.Add(email_downBox);


                Button removeButton = new Button
                {
                    Height = 20,
                    Name = "rmv" + Remove_Panel.Children.Count.ToString(),
                    Content = "-"
                };
                removeButton.Click += RemoveButton_Click;
                Remove_Panel.Children.Add(removeButton);
            }
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            Button thisButton = (Button)sender;
            int it = Singleton.Get_Nr_From_Object(thisButton);
            string msg_id = ((TextBlock)ID_Panel.Children[it]).Text;
            ID_Panel.Children.RemoveAt(it);
            SMS_UP_Panel.Children.RemoveAt(it);
            SMS_DOWN_Panel.Children.RemoveAt(it);
            EMAIL_UP_Panel.Children.RemoveAt(it);
            EMAIL_DOWN_Panel.Children.RemoveAt(it);
            Remove_Panel.Children.RemoveAt(it);

            for (int i = it; i < ID_Panel.Children.Count; i++)
            {
                ((Button)Remove_Panel.Children[i]).Name = "rmv" + i.ToString();
            }
            Singleton.Instance.configuration[user_id].Remove(msg_id);
            Refresh();
        }

        private void Save_Click(Object sender, EventArgs e)
        {
            if (Singleton.Instance.configuration.ContainsKey(user_id))
            {
                string msg_id = msgID_AddBox.Text;
                bool sms_up = (bool)SMSup_AddBox.IsChecked;
                bool sms_down = (bool)SMSdown_AddBox.IsChecked;

                bool email_up = (bool)EMAILup_AddBox.IsChecked;
                bool email_down = (bool)EMAILdown_AddBox.IsChecked;

                for (int i = 1; i < ID_Panel.Children.Count; i++)
                {
                    if (((TextBlock)ID_Panel.Children[i]).Text.Equals(msg_id))
                    {
                        System.Windows.MessageBox.Show("najpierw usuń tą wiadomość użytkownikowi!");
                        return;
                    }
                }

                Singleton.Instance.configuration[user_id][msg_id] = new Configuration(sms_up, email_up, sms_down, email_down);
                this.Refresh();
            }
        }

        private void Window_Clear()
        {
            ID_Panel.Children.RemoveRange(1, ID_Panel.Children.Count - 1);
            SMS_UP_Panel.Children.RemoveRange(1, SMS_UP_Panel.Children.Count - 1);
            SMS_DOWN_Panel.Children.RemoveRange(1, SMS_DOWN_Panel.Children.Count - 1);
            EMAIL_UP_Panel.Children.RemoveRange(1, EMAIL_UP_Panel.Children.Count - 1);
            EMAIL_DOWN_Panel.Children.RemoveRange(1, EMAIL_DOWN_Panel.Children.Count - 1);
            Remove_Panel.Children.RemoveRange(1, Remove_Panel.Children.Count - 1);
        }

        public void Refresh()
        {
            Window_Clear();
            if(Singleton.Instance.configuration.ContainsKey(user_id))
                Add_Lines(Singleton.Instance.configuration[user_id]);
        }
    }
}
