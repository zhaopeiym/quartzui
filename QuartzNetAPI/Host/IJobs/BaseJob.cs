using Host.Common;
using Host.Controllers;
using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Host.IJobs
{
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public abstract class BaseJob : IJob
    {
        public readonly int maxLogCount = 20;//最多保存日志数量        

        public async Task Execute(IJobExecutionContext context)
        {
            //如果结束时间超过当前时间，则暂停当前任务。
            var endTime = context.JobDetail.JobDataMap.GetString("EndAt");
            if (!string.IsNullOrWhiteSpace(endTime) && DateTime.Parse(endTime) <= DateTime.Now)
            {
                await context.Scheduler.PauseJob(new JobKey(context.JobDetail.Key.Name, context.JobDetail.Key.Group));
                return;
            }

            //记录执行次数
            var runNumber = context.JobDetail.JobDataMap.GetLong(Constant.RUNNUMBER);
            context.JobDetail.JobDataMap[Constant.RUNNUMBER] = ++runNumber;

            try
            {
                await NextExecute(context);
            }
            catch (Exception ex)
            {

            }
            finally { }
        }

        public abstract Task NextExecute(IJobExecutionContext context);

        public async Task WarningAsync(string title, string msg, MailMessageEnum mailMessage)
        {
            Log.Logger.Warning(msg);
            if (mailMessage == MailMessageEnum.All)
            {
                await new SetingController().SendMail(new Model.SendMailModel()
                {
                    Title = $"任务调度-{title}【警告】消息",
                    Content = msg
                });
            }
        }

        public async Task InformationAsync(string title, string msg, MailMessageEnum mailMessage)
        {
            Log.Logger.Information(msg);
            if (mailMessage == MailMessageEnum.All)
            {
                await new SetingController().SendMail(new Model.SendMailModel()
                {
                    Title = $"任务调度-{title}消息",
                    Content = msg
                });
            }
        }

        public async Task ErrorAsync(string title, Exception ex, string msg, MailMessageEnum mailMessage)
        {
            Log.Logger.Error(ex, msg);
            if (mailMessage == MailMessageEnum.Err || mailMessage == MailMessageEnum.All)
            {
                await new SetingController().SendMail(new Model.SendMailModel()
                {
                    Title = $"任务调度-{title}【异常】消息",
                    Content = msg
                });
            }
        }
    }
}
