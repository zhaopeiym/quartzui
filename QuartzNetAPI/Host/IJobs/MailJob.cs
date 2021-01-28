using Host.Common;
using Host.IJobs.Model;
using Quartz;
using System.Threading.Tasks;

namespace Host.IJobs
{
    public class MailJob : BaseJob<LogMailModel>, IJob
    {
        public MailJob()
            : base(new LogMailModel())
        { }

        public override async Task NextExecute(IJobExecutionContext context)
        {
            var title = context.JobDetail.JobDataMap.GetString(Constant.MailTitle);
            var content = context.JobDetail.JobDataMap.GetString(Constant.MailContent);
            var mailTo = context.JobDetail.JobDataMap.GetString(Constant.MailTo);

            LogInfo.Title = title;
            LogInfo.Content = content;
            LogInfo.MailTo = mailTo;

            await MailHelper.SendMail(title, content, mailTo);

        }
    }
}
