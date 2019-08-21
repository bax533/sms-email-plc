using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
    public partial class Users_Page : Page
    {
        /*Button save_users_button = new Button
        {
            Name = "SaveUsers_Button",
            Content = "eksportuj",
            Margin = new Thickness(0, 0, 5, 0),
            Height = 20,
            Width = 120,
            VerticalContentAlignment = VerticalAlignment.Center,
            ToolTip = "Eksportuj użytkowników do pliku"
        };


        Button load_users_button = new Button
        {
            Name = "LoadUsers_Button",
            Content = "importuj",
            Margin = new Thickness(0, 0, 5, 0),
            Height = 20,
            Width = 120,
            VerticalContentAlignment = VerticalAlignment.Center,
            ToolTip = "Wczytaj użytkowników z pliku"
        };



        Button load_users_from_database_button = new Button
        {
            Content = "wczytaj z bazy",
            Margin = new Thickness(0, 0, 5, 0),
            Height = 20,
            Width = 120,
            VerticalContentAlignment = VerticalAlignment.Center,
            ToolTip = "Wczytaj użytkowników z Bazy SQL"
        };
        */

        public Users_Page()
        {
            InitializeComponent();
        }


        public void Window_Clear()
        {
            ID_Panel.Children.RemoveRange(0, ID_Panel.Children.Count);
            Name_Panel.Children.RemoveRange(0, Name_Panel.Children.Count);
            PhoneNumber_Panel.Children.RemoveRange(0, PhoneNumber_Panel.Children.Count);
            Email_Panel.Children.RemoveRange(0, Email_Panel.Children.Count);
            RemoveButtons_Panel.Children.RemoveRange(0, RemoveButtons_Panel.Children.Count);
        }


        public void Add_Line(string id, string name, string phone_number, string email)
        {
            TextBlock IDBox = new TextBlock
            {
                Text = id,
                Width = 50,
                Height = 20,
                FontSize = 15,
                TextAlignment = TextAlignment.Center
            };
            ID_Panel.Children.Add(IDBox);

            TextBlock NameBox = new TextBlock
            {
                Text = name,
                Width = 150,
                Height = 20,
                FontSize = 15,
                TextAlignment = TextAlignment.Center
            };
            Name_Panel.Children.Add(NameBox);

            TextBlock NrBox = new TextBlock
            {
                Text = phone_number,
                Width = 100,
                Height = 20,
                FontSize = 15,
                TextAlignment = TextAlignment.Center
            };
            PhoneNumber_Panel.Children.Add(NrBox);

            TextBlock EmailBox = new TextBlock
            {
                Text = email,
                Width = 150,
                Height = 20,
                FontSize = 15,
                TextAlignment = TextAlignment.Center
            };
            Email_Panel.Children.Add(EmailBox);

            /*Border border = new Border
            {
                BorderThickness = new Thickness(1, 1, 1, 1),
                BorderBrush = Brushes.Black,
                CornerRadius = new CornerRadius(4)
            };*/

            Button removeButton = new Button
            {
                Height = 20,
                VerticalContentAlignment = VerticalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Name = "rmv" + RemoveButtons_Panel.Children.Count.ToString(),
                Content = "-",
                Visibility = Visibility.Hidden
            };
            removeButton.Click += RemoveButton_Click;
            //border.Child = removeButton;
            RemoveButtons_Panel.Children.Add(removeButton);
        }

        private void RemoveExpand_Click(Object sender, EventArgs e)
        {
            if (RemoveExpand_Button.Content.Equals("Usuwanie v"))
                RemoveExpand_Button.Content = "Usuwanie ^";
            else
                RemoveExpand_Button.Content = "Usuwanie v";

            for (int i = 0; i < RemoveButtons_Panel.Children.Count; i++)
            {
                Button current = ((Button)RemoveButtons_Panel.Children[i]);
                current.Visibility = current.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (Singleton.Instance.Admin)
            {
                AddUser_Dialog dialog = new AddUser_Dialog();
                dialog.Show();
            }
            else
                Singleton.Show_MessageBox("Niewystarczające uprawnienia!");
        }


        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (Singleton.Instance.Admin)
            {
                if (Singleton.Show_MessageBox("Czy napewno chcesz usunąć użytkownika?", true))
                {
                    Button thisButton = (Button)sender;
                    int it = Singleton.Get_Nr_From_Object(thisButton);
                    string id = ((TextBlock)ID_Panel.Children[it]).Text;
                    ID_Panel.Children.RemoveAt(it);
                    Name_Panel.Children.RemoveAt(it);
                    PhoneNumber_Panel.Children.RemoveAt(it);
                    Email_Panel.Children.RemoveAt(it);
                    RemoveButtons_Panel.Children.RemoveAt(it);

                    for (int i = it; i < ID_Panel.Children.Count; i++)
                    {
                        ((Button)RemoveButtons_Panel.Children[i]).Name = "rmv" + i.ToString();
                    }
                    Singleton.Instance.Remove_User(id);
                    Singleton.Instance.Remove_User_From_Configuration(id);
                    Singleton.Instance.configuration_page.Refresh();
                }

            }
            else
                Singleton.Show_MessageBox("Niewystarczające uprawnienia!");
        }
    }
}
     