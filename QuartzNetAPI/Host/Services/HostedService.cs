using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Host.Services
{
    public class HostedService : IHostedService
    {
        private SchedulerCenter schedulerCenter;
        public HostedService(SchedulerCenter schedulerCenter)
        {
            this.schedulerCenter = schedulerCenter;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {           
            //开启调度器
            await schedulerCenter.StartScheduleAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
