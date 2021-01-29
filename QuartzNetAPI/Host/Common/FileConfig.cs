using Host.Entity;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Host.Common
{
    public static class FileConfig
    {
        private static string filePath = "File/Mail.txt";
        private static MailEntity mailData = null;
        public static async Task<MailEntity> GetMailInfo()
        {
            if (mailData == null)
            {
                if (!System.IO.File.Exists(filePath)) return new MailEntity();
                var mail = await System.IO.File.ReadAllTextAsync(filePath);
                mailData = JsonConvert.DeserializeObject<MailEntity>(mail);
            }
            return mailData;
        }

        public static async Task<bool> SaveMailInfo(MailEntity mailEntity)
        {
            mailData = mailEntity;
            await System.IO.File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(mailEntity));
            return true;
        }
    }
}
