using Host.Entity;
using Host.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using NETCore.Encrypt;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Host.Controllers
{
    [Route("api/[controller]/[Action]")]
    [EnableCors("AllowSameDomain")] //允许跨域 
    public class SetingController : Controller
    {
        static string filePath = "File/Mail.txt";
        static string refreshIntervalPath = "File/RefreshInterval.json";

        static MailEntity mailData = null;
        /// <summary>
        /// 保存Mail信息
        /// </summary>
        /// <param name="mailEntity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> SaveMailInfo([FromBody]MailEntity mailEntity)
        {
            mailData = mailEntity;
            //加密邮件信息
            #region 生成密钥及向量
            var aseKey = EncryptProvider.CreateAesKey();
            var key = aseKey.Key;
            var iv = aseKey.IV;
            #endregion

            #region 加密邮件信息并组织Json数据
            var t1 = JsonConvert.SerializeObject(mailEntity);
            var encrypted = EncryptProvider.AESEncrypt(EncryptProvider.AESEncrypt(t1, key), key, iv);
            var t2 = new
            {
                mail = encrypted,
                pwd = key,
                pwd1 = iv,
            };
            #endregion

            await System.IO.File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(t2));
            return true;
        }

        /// <summary>
        /// 保存刷新间隔
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> SaveRefreshInterval([FromBody]RefreshIntervalEntity entity)
        {
            await System.IO.File.WriteAllTextAsync(refreshIntervalPath, JsonConvert.SerializeObject(entity));
            return true;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<RefreshIntervalEntity> GetRefreshInterval()
        {
            return JsonConvert.DeserializeObject<RefreshIntervalEntity>(await System.IO.File.ReadAllTextAsync(refreshIntervalPath));
        }

        /// <summary>
        /// 获取eMail信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<MailEntity> GetMailInfo()
        {
            if (mailData == null)
            {
                #region 从文件中取出对应的值并解密
                var t2 = await System.IO.File.ReadAllTextAsync(filePath);
                JObject o = JObject.Parse(t2);
                var pwd = o["pwd"].ToString();
                var pwd1 = o["pwd1"].ToString();
                var t1 = o["mail"].ToString();
                var decrypted = EncryptProvider.AESDecrypt(EncryptProvider.AESDecrypt(t1, pwd, pwd1), pwd);
                #endregion 从文件中取出对应的值并解密

                //var mail = await System.IO.File.ReadAllTextAsync(filePath);
                mailData = JsonConvert.DeserializeObject<MailEntity>(decrypted);
            }
            return mailData;
        }
        
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="mail"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> SendMail([FromBody]SendMailModel model)
        {
            try
            {
                if (model.MailInfo == null)
                    model.MailInfo = await GetMailInfo();
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(model.MailInfo.MailFrom, model.MailInfo.MailFrom));
                foreach (var mailTo in model.MailInfo.MailTo.Replace("；", ";").Replace("，", ";").Replace(",", ";").Split(';'))
                {
                    message.To.Add(new MailboxAddress(mailTo, mailTo));
                }
                message.Subject = string.Format(model.Title);
                message.Body = new TextPart("html")
                {
                    Text = model.Content
                };
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(model.MailInfo.MailHost, 465, true);
                    client.Authenticate(model.MailInfo.MailFrom, model.MailInfo.MailPwd);
                    client.Send(message);
                    client.Disconnect(true);
                }
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}
