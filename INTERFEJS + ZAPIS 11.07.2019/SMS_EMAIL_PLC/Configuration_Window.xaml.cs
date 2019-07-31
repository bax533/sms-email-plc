using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class Configuration_Window : Window
    {
        public Configuration_Window()
        {
            InitializeComponent();
            Toolbar_Panel.Children.Add(new My_Toolbar());
        }

        void ConfigurationWindow_Closing(object sender, CancelEventArgs e)
        {
            Singleton.Instance.Close_Application();
        }

        public void Clear_Window()
        {
            try
            {
                ID_Panel.Children.RemoveRange(1, ID_Panel.Children.Count - 1);
                Users_Panel.Children.RemoveRange(1, Users_Panel.Children.Count - 1);
                Config_Panel.Children.RemoveRange(1, Config_Panel.Children.Count - 1);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public void Add_Users()
        {
            foreach(User user in Singleton.Instance.users)
                Add_Line(user.Get_ID(), user.Get_Name());
        }

        private void ConfigButton_Click(Object sender, EventArgs e)
        {
            int it = Singleton.Get_Nr_From_Object(sender);
            string user_id = ((TextBlock)ID_Panel.Children[it]).Text;

            if (!Singleton.Instance.configuration.ContainsKey(user_id))
                Singleton.Instance.configuration[user_id] = new Dictionary<string, Configuration>();

            Configuration_Panel panel = new Configuration_Panel(user_id, Singleton.Instance.configuration[user_id]);
            panel.Show();
        }

        public void Add_Line(string user_id, string username)
        {
            TextBlock idBlock = new TextBlock
            {
                Text = user_id,
                Width = 50,
                Height = 20
            };
            ID_Panel.Children.Add(idBlock);

            TextBlock userBlock = new TextBlock
            {
                Text = username,
                Width = 150,
                Height = 20
            };
            Users_Panel.Children.Add(userBlock);

            Button configButton = new Button
            {
                Content = "konfiguracja",
                Width = 90,
                Height = 20,
                Name = "cnf" + Config_Panel.Children.Count
            };
            configButton.Click += ConfigButton_Click;
            Config_Panel.Children.Add(configButton);
        }

        public void Refresh()
        {
            Clear_Window();
            Add_Users();
        }
    }
}
