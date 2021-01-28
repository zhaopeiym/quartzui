namespace Host.IJobs.Model
{
    public class LogMailModel : LogModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// 收件邮箱
        /// </summary>
        public string MailTo { get; set; }
    }
}
