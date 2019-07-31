using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.Mail;

namespace SMS_EMAIL_PLC
{
    class Email_Manager
    {
        SmtpClient smtp;
        public Email_Manager()
        {
            smtp = new SmtpClient("mail.pro-control.pl");//995, 465
        }

        public bool Send(string adress, string subject, string message)
        {
            MailMessage mail = new MailMessage();

            smtp.Credentials = new System.Net.NetworkCredential("temail@pro-control.pl", "pro1$0control");
            smtp.EnableSsl = true;

            mail.From = new MailAddress("temail@pro-control.pl");
            
            mail.To.Add(adress);
            mail.Subject = subject;
            mail.Body = message;

            try
            {
                smtp.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n");
                return false;
            }
        }
    }
}
