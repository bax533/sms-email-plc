using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SMS_EMAIL_PLC
{
    public partial class Driver_Window : Window
    {
        public Driver_Window()
        {
            InitializeComponent();
        }

        private void Save(Object sender, EventArgs e)
        {
            if (Singleton.Instance.plc_manager.connected)
            {
                if (Singleton.Instance.plc_manager.connected)
                {
                    for (int i = 1; i < AdressesUP_Panel.Children.Count; i++)
                    {
                        try
                        {
                            string adres = ((TextBox)AdressesUP_Panel.Children[i]).Text;
                            string val = ((TextBox)ChangesUP_Panel.Children[i]).Text;
                            Singleton.Instance.plc_manager.Set_Int_Value(adres, Int16.Parse(val));
                        }
                        catch (Exception ex)
                        { }
                    }

                    for (int i = 1; i < AdressesDown_Panel.Children.Count; i++)
                    {
                        try
                        {
                            string adres = ((TextBox)AdressesDown_Panel.Children[i]).Text;
                            string val = ((TextBox)ChangesDown_Panel.Children[i]).Text;
                            Singleton.Instance.plc_manager.Set_Int_Value(adres, Int16.Parse(val));
                        }
                        catch (Exception ex)
                        { }
                    }
                }
            }
        }

        public void OnTimedEvent()
        {
            if(Singleton.Instance.plc_manager.connected)
            { 
                for(int i=1; i< AdressesUP_Panel.Children.Count; i++)
                {
                    try
                    {
                        string adres = ((TextBox)AdressesUP_Panel.Children[i]).Text;
                        ((TextBox)ValuesUP_Panel.Children[i]).Text = Singleton.Instance.plc_manager.Get_Int_Value(adres).ToString();
                    }
                    catch(Exception ex)
                    { }
                }

                for (int i = 1; i < AdressesDown_Panel.Children.Count; i++)
                {
                    try
                    {
                        string adres = ((TextBox)AdressesDown_Panel.Children[i]).Text;
                        ((TextBox)ValuesDown_Panel.Children[i]).Text = Singleton.Instance.plc_manager.Get_Int_Value(adres).ToString();
                    }
                    catch (Exception ex)
                    { }
                }
            }
        }

        private void DriverWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Singleton.Instance.driver_window.Visibility = Visibility.Collapsed;
        }

        public void Add_Line_Down(string name)
        {
            TextBox NameBox = new TextBox();
            NameBox.Text = name;
            NameBox.Width = 100;
            NameBox.Height = 20;
            NameBox.FontSize = 15;
            NameBox.TextAlignment = TextAlignment.Center;
            AdressesDown_Panel.Children.Add(NameBox);

            TextBox ValueBox = new TextBox();
            ValueBox.IsReadOnly = true;
            ValueBox.Text = "";
            ValueBox.Width = 100;
            ValueBox.Height = 20;
            ValueBox.FontSize = 15;
            ValueBox.TextAlignment = TextAlignment.Center;
            ValuesDown_Panel.Children.Add(ValueBox);

            TextBox ChangeBox = new TextBox();
            ChangeBox.Text = "-";
            ChangeBox.Width = 100;
            ChangeBox.Height = 20;
            ChangeBox.FontSize = 15;
            ChangeBox.TextAlignment = TextAlignment.Center;
            ChangesDown_Panel.Children.Add(ChangeBox);
        }


        public void Add_Line_UP(string name)
        {
            TextBox NameBox = new TextBox();
            NameBox.Text = name;
            NameBox.Width = 100;
            NameBox.Height = 20;
            NameBox.FontSize = 15;
            NameBox.TextAlignment = TextAlignment.Center;
            AdressesUP_Panel.Children.Add(NameBox);

            TextBox ValueBox = new TextBox();
            ValueBox.IsReadOnly = true;
            ValueBox.Text = "";
            ValueBox.Width = 100;
            ValueBox.Height = 20;
            ValueBox.FontSize = 15;
            ValueBox.TextAlignment = TextAlignment.Center;
            ValuesUP_Panel.Children.Add(ValueBox);

            TextBox ChangeBox = new TextBox();
            ChangeBox.Text = "-";
            ChangeBox.Width = 100;
            ChangeBox.Height = 20;
            ChangeBox.FontSize = 15;
            ChangeBox.TextAlignment = TextAlignment.Center;
            ChangesUP_Panel.Children.Add(ChangeBox);
        }

        public string Get_Dialog(string starting)
        {
            SMS_Dialog dialog = new SMS_Dialog(starting);
            dialog.ShowDialog();
            return dialog.Result;
        }

        private void AddButtonUP_Click(Object sender, EventArgs e)
        {
            string adres = Get_Dialog("");
            Driver driver = new Driver(adres);
            try
            {
                int test = Singleton.Instance.plc_manager.Get_Int_Value(adres);

                Singleton.Instance.toControlUP.Add(driver);
                Add_Line_UP(adres);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("niepoprawny adres komórki");
            }
            
        }

        private void dbg_Click(Object sender, EventArgs e)
        {
            string ret = "";
            foreach(Driver driver in Singleton.Instance.toControlUP)
            {
                ret += driver.name + "\n";
            }
            ret += "========\n";
            foreach (Driver driver in Singleton.Instance.toControlDown)
            {
                ret += driver.name + "\n";
            }
            ret += "=========";
            System.Windows.MessageBox.Show(ret);

        }

        private void AddButtonDown_Click(Object sender, EventArgs e)
        {
            string adres = Get_Dialog("");
            Driver driver = new Driver(adres);
            try
            {
                int test = Singleton.Instance.plc_manager.Get_Int_Value(adres);

                Singleton.Instance.toControlDown.Add(driver);
                Add_Line_Down(adres);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("niepoprawny adres komórki");
            }

        }

    }
}
