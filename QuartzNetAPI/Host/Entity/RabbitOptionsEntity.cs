using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Host.Entity
{
    public class RabbitOptionsEntity
    {
        public string RabbitHost { get; set; }
        public int RabbitPort { get; set; }
        public string RabbitUserName { get; set; }
        public string RabbitPassword { get; set; }
    }
}
