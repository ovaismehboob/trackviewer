using System;   
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace WCFTrackServiceWebRole
{
    public class EmailManager
    {
        private static EmailManager instance = null;

        public static EmailManager GetInstance()
        {
            instance = (instance == null) ? new EmailManager() : instance;
            return instance;
        }

        private EmailManager() { }


        public String DevelopMessage(string name, string activationCode)
        {
            string message = "<html><body><table style='width:100%;'><tr><td style='font-family: Arial, Helvetica, sans-serif'>Hi "+name+",</td>";
            message +="</tr><tr><td style='font-family: Arial, Helvetica, sans-serif'>&nbsp;</td></tr><tr><td style='font-family: Arial, Helvetica, sans-serif'>";
            message +="Thank you for registering TrackViewer. Your activation code to TrackViewer service is "+activationCode+", please enter this code to complete your registration.</td></tr>";
            message +="<tr><td style='font-family: Arial, Helvetica, sans-serif'>&nbsp;</td></tr><tr><td style='font-family: Arial, Helvetica, sans-serif'>Thanks,</td></tr>";
            message += "<tr><td style='font-family: Arial, Helvetica, sans-serif'>TrackViewer Team</td></tr></table></body></html>";
            return message;
        }


        public String DevelopCompletionEmailMessage(string name)
        {
            string message = "<html><body><table style='width:100%;'><tr><td style='font-family: Arial, Helvetica, sans-serif'>Hi " + name + ",</td>";
            message += "</tr><tr><td style='font-family: Arial, Helvetica, sans-serif'>&nbsp;</td></tr><tr><td style='font-family: Arial, Helvetica, sans-serif'>";
            message += "Thank you for completing your registration with TrackViewer. Now you can track users globally! </td></tr>";
            message += "<tr><td style='font-family: Arial, Helvetica, sans-serif'>&nbsp;</td></tr><tr><td style='font-family: Arial, Helvetica, sans-serif'>Thanks,</td></tr>";
            message += "<tr><td style='font-family: Arial, Helvetica, sans-serif'>TrackViewer Team</td></tr></table></body></html>";
            return message;
        }

        public void SendEmail(string name, string email, string activationCode)
        {

            string smtphost = System.Configuration.ConfigurationSettings.AppSettings.Get("SMTPHost");
            string subject = System.Configuration.ConfigurationSettings.AppSettings.Get("Subject");
            string userName = System.Configuration.ConfigurationSettings.AppSettings.Get("UserName");
            string password = System.Configuration.ConfigurationSettings.AppSettings.Get("Password");
            int port = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings.Get("Port"));
            string to = email;
            string from = System.Configuration.ConfigurationSettings.AppSettings.Get("From");

            Task task = new Task(() =>
            {
                MailMessage message = new MailMessage();
                message.To.Add(to);
                message.From = new MailAddress(from);
                message.Body = DevelopMessage(name, activationCode);
                message.Subject = subject;
                message.IsBodyHtml = true;
                System.Net.Mail.SmtpClient smpt = new System.Net.Mail.SmtpClient();
                smpt.Host = smtphost;
                smpt.Port = port;
                smpt.EnableSsl = true;
                smpt.Credentials = new System.Net.NetworkCredential(userName, password);

                smpt.Send(message);

            });
            task.Start();
        }

        public void SendCompletionEmail(string name, string email)
        {

            string smtphost = System.Configuration.ConfigurationSettings.AppSettings.Get("SMTPHost");
            string userName = System.Configuration.ConfigurationSettings.AppSettings.Get("UserName");
            string password = System.Configuration.ConfigurationSettings.AppSettings.Get("Password");
            int port = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings.Get("Port"));
            string to = email;
            string from = System.Configuration.ConfigurationSettings.AppSettings.Get("From");

            Task task = new Task(() =>
            {
                MailMessage message = new MailMessage();
                message.To.Add(to);
                message.From = new MailAddress(from);
                message.Body = DevelopCompletionEmailMessage(name);
                message.Subject = "Thankyou for Registering";
                message.IsBodyHtml = true;
                System.Net.Mail.SmtpClient smpt = new System.Net.Mail.SmtpClient();
                smpt.Host = smtphost;
                smpt.Port = port;
                smpt.EnableSsl = true;
                smpt.Credentials = new System.Net.NetworkCredential(userName, password);

                smpt.Send(message);

            });
            task.Start();
        }
    }
}