using Newtonsoft.Json;
using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Host
{
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class HttpJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var requestUrl = context.JobDetail.JobDataMap.GetString("RequestUrl");
            var requestParameters = context.JobDetail.JobDataMap.GetString("RequestParameters");
            var requestType = (RequestTypeEnum)int.Parse(context.JobDetail.JobDataMap.GetString("RequestType"));

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart(); //  开始监视代码运行时间
            var result = string.Empty;

            Log.Logger.Information($@"Begin - Code:{GetHashCode()} Type:{requestType} Url:{requestUrl} Parameters:{requestParameters} JobName:{context.JobDetail.Key.Group}.{context.JobDetail.Key.Name}");

            try
            {
                switch (requestType)
                {
                    case RequestTypeEnum.Get:
                        result = await HttpHelper.Instance.GetAsync(requestUrl);
                        break;
                    case RequestTypeEnum.Post:
                        result = await HttpHelper.Instance.PostAsync(requestUrl, requestParameters);
                        break;
                    case RequestTypeEnum.Put:
                        result = await HttpHelper.Instance.PutAsync(requestUrl, requestParameters);
                        break;
                    case RequestTypeEnum.Delete:
                        result = await HttpHelper.Instance.DeleteAsync(requestUrl);
                        break;
                }
            }
            catch (Exception ex)
            {
                context.JobDetail.JobDataMap["Exception"] = $"Time:{DateTime.Now} Url:{requestUrl} Parameters:{requestParameters} Err:{ex.Message}";
                stopwatch.Stop(); //  停止监视            
                double seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数
                Log.Logger.Error(ex, $@"End   - Code:{GetHashCode()} Type:{requestType} 耗时:{seconds}秒  Url:{requestUrl} Parameters:{requestParameters} JobName:{context.JobDetail.Key.Group}.{context.JobDetail.Key.Name}");
            }
            finally
            {
                stopwatch.Stop(); //  停止监视            
                double seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数
                Log.Logger.Information($@"End   - Code:{GetHashCode()} Type:{requestType} 耗时:{seconds}秒  Url:{requestUrl} Parameters:{requestParameters} JobName:{context.JobDetail.Key.Group}.{context.JobDetail.Key.Name}");
                if (seconds >= 60)//如果请求操作一分钟，记录警告日志
                {
                    Log.Logger.Warning($@"End   - Code:{GetHashCode()} Type:{requestType} 耗时:{seconds}秒  Url:{requestUrl} Parameters:{requestParameters} JobName:{context.JobDetail.Key.Group}.{context.JobDetail.Key.Name}");
                }
            }
            //return Task.CompletedTask;
        }
    }
}
