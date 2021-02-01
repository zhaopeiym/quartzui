using Host.Common;
using Host.IJobs.Model;
using Host.Managers;
using Quartz;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

namespace Host.IJobs
{
    public class RabbitJob : JobBase<LogRabbitModel>, IJob
    {
        private IConnection connection;
        public RabbitJob() : base(new LogRabbitModel())
        {
            connection = RabbitMQManager.Instance.Connection;
        }

        public override async Task NextExecute(IJobExecutionContext context)
        {
            var queue = context.JobDetail.JobDataMap.GetString(Constant.RabbitQueue);
            var body = context.JobDetail.JobDataMap.GetString(Constant.RabbitBody);
            LogInfo.RabbitQueue = queue;
            LogInfo.RabbitBody = body;

            var rabbitSet = await FileConfig.GetRabbitSetAsync();
            if (string.IsNullOrWhiteSpace(rabbitSet.RabbitHost) || string.IsNullOrWhiteSpace(rabbitSet.RabbitUserName))
                LogInfo.ErrorMsg = $"<span class='error'>请先在 [/seting] 页面配置RabbitMQ设置。</span>";
            else if (!connection?.IsOpen ?? true)
                LogInfo.ErrorMsg = $"<span class='error'>RabbitMQ服务连接失败。</span>";
            else
            {
                //创建通道
                using (var channel = connection.CreateModel())
                {
                    //声明一个队列
                    channel.QueueDeclare(queue, false, false, false, null);
                    //发布消息
                    channel.BasicPublish("", queue, null, Encoding.UTF8.GetBytes(body));
                    channel.Close();
                }

                LogInfo.Result = "发送成功！";
            }
        }
    }
}
