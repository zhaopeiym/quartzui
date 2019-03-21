using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Host.Entity
{
    public class MailEntity
    {
        /// <summary>
        /// 发件邮箱
        /// </summary>
        public string MailFrom { get; set; }
        /// <summary>
        /// 邮箱密码
        /// </summary>
        public string MailPwd { get; set; }
        /// <summary>
        /// 发件服务器
        /// </summary>
        public string MailHost { get; set; }
        /// <summary>
        /// 收件邮箱
        /// </summary>
        public string MailTo { get; set; }
    }
}
