using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Core.Utils
{
    public class EmailUtil
    {
        public void SendEmail(string fromAddress, string fromName, string toAddress, string toName, string subject, string body, bool isBodyHtml)
        {
            MailMessage mailMsg = new MailMessage();
            if (string.IsNullOrEmpty(fromName)) mailMsg.From = new MailAddress(fromAddress);
            else mailMsg.From = new MailAddress(fromAddress, fromName);
            if (string.IsNullOrEmpty(toName)) mailMsg.To.Add(new MailAddress(toAddress));
            else mailMsg.To.Add(new MailAddress(toAddress, toName));
            mailMsg.Subject = subject;
            mailMsg.Body = body;
            mailMsg.IsBodyHtml = isBodyHtml;

            SmtpClient client = new SmtpClient();
            client.Send(mailMsg);
        }

        public void SendEmail(SMTP smtp, string fromAddress, string fromName, string toAddress, string toName, string subject, string body, bool isBodyHtml)
        {
            MailMessage mailMsg = new MailMessage();
            if (string.IsNullOrEmpty(fromName)) mailMsg.From = new MailAddress(fromAddress);
            else mailMsg.From = new MailAddress(fromAddress, fromName);
            if (string.IsNullOrEmpty(toName)) mailMsg.To.Add(new MailAddress(toAddress));
            else mailMsg.To.Add(new MailAddress(toAddress, toName));
            mailMsg.Subject = subject;
            mailMsg.Body = body;
            mailMsg.IsBodyHtml = isBodyHtml;

            SmtpClient client = new SmtpClient();
            client.Port = smtp.Port;//指定 smtp 服务器的端口，默认是25，如果采用默认端口，可省去
            client.Host = smtp.Host;//指定 smtp 服务器地址
            client.UseDefaultCredentials = false;//服务器是否需要身份认证
            client.Credentials = new NetworkCredential(smtp.Email, smtp.Password);
            client.EnableSsl = false;//smtp服务器是否启用SSL加密
            client.DeliveryMethod = SmtpDeliveryMethod.Network;//将smtp的出站方式设为 Network
            client.Send(mailMsg);
        }
    }

    public class SMTP
    {
        public int Port { get; set; }
        public string Host { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
