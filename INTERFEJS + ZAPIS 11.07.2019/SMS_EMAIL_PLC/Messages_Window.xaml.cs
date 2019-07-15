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
    
    
    public partial class Messages_Window : Window
    {
        public Messages_Window()
        {
            InitializeComponent();
        }

        void MessagesWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Singleton.Instance.messages_window.Visibility = Visibility.Collapsed;
        }

        public string Get_Dialog(string starting)
        {
            SMS_Dialog dialog = new SMS_Dialog(starting);
            dialog.ShowDialog();
            return dialog.Result;
        }

        private void SMS_Click(object sender, EventArgs e)
        {
            int it = Singleton.Get_Nr_From_Object(sender);
            string number = ((TextBox)Number_Panel.Children[it]).Text;
            string content = Get_Dialog(Singleton.Instance.Get_SMS(number));

            Singleton.Instance.Set_SMS(number, content);
            //System.Windows.MessageBox.Show(it.ToString());
        }

        private void Email_Click(object sender, EventArgs e)
        {
            int it = Singleton.Get_Nr_From_Object(sender);
            string number = ((TextBox)Number_Panel.Children[it]).Text;
            string content = Get_Dialog(Singleton.Instance.Get_Email(number));

            Singleton.Instance.Set_Email(number, content);
            //System.Windows.MessageBox.Show(it.ToString());
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            string content = Get_Dialog("podaj numer wiadomości");
            Add_Line(content, "opis");
        }

        public void Add_Line(string nr, string description)
        {
            Singleton.Instance.Create_Message(nr);

            TextBox NumberBox = new TextBox();
            NumberBox.IsReadOnly = true;
            NumberBox.Text = nr;
            NumberBox.Width = 50;
            NumberBox.Height = 20;
            NumberBox.HorizontalAlignment = HorizontalAlignment.Center;
            Number_Panel.Children.Add(NumberBox);

            TextBox DescriptionBox = new TextBox();
            DescriptionBox.Text = description;
            DescriptionBox.Width = 200;
            DescriptionBox.Height = 20;
            DescriptionBox.TextAlignment = TextAlignment.Center;
            Description_Panel.Children.Add(DescriptionBox);

            StackPanel Buttons_Panel = new StackPanel();
            Buttons_Panel.Orientation = Orientation.Horizontal;
                Button SMS_Button = new Button();
                SMS_Button.Name = "SMS" + Messages_Panel.Children.Count.ToString();
                SMS_Button.Content = "SMS";
                SMS_Button.Height = 20;
                SMS_Button.Width = 75;
                SMS_Button.Click += SMS_Click;

                Button Email_Button = new Button();
                Email_Button.Name = "EML" + Messages_Panel.Children.Count.ToString();
                Email_Button.Content = "Email";
                Email_Button.Height = 20;
                Email_Button.Width = 75;
                Email_Button.Click += Email_Click;
            Buttons_Panel.Children.Add(SMS_Button);
            Buttons_Panel.Children.Add(Email_Button);
            Messages_Panel.Children.Add(Buttons_Panel);

            Button removeButton = new Button();
            removeButton.Height = 20;
            removeButton.Name = "rmv" + RemoveButtons_Panel.Children.Count.ToString();
            removeButton.Content = "-";
            removeButton.Click += RemoveButton_Click;
            RemoveButtons_Panel.Children.Add(removeButton);
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            Button thisButton = (Button)sender;
            int it = Singleton.Get_Nr_From_Object(thisButton);

            string nr = ((TextBox)Number_Panel.Children[it]).Text;

            Singleton.Instance.Remove_Message(nr);
            Singleton.Instance.Remove_From_Configuration(nr);
            //Singleton.Instance.resources.Remove_SMS(nr);
            //Singleton.Instance.resources.Remove_Email(nr);

            Number_Panel.Children.RemoveAt(it);
            Description_Panel.Children.RemoveAt(it);
            Messages_Panel.Children.RemoveAt(it);
            RemoveButtons_Panel.Children.RemoveAt(it);

            for (int i = it; i < Number_Panel.Children.Count; i++)
            {
                ((Button)RemoveButtons_Panel.Children[i]).Name = "rmv" + i.ToString();
                ((Button)((StackPanel)Messages_Panel.Children[i]).Children[0]).Name = "SMS" + i.ToString();
                ((Button)((StackPanel)Messages_Panel.Children[i]).Children[1]).Name = "EML" + i.ToString();
            }
        }

        public void Window_Clear()
        {
            Number_Panel.Children.RemoveRange(1, Number_Panel.Children.Count - 1);
            Description_Panel.Children.RemoveRange(1, Description_Panel.Children.Count - 1);
            Messages_Panel.Children.RemoveRange(1, Messages_Panel.Children.Count - 1);
            RemoveButtons_Panel.Children.RemoveRange(1, RemoveButtons_Panel.Children.Count - 1);
        }


        private void dbg_Click(Object sender, EventArgs e)
        {
            Singleton.Instance.msgs_dbg();
        }

        private void LoadBase_Click(Object sender, EventArgs e)
        {
            Singleton.Instance.Clear_Configuration();
            Singleton.Instance.sql_manager.Load_Messages();
        }
    }
}
