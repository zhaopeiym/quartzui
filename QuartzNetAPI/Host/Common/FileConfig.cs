using Host.Entity;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Host.Common
{
    public static class FileConfig
    {
        private static string filePath = "File/Mail.txt";

        private static string mqttFilePath = "File/mqtt.json";
        private static string rabbitFilePath = "File/rabbitmq.json";

        private static MailEntity mailData = null;
        public static async Task<MailEntity> GetMailInfoAsync()
        {
            if (mailData == null)
            {
                if (!System.IO.File.Exists(filePath)) return new MailEntity();
                var mail = await System.IO.File.ReadAllTextAsync(filePath);
                mailData = JsonConvert.DeserializeObject<MailEntity>(mail);
            }
            //深度复制，调用方修改。
            return JsonConvert.DeserializeObject<MailEntity>(JsonConvert.SerializeObject(mailData));
        }

        public static async Task<bool> SaveMailInfoAsync(MailEntity mailEntity)
        {
            mailData = mailEntity;
            await System.IO.File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(mailEntity));
            return true;
        }

        /// <summary>
        /// 保存Mqtt 配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<bool> SaveMqttSetAsync(MqttOptionsEntity input)
        {
            await System.IO.File.WriteAllTextAsync(mqttFilePath, JsonConvert.SerializeObject(input));
            return true;
        }

        /// <summary>
        /// 获取Mqtt 配置
        /// </summary>
        /// <returns></returns>
        public static async Task<MqttOptionsEntity> GetMqttSetAsync()
        {
            if (!System.IO.File.Exists(mqttFilePath)) return new MqttOptionsEntity();

            var entity = await System.IO.File.ReadAllTextAsync(mqttFilePath);
            return JsonConvert.DeserializeObject<MqttOptionsEntity>(entity);
        }

        /// <summary>
        /// 保存Rabbit 配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<bool> SaveRabbitSetAsync(RabbitOptionsEntity input)
        {
            await System.IO.File.WriteAllTextAsync(rabbitFilePath, JsonConvert.SerializeObject(input));
            return true;
        }

        /// <summary>
        /// 获取Rabbit 配置
        /// </summary>
        /// <returns></returns>
        public static async Task<RabbitOptionsEntity> GetRabbitSetAsync()
        {
            if (!System.IO.File.Exists(rabbitFilePath)) return new RabbitOptionsEntity();

            var entity = await System.IO.File.ReadAllTextAsync(rabbitFilePath);
            return JsonConvert.DeserializeObject<RabbitOptionsEntity>(entity);
        }
    }
}
