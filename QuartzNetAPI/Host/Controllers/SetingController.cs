using Host.Attributes;
using Host.Common;
using Host.Entity;
using Host.Managers;
using Host.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Talk.Extensions;

namespace Host.Controllers
{
    /// <summary>
    /// 设置
    /// </summary>
    [Route("api/[controller]/[Action]")]
    [EnableCors("AllowSameDomain")] //允许跨域 
    public class SetingController : Controller
    {
        private static string refreshIntervalPath = "File/RefreshInterval.json";
        private static string loginPasswordPath = "File/LoginPassword.json";

        private static UpdateLoginInfoEntity LoginInfo = null;
        /// <summary>
        /// 保存Mail信息
        /// </summary>
        /// <param name="mailEntity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> SaveMailInfo([FromBody] MailEntity mailEntity)
        {
            return await FileConfig.SaveMailInfoAsync(mailEntity);
        }

        /// <summary>
        /// 保存Mqtt的配置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> SaveMqttSet([FromBody] MqttOptionsEntity input)
        {
            await FileConfig.SaveMqttSetAsync(input);
            await MqttManager.Instance.RestartAsync();
            return true;
        }

        /// <summary>
        /// 获取Mqtt的配置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<MqttOptionsEntity> GetMqttSet()
        {
            return await FileConfig.GetMqttSetAsync();
        }

        /// <summary>
        /// 保存Rabbit 配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> SaveRabbitSet([FromBody] RabbitOptionsEntity input)
        {
            await FileConfig.SaveRabbitSetAsync(input);
            return true;
        }

        /// <summary>
        /// 重启Rabbit
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> RestartRabbit()
        {
            return await RabbitMQManager.Instance.RestartAsync();
        }

        /// <summary>
        /// 获取Rabbit的配置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<RabbitOptionsEntity> GetRabbitSet()
        {
            return await FileConfig.GetRabbitSetAsync();
        }

        /// <summary>
        /// 保存刷新间隔
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> SaveRefreshInterval([FromBody] RefreshIntervalEntity entity)
        {
            await System.IO.File.WriteAllTextAsync(refreshIntervalPath, JsonConvert.SerializeObject(entity));
            return true;
        }

        /// <summary>
        /// 保存登录密码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> SaveLoginInfo([FromBody] UpdateLoginInfoEntity entity)
        {
            LoginInfo = await GetLoginAsync();
            if (LoginInfo.NewPassword == entity.OldPassword)
            {
                await System.IO.File.WriteAllTextAsync(loginPasswordPath, JsonConvert.SerializeObject(entity));
                LoginInfo = entity;
                return true;
            }
            return false;
        }

        private static DateTime ErrLoginTime = DateTime.MinValue;
        private static int LoginNumber = 0;
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [NoLogin]
        public async Task<LoginInfoOutput> VerifyLoginInfo([FromBody] LoginInfoEntity input)
        {
            var output = new LoginInfoOutput();

            //防止暴力破解,2分钟内只允许错误20次。
            if (LoginNumber++ >= 20 || ErrLoginTime.AddMinutes(2) >= DateTime.Now)
            {
                ErrLoginTime = DateTime.Now;
                LoginNumber = 0;
                return output;
            }

            LoginInfo = await GetLoginAsync();
            if (input.Password == LoginInfo.NewPassword.ToBase64())
            {
                output.Token = $"{DateTime.Now}".DES3Encrypt();
                LoginNumber = 0;
            }
            else
            {
                var defaultPassword = ConfigurationManager.GetTryConfig("DefaultPassword");
                if (!string.IsNullOrWhiteSpace(defaultPassword) && input.Password == defaultPassword.ToBase64())
                {
                    output.Token = $"{DateTime.Now}".DES3Encrypt();
                    LoginNumber = 0;
                }
            }
            return output;
        }

        /// <summary>
        /// 获取登录信息
        /// </summary>
        /// <returns></returns>
        private async Task<UpdateLoginInfoEntity> GetLoginAsync()
        {
            if (LoginInfo == null)
            {
                if (!System.IO.File.Exists(loginPasswordPath))
                    await System.IO.File.WriteAllTextAsync(loginPasswordPath, JsonConvert.SerializeObject(new UpdateLoginInfoEntity()));
                LoginInfo = JsonConvert.DeserializeObject<UpdateLoginInfoEntity>(await System.IO.File.ReadAllTextAsync(loginPasswordPath)) ?? new UpdateLoginInfoEntity();
            }
            return LoginInfo;
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
            return await FileConfig.GetMailInfoAsync();
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [NoLogin]
        public async Task<bool> SendMail([FromBody] SendMailModel model)
        {
            return await MailHelper.SendMail(model.Title, model.Content, model.MailInfo);
        }
    }
}
