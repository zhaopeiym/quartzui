using Host.Entity;
using MimeKit;
using System;
using System.Threading.Tasks;
using Talk.Extensions;

namespace Host.Common
{
    public static class MailHelper
    {
        public static async Task<bool> SendMail(string title, string content, MailEntity mailInfo = null)
        {
            if (mailInfo == null)
            {
                mailInfo = await FileConfig.GetMailInfo();
                if (mailInfo.MailPwd.IsNullOrWhiteSpace() ||
                    mailInfo.MailFrom.IsNullOrWhiteSpace() ||
                    mailInfo.MailHost.IsNullOrWhiteSpace())
                {
                    throw new Exception("请先在/seting页面配置邮箱设置。");
                }
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(mailInfo.MailFrom, mailInfo.MailFrom));
            foreach (var mailTo in mailInfo.MailTo.Replace("；", ";").Replace("，", ";").Replace(",", ";").Split(';'))
            {
                message.To.Add(new MailboxAddress(mailTo, mailTo));
            }
            message.Subject = string.Format(title);
            message.Body = new TextPart("html")
            {
                Text = content
            };
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect(mailInfo.MailHost, 465, true);
                client.Authenticate(mailInfo.MailFrom, mailInfo.MailPwd);
                client.Send(message);
                client.Disconnect(true);
            }
            return true;
        }

        public static async Task<bool> SendMail(string title, string content, string mailTo)
        {
            var info = await FileConfig.GetMailInfo();
            if (info.MailPwd.IsNullOrWhiteSpace() || info.MailFrom.IsNullOrWhiteSpace() || info.MailHost.IsNullOrWhiteSpace())
                throw new Exception("请先在/seting页面配置邮箱设置。");
            info.MailTo = mailTo;
            return await SendMail(title, content, info);
        }
    }
}
