using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
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
    
    
    public partial class Messages_Page : Page
    {

        /*Button SaveUsers_Button = new Button { Height=20, Width=120, Content="eksportuj", Margin = new Thickness(0, 0, 7.5, 0), FontSize=12, HorizontalAlignment=HorizontalAlignment.Center};
        Button LoadUsers_Button = new Button { Height = 20, Width = 120, Content = "wczytaj", Margin = new Thickness(0, 0, 7.5, 0), FontSize = 12, HorizontalAlignment = HorizontalAlignment.Center };
        Button LoadBase_Button = new Button { Height = 20, Width = 120, Content = "wczytaj z bazy", Margin = new Thickness(0, 0, 0, 0), FontSize = 12, HorizontalAlignment = HorizontalAlignment.Center };
        */
        public Messages_Page()
        {
            InitializeComponent();
        }

        void MessagesWindow_Closing(object sender, CancelEventArgs e)
        {
            Singleton.Instance.Close_Application();
        }

        private void SMS_Click(object sender, EventArgs e)
        {
            int it = Singleton.Get_Nr_From_Object(sender);
            string number = ((TextBox)Number_Panel.Children[it]).Text;
            //string content = Get_Dialog(Singleton.Instance.Get_SMS(number));

            /*if (content.Length > 160)
            {
                Singleton.Show_MessageBox("przekroczono limit (160 znaków)");
            }
            else
            {
                ((TextBlock)((StackPanel)Messages_Panel.Children[it]).Children[1]).Text = content;
               // Singleton.Instance.Set_SMS(number, content);
            }*/
            //Singleton.Show_MessageBox(it.ToString());
        }

        private void Email_Click(object sender, EventArgs e)
        {
            int it = Singleton.Get_Nr_From_Object(sender);
            string number = ((TextBox)Number_Panel.Children[it]).Text;
            //string content = Get_Dialog(Singleton.Instance.Get_Email(number));

            //Singleton.Instance.Set_Email(number, content);
            //Singleton.Show_MessageBox(it.ToString());
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (Singleton.Instance.Admin)
            {
                AddMessage_Dialog msg_dialog = new AddMessage_Dialog();
                msg_dialog.ShowDialog();
                string id = msg_dialog.ID;
                if (msg_dialog.closed)
                    return;
                string comment = msg_dialog.Comment;

                if (Singleton.Instance.Add_Message(id, new Message_Add(comment)))
                {
                    Add_Line(id, comment, true);
                }
            }
            else
                Singleton.Show_MessageBox("Niewystarczające uprawnienia!");
        }

        public void Add_Line(string nr, string comment, bool active)
        {
           // Singleton.Instance.Create_Message(nr);

            TextBlock NumberBox = new TextBlock
            {
                Text = nr,
                Width = 50,
                Height = 20,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Number_Panel.Children.Add(NumberBox);

            TextBlock Comment_Block = new TextBlock
            {
                Name = "CMT" + Comments_Panel.Children.Count.ToString(),
                TextWrapping = TextWrapping.Wrap,
                Text = comment,
                Width = 125,
                Height = 20
            };
            Comments_Panel.Children.Add(Comment_Block);

            CheckBox Active_Box = new CheckBox
            {
                Name = "ACT" + Active_Panel.Children.Count.ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 50,
                Height = 20,
                IsChecked = active
            };
            Active_Box.Click += ActiveBox_Click;
            Active_Panel.Children.Add(Active_Box);

            Button removeButton = new Button
            {
                Height = 20,
                Name = "rmv" + RemoveButtons_Panel.Children.Count.ToString(),
                Content = "-",
                Visibility = Visibility.Hidden
            };
            removeButton.Click += RemoveButton_Click;
            RemoveButtons_Panel.Children.Add(removeButton);
        }

        void ActiveBox_Click(Object sender, EventArgs e)
        {
            int it = Singleton.Get_Nr_From_Object(sender);
            bool newActive = (bool)((CheckBox)sender).IsChecked;
            string id = ((TextBlock)Number_Panel.Children[it]).Text;

            Singleton.Instance.SetMessageActive(id, newActive);
        }


        private void RemoveExpand_Click(Object sender, EventArgs e)
        {
            if (RemoveExpand_Button.Content.Equals("Usuwanie v"))
                RemoveExpand_Button.Content = "Usuwanie ^";
            else
                RemoveExpand_Button.Content = "Usuwanie v";

            for (int i=0; i<RemoveButtons_Panel.Children.Count; i++)
            {
                Button current = ((Button)RemoveButtons_Panel.Children[i]);
                current.Visibility = current.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            }
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (Singleton.Instance.Admin)
            {
                if (Singleton.Show_MessageBox("Czy na pewno chcesz usunąć tą wiadomość?", true))
                {
                    Button thisButton = (Button)sender;
                    int it = Singleton.Get_Nr_From_Object(thisButton);

                    string nr = ((TextBlock)Number_Panel.Children[it]).Text;

                    Singleton.Instance.Remove_Message_From_Dict(nr);
                    Singleton.Instance.Remove_Msg_From_Configuration(nr);

                    Number_Panel.Children.RemoveAt(it);
                    //Description_Panel.Children.RemoveAt(it);
                    Comments_Panel.Children.RemoveAt(it);
                    Active_Panel.Children.RemoveAt(it);
                    RemoveButtons_Panel.Children.RemoveAt(it);

                    for (int i = it; i < Number_Panel.Children.Count; i++)
                    {
                        ((Button)RemoveButtons_Panel.Children[i]).Name = "rmv" + i.ToString();
                        ((TextBlock)Comments_Panel.Children[i]).Name = "CMT" + i.ToString();
                        ((CheckBox)Active_Panel.Children[i]).Name = "ACT" + i.ToString();
                    }
                }
            }
            else
                Singleton.Show_MessageBox("Niewystarczające uprawnienia!");
        }

        public void Window_Clear()
        {
            Number_Panel.Children.RemoveRange(0, Number_Panel.Children.Count);
            //Description_Panel.Children.RemoveRange(1, Description_Panel.Children.Count - 1);
            Comments_Panel.Children.RemoveRange(0, Comments_Panel.Children.Count);
            Active_Panel.Children.RemoveRange(0, Active_Panel.Children.Count);
            RemoveButtons_Panel.Children.RemoveRange(0, RemoveButtons_Panel.Children.Count);
        }

    }
}
