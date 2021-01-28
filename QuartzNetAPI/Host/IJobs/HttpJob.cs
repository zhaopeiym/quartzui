using Host.Common;
using Host.Controllers;
using Host.IJobs;
using Host.Model;
using Newtonsoft.Json;
using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Talk.Extensions;

namespace Host
{
    public class HttpJob : BaseJob
    {
        public readonly int warnTime = 20;//接口请求超过多少秒记录警告日志 

        public override async Task NextExecute(IJobExecutionContext context)
        {
            //获取相关参数
            var requestUrl = context.JobDetail.JobDataMap.GetString(Constant.REQUESTURL)?.Trim();
            requestUrl = requestUrl?.IndexOf("http") == 0 ? requestUrl : "http://" + requestUrl;
            var requestParameters = context.JobDetail.JobDataMap.GetString(Constant.REQUESTPARAMETERS);
            var headersString = context.JobDetail.JobDataMap.GetString(Constant.HEADERS);
            var mailMessage = (MailMessageEnum)int.Parse(context.JobDetail.JobDataMap.GetString(Constant.MAILMESSAGE) ?? "0");
            var headers = headersString != null ? JsonConvert.DeserializeObject<Dictionary<string, string>>(headersString?.Trim()) : null;
            var requestType = (RequestTypeEnum)int.Parse(context.JobDetail.JobDataMap.GetString(Constant.REQUESTTYPE));

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart(); //  开始监视代码运行时间
            HttpResponseMessage response = new HttpResponseMessage();

            var loginfo = new LogInfoModel();
            loginfo.Url = requestUrl;
            loginfo.BeginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            loginfo.RequestType = requestType.ToString();
            loginfo.Parameters = requestParameters;
            loginfo.JobName = $"{context.JobDetail.Key.Group}.{context.JobDetail.Key.Name}";

            var logs = context.JobDetail.JobDataMap[Constant.LOGLIST] as List<string> ?? new List<string>();
            if (logs.Count >= maxLogCount)
                logs.RemoveRange(0, logs.Count - maxLogCount);

            try
            {
                var http = HttpHelper.Instance;
                switch (requestType)
                {
                    case RequestTypeEnum.Get:
                        response = await http.GetAsync(requestUrl, headers);
                        break;
                    case RequestTypeEnum.Post:
                        response = await http.PostAsync(requestUrl, requestParameters, headers);
                        break;
                    case RequestTypeEnum.Put:
                        response = await http.PutAsync(requestUrl, requestParameters, headers);
                        break;
                    case RequestTypeEnum.Delete:
                        response = await http.DeleteAsync(requestUrl, headers);
                        break;
                }
                var result = HttpUtility.HtmlEncode(await response.Content.ReadAsStringAsync());

                stopwatch.Stop(); //  停止监视            
                double seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数                                
                loginfo.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                loginfo.ExecuteTime = seconds + "秒";
                loginfo.Result = $"<span class='result'>{result.MaxLeft(1000)}</span>";
                if (!response.IsSuccessStatusCode)
                {
                    loginfo.ErrorMsg = $"<span class='error'>{result.MaxLeft(3000)}</span>";
                    await ErrorAsync(loginfo.JobName, new Exception(result.MaxLeft(3000)), JsonConvert.SerializeObject(loginfo), mailMessage);
                    context.JobDetail.JobDataMap[Constant.EXCEPTION] = JsonConvert.SerializeObject(loginfo);
                }
                else
                {
                    try
                    {
                        //这里需要和请求方约定好返回结果约定为HttpResultModel模型
                        var httpResult = JsonConvert.DeserializeObject<HttpResultModel>(HttpUtility.HtmlDecode(result));
                        if (!httpResult.IsSuccess)
                        {
                            loginfo.ErrorMsg = $"<span class='error'>{httpResult.ErrorMsg}</span>";
                            await ErrorAsync(loginfo.JobName, new Exception(httpResult.ErrorMsg), JsonConvert.SerializeObject(loginfo), mailMessage);
                            context.JobDetail.JobDataMap[Constant.EXCEPTION] = JsonConvert.SerializeObject(loginfo);
                        }
                        else
                            await InformationAsync(loginfo.JobName, JsonConvert.SerializeObject(loginfo), mailMessage);
                    }
                    catch (Exception)
                    {
                        await InformationAsync(loginfo.JobName, JsonConvert.SerializeObject(loginfo), mailMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop(); //  停止监视            
                double seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数
                loginfo.ErrorMsg = $"<span class='error'>{ex.Message} {ex.StackTrace}</span>";
                context.JobDetail.JobDataMap[Constant.EXCEPTION] = JsonConvert.SerializeObject(loginfo);
                loginfo.ExecuteTime = seconds + "秒";
                await ErrorAsync(loginfo.JobName, ex, JsonConvert.SerializeObject(loginfo), mailMessage);
            }
            finally
            {
                logs.Add($"<p class='msgList'>{JsonConvert.SerializeObject(loginfo)}</p>");
                context.JobDetail.JobDataMap[Constant.LOGLIST] = logs;
                double seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数
                if (seconds >= warnTime)//如果请求超过20秒，记录警告日志    
                {
                    await WarningAsync(loginfo.JobName, "耗时过长 - " + JsonConvert.SerializeObject(loginfo), mailMessage);
                }
            }
        } 
    }
}
