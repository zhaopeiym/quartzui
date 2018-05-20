using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Host
{
    public enum RequestTypeEnum
    {     
        None = 0,
        Get = 1,
        Post = 2,
        Put = 4,
        Delete = 8
    }
}
