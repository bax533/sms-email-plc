using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMS_EMAIL_PLC
{
    class SQL_Manager
    {
        public SqlConnection cnn;
        public SqlCommand cmd = new SqlCommand();
        public SqlDataReader reader;
        private bool status = false;


        int Count_Rows(SqlCommand cmd)
        {
            System.Data.DataTable dt = new DataTable();
            Singleton.Instance.sql_manager.reader = cmd.ExecuteReader();
            dt.Load(Singleton.Instance.sql_manager.reader);
            int ret = dt.Rows.Count;
            Singleton.Instance.sql_manager.reader.Close();
            return ret;
        }


        public void Login(string username, string password, string serveradres, string database)
        {
            string connetionString = null;
            connetionString = $"Server={serveradres};" + $" Database={database};" + $"User Id={ username };" + $"Password = { password }; ";
            cnn = new SqlConnection(connetionString);
            try
            {
                cnn.Open();
                status = true;
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                status = false;
            }
            Singleton.Instance.main_window.sql_status_text.Foreground = Brushes.Black;
            Singleton.Instance.main_window.sql_status_text.Text = status ? "Połączono" : "Niepołączono";
            Singleton.Instance.main_window.sql_status_text.Background = status ? Brushes.Green : Brushes.Red;
        }

        public void Load_Users()
        {
            try
            {
                Singleton.Instance.Clear_Users();
                Singleton.Instance.users_window.Window_Clear();

                cmd.CommandText = "select * from users_table";
                cmd.Connection = cnn;
                cmd.CommandType = System.Data.CommandType.Text;

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string user_id = reader["id użytkownika"].ToString();
                    string username = reader["nazwa użytkownika"].ToString();
                    string phone_number = reader["numer telefonu"].ToString();
                    string email = reader["adres email"].ToString();

                    Singleton.Instance.users_window.Add_Line(user_id, username, phone_number, email);
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

            try
            {
                reader.Close();
            }
            catch(Exception ex)
            { }
        }

        public void Load_Messages()
        {
            try
            {
                Singleton.Instance.Clear_Messages();
                Singleton.Instance.messages_window.Window_Clear();

                cmd.CommandText = "select * from messages_table";
                cmd.Connection = cnn;
                cmd.CommandType = System.Data.CommandType.Text;

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string msg_id = reader["id wiadomości"].ToString();
                    string description = reader["opis"].ToString();
                    string sms_text = reader["treść sms"].ToString();
                    string email_text = reader["treść email"].ToString();

                    Singleton.Instance.messages_window.Add_Line(msg_id, description);
                    Singleton.Instance.messages[msg_id] = new Message(sms_text, email_text);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

            try
            {
                reader.Close();
            }
            catch (Exception ex)
            { }
        }


    }
}
