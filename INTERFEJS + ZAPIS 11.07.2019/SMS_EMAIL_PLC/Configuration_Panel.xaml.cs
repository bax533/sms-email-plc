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

    public class CheckBox_Panel : StackPanel
    {
        public CheckBox sms_up;
        public CheckBox sms_down;
        public CheckBox email_up;
        public CheckBox email_down;

        string user_id, msg_id;

        public CheckBox_Panel(string name)
        {
            Name = name;
            Orientation = Orientation.Horizontal;
            Height = 20;

            sms_up = new CheckBox { IsEnabled = false, Focusable = false, Height = 20, Name = "SMSup_AddBox", VerticalContentAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.WhiteSmoke, Margin = new Thickness(40, 0, 40, 0) };
            sms_down = new CheckBox { IsEnabled = false, Focusable = false,  Height = 20, Name = "SMSdown_AddBox", VerticalContentAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.WhiteSmoke, Margin = new Thickness(40, 0, 50, 0) };
            email_up = new CheckBox { IsEnabled = false, Focusable = false, Height = 20, Name = "Emailup_AddBox", VerticalContentAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.WhiteSmoke, Margin = new Thickness(35, 0, 40, 0) };
            email_down = new CheckBox { IsEnabled = false, Focusable = false, Height = 20, Name = "Emaildown_AddBox", VerticalContentAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.WhiteSmoke, Margin = new Thickness(35, 0, 40, 0) };

            Children.Add(sms_up);
            Children.Add(sms_down);
            Children.Add(email_up);
            Children.Add(email_down);
        }
        public CheckBox_Panel(string name, string user_id, string msg_id, Configuration config)
        {
            Name = name;
            this.user_id = user_id;
            this.msg_id = msg_id;
            Orientation = Orientation.Horizontal;
            Height = 20;

            sms_up = new CheckBox { IsChecked = config.sms_up, IsEnabled = false, Focusable = false, Height = 20, Name = "SMSup_AddBox", VerticalContentAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.WhiteSmoke, Margin = new Thickness(37, 0, 37, 0) };
            sms_down = new CheckBox { IsChecked = config.sms_down, IsEnabled = false, Focusable = false, Height = 20, Name = "SMSdown_AddBox", VerticalContentAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.WhiteSmoke, Margin = new Thickness(37, 0, 40, 0) };
            email_up = new CheckBox { IsChecked = config.email_up, IsEnabled = false, Focusable = false, Height = 20, Name = "Emailup_AddBox", VerticalContentAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.WhiteSmoke, Margin = new Thickness(40, 0, 37, 0) };
            email_down = new CheckBox { IsChecked = config.email_down, IsEnabled = false, Focusable = false, Height = 20, Name = "Emaildown_AddBox", VerticalContentAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.WhiteSmoke, Margin = new Thickness(37, 0, 37, 0) };

            sms_up.Click += ChangeConfig_Click;
            sms_down.Click += ChangeConfig_Click;
            email_up.Click += ChangeConfig_Click;
            email_down.Click += ChangeConfig_Click;

            Children.Add(sms_up);
            Children.Add(email_up);
            Children.Add(sms_down);
            Children.Add(email_down);
        }

        void ChangeConfig_Click(Object sender, EventArgs e)
        {
            Singleton.Instance.configuration[user_id][msg_id] = this.GetConfig();
        }

        public Configuration GetConfig()
        {
            return new Configuration((bool)sms_up.IsChecked, (bool)email_up.IsChecked, (bool)sms_down.IsChecked, (bool)email_down.IsChecked);
        }

        public void SetActive(bool value)
        {
            for (int i = 0; i < 4; i++)
            {
                ((CheckBox)Children[i]).IsEnabled = value;
                ((CheckBox)Children[i]).Focusable = value;
            }
        }


    }

    public partial class Configuration_Panel : Page
    {
        int selected_line_new = 0, selected_line_cur = 0;
        string user_id;
        public Configuration_Panel(string id, string name, Dictionary<string, Configuration> config)
        {
            InitializeComponent();
            this.user_id = id;
            TitleBlock.Text = "użytkownik: " + id + " " + name;
            Add_Lines(config);
            Add_Messages();
        }

        void Add_Lines(Dictionary<string, Configuration> config)
        {
            foreach(KeyValuePair<string, Configuration> cnf in config)
            {
                string msg_id = cnf.Key;

                Button selectCur_Button = new Button
                {
                    Content = ">",
                    Name = "SLC" + Select_Panel.Children.Count.ToString(),
                    Width = 50,
                    Height = 20,
                };
                selectCur_Button.Click += SelectLine_Click;
                Select_Panel.Children.Add(selectCur_Button);

                TextBlock idBlock = new TextBlock
                {
                    Text = msg_id,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 100,
                    Height = 20
                };
                ID_Panel.Children.Add(idBlock);

                ConfigurationCur_Panel.Children.Add(new CheckBox_Panel("CTR" + ConfigurationCur_Panel.Children.Count.ToString(), user_id, msg_id, cnf.Value));

                Button removeButton = new Button
                {
                    Height = 20,
                    Name = "rmv" + Remove_Panel.Children.Count.ToString(),
                    Content = "-",
                    Visibility = Visibility.Hidden
                };
                removeButton.Click += RemoveButton_Click;
                Remove_Panel.Children.Add(removeButton);
            }
        }

        void Add_Messages()
        {
            foreach (KeyValuePair<string, Message_Add> msg_in in Singleton.Instance.messages_dict)
            {
                Button line_Button = new Button
                {
                    Content = ">",
                    Name = "SLC" + SelectNew_Panel.Children.Count.ToString(),
                    Width = 50,
                    Height = 20,
                };
                line_Button.Click += SelectLineNew_Click;
                SelectNew_Panel.Children.Add(line_Button);

                TextBlock id_Block = new TextBlock
                {
                    Text = msg_in.Key,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 50,
                    Height = 20
                };
                TextBlock description_Block = new TextBlock
                {
                    Text = msg_in.Value.Comment,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 100,
                    Height = 20
                };
                Button add_Button = new Button
                {
                    Height = 20,
                    Name = "add" + AddNew_Panel.Children.Count.ToString(),
                    Content = "+",
                    Visibility = Visibility.Hidden
                };
                add_Button.Click += Save_Click;

                IDNew_Panel.Children.Add(id_Block);
                DescriptionNew_Panel.Children.Add(description_Block);
                ConfigurationNew_Panel.Children.Add(new CheckBox_Panel("CTR" + ConfigurationNew_Panel.Children.Count.ToString()));
                AddNew_Panel.Children.Add(add_Button);
            }
        }

        void ChangeConfig_Click(Object sender, EventArgs e)
        {
            int it = Singleton.Get_Nr_From_Object(sender);
            string msg_id = ((TextBlock)ID_Panel.Children[it]).Text;

            Singleton.Instance.configuration[user_id][msg_id] = ((CheckBox_Panel)ConfigurationCur_Panel.Children[it]).GetConfig();
        }


        void Set_Active_Current(bool value, int nr)
        {
            ((CheckBox_Panel)ConfigurationCur_Panel.Children[nr]).SetActive(value);
            for(int i=0; i<Remove_Panel.Children.Count; i++)
                ((Button)Remove_Panel.Children[i]).Visibility = Visibility.Hidden;
            ((Button)Remove_Panel.Children[nr]).Visibility = Visibility.Visible;
        }

        void SelectLine_Click(Object sender, EventArgs e)
        {
            if (selected_line_cur >= 0 && selected_line_cur < ID_Panel.Children.Count)
                Set_Active_Current(false, selected_line_cur);

            int it = Singleton.Get_Nr_From_Object(sender);
            Set_Active_Current(true, it);
            SetActiveLineCur(it);
            selected_line_cur = it;
        }

        void SelectLineNew_Click(Object sender, EventArgs e)
        {
            if (selected_line_new >= 0 && selected_line_new < ConfigurationNew_Panel.Children.Count)
                ((CheckBox_Panel)ConfigurationNew_Panel.Children[selected_line_new]).SetActive(false);


            int it = Singleton.Get_Nr_From_Object(sender);
            ((CheckBox_Panel)ConfigurationNew_Panel.Children[it]).SetActive(true);
            SetActiveLineNew(it);
            selected_line_new = it;
        }

        void SetActiveLineCur(int nr)
        {
            if (selected_line_cur >= 0 && selected_line_cur < ID_Panel.Children.Count)
            {
                ((TextBlock)ID_Panel.Children[selected_line_cur]).Background = BackgroundSample_Panel.Background;
                ((CheckBox_Panel)ConfigurationCur_Panel.Children[selected_line_cur]).Background = BackgroundSample_Panel.Background;
            }

            ((TextBlock)ID_Panel.Children[nr]).Background = Brushes.Green;
            ((CheckBox_Panel)ConfigurationCur_Panel.Children[nr]).Background = Brushes.Green;
        }

        void SetActiveLineNew(int nr)
        {
            if (selected_line_new >= 0 && selected_line_new < ConfigurationNew_Panel.Children.Count)
            {
                ((TextBlock)IDNew_Panel.Children[selected_line_new]).Background = BackgroundSample_Panel.Background;
                ((TextBlock)DescriptionNew_Panel.Children[selected_line_new]).Background = BackgroundSample_Panel.Background;
                ((CheckBox_Panel)ConfigurationNew_Panel.Children[selected_line_new]).Background = BackgroundSample_Panel.Background;
            }

            ((TextBlock)IDNew_Panel.Children[nr]).Background = Brushes.Green;
            ((TextBlock)DescriptionNew_Panel.Children[nr]).Background = Brushes.Green;
            ((CheckBox_Panel)ConfigurationNew_Panel.Children[nr]).Background = Brushes.Green;

            for (int i = 0; i < AddNew_Panel.Children.Count; i++)
                ((Button)AddNew_Panel.Children[i]).Visibility = Visibility.Hidden;
            ((Button)AddNew_Panel.Children[nr]).Visibility = Visibility.Visible;
        }


        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (Singleton.Instance.Admin)
            {
                if (Singleton.Show_MessageBox("Czy napewno chcesz usunąć tą wiadomość?", true))
                {
                    Button thisButton = (Button)sender;
                    int it = Singleton.Get_Nr_From_Object(thisButton);
                    string msg_id = ((TextBlock)ID_Panel.Children[it]).Text;

                    Select_Panel.Children.RemoveAt(it);
                    ID_Panel.Children.RemoveAt(it);
                    ConfigurationCur_Panel.Children.RemoveAt(it);
                    Remove_Panel.Children.RemoveAt(it);

                    for (int i = it; i < ID_Panel.Children.Count; i++)
                    {
                        ((Button)Select_Panel.Children[i]).Name = "SLC" + i.ToString();
                        ((Button)Remove_Panel.Children[i]).Name = "rmv" + i.ToString();
                        ((CheckBox_Panel)ConfigurationCur_Panel.Children[i]).Name = "CTR" + i.ToString();
                    }
                    Singleton.Instance.configuration[user_id].Remove(msg_id);
                    Refresh();
                }
            }
            else
                Singleton.Show_MessageBox("Niewystarczające uprawnienia!");
        }

        private void Save_Click(Object sender, EventArgs e)
        {
            if (Singleton.Instance.Admin)
            {
                if (Singleton.Instance.configuration.ContainsKey(user_id))
                {
                    int it = Singleton.Get_Nr_From_Object(sender);

                    if (it == selected_line_new)
                    {

                        string msg_id = ((TextBlock)IDNew_Panel.Children[it]).Text;

                        for (int i = 0; i < ID_Panel.Children.Count; i++)
                        {
                            if (((TextBlock)ID_Panel.Children[i]).Text.Equals(msg_id))
                            {
                                Singleton.Show_MessageBox("najpierw usuń tą wiadomość użytkownikowi!");
                                return;
                            }
                        }

                        CheckBox_Panel currentconfig_panel = ((CheckBox_Panel)ConfigurationNew_Panel.Children[it]);
                        Configuration currentconfig = new Configuration((bool)((CheckBox)currentconfig_panel.Children[0]).IsChecked, (bool)((CheckBox)currentconfig_panel.Children[1]).IsChecked, (bool)((CheckBox)currentconfig_panel.Children[2]).IsChecked, (bool)((CheckBox)currentconfig_panel.Children[3]).IsChecked);
                        Singleton.Instance.configuration[user_id][msg_id] = currentconfig;
                        this.Refresh();
                    }
                }
            }
            else
                Singleton.Show_MessageBox("Niewystarczające uprawnienia!");
        }

        void Back_Click(Object sender, EventArgs e)
        {
            Singleton.Instance.main_window.MainFrame.Content = Singleton.Instance.configuration_page;
        }

        private void Window_Clear()
        {
            Select_Panel.Children.RemoveRange(0, Select_Panel.Children.Count);
            ID_Panel.Children.RemoveRange(0, ID_Panel.Children.Count);
            ConfigurationCur_Panel.Children.RemoveRange(0, ConfigurationCur_Panel.Children.Count);
            Remove_Panel.Children.RemoveRange(0, Remove_Panel.Children.Count);
        }

        public void Refresh()
        {
            Window_Clear();
            if(Singleton.Instance.configuration.ContainsKey(user_id))
                Add_Lines(Singleton.Instance.configuration[user_id]);
        }
    }
}
