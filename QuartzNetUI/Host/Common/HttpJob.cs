using Host.Common;
using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Talk.Extensions;

namespace Host
{
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class HttpJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var maxLogCount = 20;//最多保存日志数量
            var warnTime = 20;//接口请求超过多少秒记录警告日志

            //获取相关参数
            var requestUrl = context.JobDetail.JobDataMap.GetString(Constant.REQUESTURL);
            requestUrl = requestUrl?.IndexOf("http") == 0 ? requestUrl : "http://" + requestUrl;
            var requestParameters = context.JobDetail.JobDataMap.GetString(Constant.REQUESTPARAMETERS);
            var requestType = (RequestTypeEnum)int.Parse(context.JobDetail.JobDataMap.GetString(Constant.REQUESTTYPE));

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart(); //  开始监视代码运行时间
            var result = string.Empty;

            var logBeginMsg = $@"Begin - Code:{GetHashCode()} Type:{requestType} Url:{requestUrl} Parameters:{requestParameters} JobName:{context.JobDetail.Key.Group}.{context.JobDetail.Key.Name}";
            Log.Logger.Information(logBeginMsg);

            var logs = context.JobDetail.JobDataMap[Constant.LOGLIST] as List<string> ?? new List<string>();
            if (logs.Count >= maxLogCount)
                logs.RemoveRange(0, logs.Count - maxLogCount);
            logs.Add($"<p>{logBeginMsg} Time:{DateTime.Now.yyyMMddHHssmm2()}</p>");

            try
            {
                var http = HttpHelper.Instance;
                switch (requestType)
                {
                    case RequestTypeEnum.Get:
                        result = await http.GetAsync(requestUrl);
                        break;
                    case RequestTypeEnum.Post:
                        result = await http.PostAsync(requestUrl, requestParameters);
                        break;
                    case RequestTypeEnum.Put:
                        result = await http.PutAsync(requestUrl, requestParameters);
                        break;
                    case RequestTypeEnum.Delete:
                        result = await http.DeleteAsync(requestUrl);
                        break;
                }

                stopwatch.Stop(); //  停止监视            
                double seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数
                var logEndMsg = $@"End   - Code:{GetHashCode()} Type:{requestType} 耗时:{seconds}秒  Url:{requestUrl} Parameters:{requestParameters} JobName:{context.JobDetail.Key.Group}.{context.JobDetail.Key.Name}";
                Log.Logger.Information(logEndMsg);
                if (seconds >= warnTime)//如果请求超过20秒，记录警告日志     
                    logs.Add($"<p>{logEndMsg} Ok <span class='warning'>Time:{DateTime.Now.yyyMMddHHssmm2()}</span></p>");
                else
                    logs.Add($"<p>{logEndMsg} Ok Time:{DateTime.Now.yyyMMddHHssmm2()}</p>");
            }
            catch (Exception ex)
            {
                context.JobDetail.JobDataMap[Constant.EXCEPTION] = $"Time:{DateTime.Now.yyyMMddHHssmm2()} Url:{requestUrl} Parameters:{requestParameters} Err:{ex.Message}";
                stopwatch.Stop(); //  停止监视            
                double seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数
                var logEndMsg = $@"End   - Code:{GetHashCode()} Type:{requestType} 耗时:{seconds}秒  Url:{requestUrl} Parameters:{requestParameters} JobName:{context.JobDetail.Key.Group}.{context.JobDetail.Key.Name}";
                Log.Logger.Error(ex, logEndMsg);
                logs.Add($"<p>{logEndMsg} <span class='error'>Err:{ex.Message}</span> Time:{DateTime.Now.yyyMMddHHssmm2()}</p>");
            }
            finally
            {
                context.JobDetail.JobDataMap[Constant.LOGLIST] = logs;
                double seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数
                if (seconds >= warnTime)//如果请求超过20秒，记录警告日志                
                    Log.Logger.Warning($@"End   - Code:{GetHashCode()} Type:{requestType} 耗时:{seconds}秒  Url:{requestUrl} Parameters:{requestParameters} JobName:{context.JobDetail.Key.Group}.{context.JobDetail.Key.Name}");
            }
        }
    }
}
