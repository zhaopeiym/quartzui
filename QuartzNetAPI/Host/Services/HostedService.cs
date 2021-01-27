using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
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
            //初始化Scheduler
            await schedulerCenter.InitSchedulerAsync();
            //开启调度器
            await schedulerCenter.StartScheduleAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
