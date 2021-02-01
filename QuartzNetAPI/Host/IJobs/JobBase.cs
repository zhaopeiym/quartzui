using Host.Common;
using Host.Controllers;
using Host.IJobs.Model;
using Host.Model;
using Newtonsoft.Json;
using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Host.IJobs
{
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public abstract class JobBase<T> where T : LogModel, new()
    {
        protected readonly int maxLogCount = 20;//最多保存日志数量  
        protected readonly int warnTime = 20;//接口请求超过多少秒记录警告日志 
        protected Stopwatch stopwatch = new Stopwatch();
        protected T LogInfo { get; private set; }
        protected MailMessageEnum MailLevel = MailMessageEnum.None;

        public JobBase(T logInfo)
        {
            LogInfo = logInfo;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            //如果结束时间超过当前时间，则暂停当前任务。
            var endTime = context.JobDetail.JobDataMap.GetString("EndAt");
            if (!string.IsNullOrWhiteSpace(endTime) && DateTime.Parse(endTime) <= DateTime.Now)
            {
                await context.Scheduler.PauseJob(new JobKey(context.JobDetail.Key.Name, context.JobDetail.Key.Group));
                return;
            }

            MailLevel = (MailMessageEnum)int.Parse(context.JobDetail.JobDataMap.GetString(Constant.MAILMESSAGE) ?? "0");
            //记录执行次数
            var runNumber = context.JobDetail.JobDataMap.GetLong(Constant.RUNNUMBER);
            context.JobDetail.JobDataMap[Constant.RUNNUMBER] = ++runNumber;

            var logs = context.JobDetail.JobDataMap[Constant.LOGLIST] as List<string> ?? new List<string>();
            if (logs.Count >= maxLogCount)
                logs.RemoveRange(0, logs.Count - maxLogCount);

            stopwatch.Restart(); //  开始监视代码运行时间
            try
            {
                LogInfo.BeginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                LogInfo.JobName = $"{context.JobDetail.Key.Group}.{context.JobDetail.Key.Name}";

                await NextExecute(context);
            }
            catch (Exception ex)
            {
                LogInfo.ErrorMsg = $"<span class='error'>{ex.Message}</span>";
                context.JobDetail.JobDataMap[Constant.EXCEPTION] = $"<div class='err-time'>{LogInfo.BeginTime}</div>{JsonConvert.SerializeObject(LogInfo)}";
                await ErrorAsync(LogInfo.JobName, ex, JsonConvert.SerializeObject(LogInfo), MailLevel);
            }
            finally
            {
                stopwatch.Stop(); //  停止监视            
                double seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数             
                LogInfo.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                if (seconds >= 1)
                    LogInfo.ExecuteTime = seconds + "秒";
                else
                    LogInfo.ExecuteTime = stopwatch.Elapsed.TotalMilliseconds + "毫秒";

                var classErr = string.IsNullOrWhiteSpace(LogInfo.ErrorMsg) ? "" : "error";
                logs.Add($"<p class='msgList {classErr}'><span class='time'>{LogInfo.BeginTime} 至 {LogInfo.EndTime}  【耗时】{LogInfo.ExecuteTime}</span>{JsonConvert.SerializeObject(LogInfo)}</p>");
                context.JobDetail.JobDataMap[Constant.LOGLIST] = logs;
                if (seconds >= warnTime)//如果请求超过20秒，记录警告日志    
                {
                    await WarningAsync(LogInfo.JobName, "耗时过长 - " + JsonConvert.SerializeObject(LogInfo), MailLevel);
                }
            }
        }

        public abstract Task NextExecute(IJobExecutionContext context);

        public async Task WarningAsync(string title, string msg, MailMessageEnum mailMessage)
        {
            Log.Logger.Warning(msg);
            if (mailMessage == MailMessageEnum.All)
            {
                await new SetingController().SendMail(new SendMailModel()
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
                await new SetingController().SendMail(new SendMailModel()
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
                await new SetingController().SendMail(new SendMailModel()
                {
                    Title = $"任务调度-{title}【异常】消息",
                    Content = msg
                });
            }
        }
    }
}
