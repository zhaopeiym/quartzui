using System;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace Host
{
    public class HttpJobFactory:IJobFactory
    {

        private readonly IServiceProvider serviceProvider;

        public HttpJobFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return serviceProvider.GetService<HttpJob>();
        }

        public void ReturnJob(IJob job)
        {
            //因为是单例，所以没用
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}