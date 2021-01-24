using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Host.Entity
{
    public class UpdateLoginInfoEntity
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
