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
            smtp = new SmtpClient("smtp.gmail.com");
        }

        public bool Send(List<string> adresses, string subject, string message)
        {
            MailMessage mail = new MailMessage();

            smtp.Port = 587;
            smtp.Credentials = new System.Net.NetworkCredential("alert.notifier.pc@gmail.com", "procontrol");
            smtp.EnableSsl = true;

            mail.From = new MailAddress("alert.notifier.pc@gmail.com");
            foreach (string email_adress in adresses)
                mail.To.Add(email_adress);
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
