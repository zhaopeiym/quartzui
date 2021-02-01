using Host.Managers;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Host.Services
{
    public class HostedService : IHostedService
    {
        private SchedulerCenter schedulerCenter;
        private MqttManager mqttManager;
        public HostedService(SchedulerCenter schedulerCenter)
        {
            this.schedulerCenter = schedulerCenter;
            mqttManager = MqttManager.Instance;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //开启调度器
            await schedulerCenter.StartScheduleAsync();

            //启动mqtt
            await mqttManager.RestartAsync();

            //启动Rabbit
            await RabbitMQManager.Instance.RestartAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
