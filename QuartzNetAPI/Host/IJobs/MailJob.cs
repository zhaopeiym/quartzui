using Host.Common;
using Host.Controllers;
using Quartz;
using System.Threading.Tasks;

namespace Host.IJobs
{
    public class MailJob : BaseJob
    {
        public override async Task NextExecute(IJobExecutionContext context)
        {
            var mailTitle = context.JobDetail.JobDataMap.GetString(Constant.MailTitle);
            var mailContent = context.JobDetail.JobDataMap.GetString(Constant.MailContent);
            var mailTo = context.JobDetail.JobDataMap.GetString(Constant.MailTo);

            //TODO 此处待优化
            var info = await new SetingController().GetMailInfo();
            info.MailTo = mailTo;
            await new SetingController().SendMail(new Model.SendMailModel()
            {
                Title = mailTitle,
                Content = mailContent,
                MailInfo = info
            });
        }
    }
}
