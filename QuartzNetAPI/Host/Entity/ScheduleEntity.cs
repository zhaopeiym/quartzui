using Host.Common;
using Host.Common.Enums;
using System;

namespace Host
{
    public class ScheduleEntity
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; set; }
        /// <summary>
        /// 任务分组
        /// </summary>
        public string JobGroup { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public JobTypeEnum JobType { get; set; } = JobTypeEnum.Url;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTimeOffset BeginTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTimeOffset? EndTime { get; set; }
        /// <summary>
        /// Cron表达式
        /// </summary>
        public string Cron { get; set; }
        /// <summary>
        /// 执行次数（默认无限循环）
        /// </summary>
        public int? RunTimes { get; set; }
        /// <summary>
        /// 执行间隔时间，单位秒（如果有Cron，则IntervalSecond失效）
        /// </summary>
        public int? IntervalSecond { get; set; }
        /// <summary>
        /// 触发器类型
        /// </summary>
        public TriggerTypeEnum TriggerType { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        public MailMessageEnum MailMessage { get; set; }

        #region Url
        /// <summary>
        /// 请求url
        /// </summary>
        public string RequestUrl { get; set; }
        /// <summary>
        /// 请求参数（Post，Put请求用）
        /// </summary>
        public string RequestParameters { get; set; }
        /// <summary>
        /// Headers(可以包含如：Authorization授权认证)
        /// 格式：{"Authorization":"userpassword.."}
        /// </summary>
        public string Headers { get; set; }
        /// <summary>
        /// 请求类型
        /// </summary>
        public RequestTypeEnum RequestType { get; set; } = RequestTypeEnum.Post;
        #endregion

        #region Emial
        public string MailTitle { get; set; }
        public string MailContent { get; set; }
        public string MailTo { get; set; }
        #endregion

        #region MQTT
        /// Topic 主题
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// Payload
        /// </summary>
        public string Payload { get; set; }
        #endregion

        #region Rabbit
        public string RabbitQueue { get; set; }
        public string RabbitBody { get; set; }
        #endregion
    }
}
