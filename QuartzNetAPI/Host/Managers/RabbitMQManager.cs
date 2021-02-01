using Host.Common;
using RabbitMQ.Client;
using System.Threading.Tasks;

namespace Host.Managers
{
    public class RabbitMQManager
    {
        public static readonly RabbitMQManager Instance;
        static RabbitMQManager()
        {
            Instance = new RabbitMQManager();
        }


        public IConnection Connection { get; private set; }

        /// <summary>
        /// 重启启动
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> RestartAsync()
        {
            Stop();

            var entity = await FileConfig.GetRabbitSetAsync();
            //创建连接工厂
            var factory = new ConnectionFactory
            {
                UserName = entity.RabbitUserName,//用户名
                Password = entity.RabbitPassword,//密码
                HostName = entity.RabbitHost,//rabbitmq ip
                Port = entity.RabbitPort,
            };
            try
            {
                //创建连接
                Connection = factory.CreateConnection();
            }
            catch (System.Exception ex)
            {
                //log
                return false;
            }
            return true;
        }

        /// <summary>
        /// Stop
        /// </summary>
        /// <returns></returns>
        private void Stop()
        {
            if (Connection?.IsOpen ?? false)
            {
                Connection?.Close();
                Connection?.Dispose();
            }
        }
    }
}
