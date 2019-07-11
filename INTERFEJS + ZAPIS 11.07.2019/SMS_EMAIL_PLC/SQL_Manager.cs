using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_EMAIL_PLC
{
    class SQL_Manager
    {
        public SqlConnection cnn;
        public SqlCommand cmd = new SqlCommand();
        public SqlDataReader reader;
        private bool status = false;


        public void Login(string username, string password, string serveradres, string database)
        {
            System.Windows.MessageBox.Show(username + password + serveradres + database);
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
            Singleton.Instance.main_window.sql_status_text.Text = status ? "Połączono" : "Niepołączono";
        }
    }
}
