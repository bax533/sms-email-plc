using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
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


        public bool Login(string username, string password, string serveradres, string database)
        {
            Thread thread = new Thread(() =>
            {
                Wait_Dialog w = new Wait_Dialog("proszę czekać");
                w.Show();

                w.Closed += (sender2, e2) =>
                w.Dispatcher.InvokeShutdown();

                System.Windows.Threading.Dispatcher.Run();
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            Thread.Sleep(1000);
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
                thread.Abort();
                Singleton.Show_MessageBox("Nie można połączyć się z serwerem SQL");
                status = false;
                return status;
            }
            thread.Abort();
            return status;
        }

        public void Load_Users()
        {
            try
            {
                Singleton.Instance.Clear_Users();
                Singleton.Instance.users_page.Window_Clear();

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

                    Singleton.Instance.users_page.Add_Line(user_id, username, phone_number, email);
                }
            }
            catch(Exception ex)
            {
                Singleton.Show_MessageBox(ex.Message);
            }

            try
            {
                reader.Close();
            }
            catch(Exception ex)
            { }
        }

        public bool GetStatus()
        {
            return status;
        }

        public void Load_Messages()
        {
            if (status)
            {
                try
                {
                    Singleton.Instance.Clear_Messages();
                    Singleton.Instance.messages_page.Window_Clear();

                    cmd.CommandText = "select * from messages_table";
                    cmd.Connection = cnn;
                    cmd.CommandType = System.Data.CommandType.Text;

                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string msg_id = reader["id wiadomości"].ToString();
                        string description = reader["opis"].ToString();
                        bool active = reader["aktywność"].ToString() == "TRUE";

                        Singleton.Instance.messages_page.Add_Line(msg_id, description, active);
                        Singleton.Instance.Add_Message(msg_id,new Message_Add(description, active));
                    }
                }
                catch (Exception ex)
                {
                    Singleton.Show_MessageBox(ex.Message);
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
}
