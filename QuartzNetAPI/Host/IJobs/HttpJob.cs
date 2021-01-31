using Host.Common;
using Host.IJobs;
using Host.IJobs.Model;
using Host.Model;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Talk.Extensions;

namespace Host
{
    public class HttpJob : JobBase<LogUrlModel>, IJob
    {
        public HttpJob() : base(new LogUrlModel())
        { }

        public override async Task NextExecute(IJobExecutionContext context)
        {
            //获取相关参数
            var requestUrl = context.JobDetail.JobDataMap.GetString(Constant.REQUESTURL)?.Trim();
            requestUrl = requestUrl?.IndexOf("http") == 0 ? requestUrl : "http://" + requestUrl;
            var requestParameters = context.JobDetail.JobDataMap.GetString(Constant.REQUESTPARAMETERS);
            var headersString = context.JobDetail.JobDataMap.GetString(Constant.HEADERS);
            var headers = headersString != null ? JsonConvert.DeserializeObject<Dictionary<string, string>>(headersString?.Trim()) : null;
            var requestType = (RequestTypeEnum)int.Parse(context.JobDetail.JobDataMap.GetString(Constant.REQUESTTYPE));


            LogInfo.Url = requestUrl;
            LogInfo.RequestType = requestType.ToString();
            LogInfo.Parameters = requestParameters;

            HttpResponseMessage response = new HttpResponseMessage();
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
            LogInfo.Result = $"<span class='result'>{result.MaxLeft(1000)}</span>";
            if (!response.IsSuccessStatusCode)
            {
                LogInfo.ErrorMsg = $"<span class='error'>{result.MaxLeft(3000)}</span>";
                await ErrorAsync(LogInfo.JobName, new Exception(result.MaxLeft(3000)), JsonConvert.SerializeObject(LogInfo), MailLevel);
                context.JobDetail.JobDataMap[Constant.EXCEPTION] = $"<div class='err-time'>{LogInfo.BeginTime}</div>{JsonConvert.SerializeObject(LogInfo)}";
            }
            else
            {
                try
                {
                    //这里需要和请求方约定好返回结果约定为HttpResultModel模型
                    var httpResult = JsonConvert.DeserializeObject<HttpResultModel>(HttpUtility.HtmlDecode(result));
                    if (!httpResult.IsSuccess)
                    {
                        LogInfo.ErrorMsg = $"<span class='error'>{httpResult.ErrorMsg}</span>";
                        await ErrorAsync(LogInfo.JobName, new Exception(httpResult.ErrorMsg), JsonConvert.SerializeObject(LogInfo), MailLevel);
                        context.JobDetail.JobDataMap[Constant.EXCEPTION] = $"<div class='err-time'>{LogInfo.BeginTime}</div>{JsonConvert.SerializeObject(LogInfo)}";
                    }
                    else
                        await InformationAsync(LogInfo.JobName, JsonConvert.SerializeObject(LogInfo), MailLevel);
                }
                catch (Exception)
                {
                    await InformationAsync(LogInfo.JobName, JsonConvert.SerializeObject(LogInfo), MailLevel);
                }
            }
        }
    }
}
