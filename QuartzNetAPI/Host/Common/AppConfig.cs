using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Talk.Extensions;

namespace Host.Common
{
    public static class AppConfig
    {
        public static string DbProviderName => ConfigurationManager.GetTryConfig("Quartz:dbProviderName");
        public static string ConnectionString => ConfigurationManager.GetTryConfig("Quartz:connectionString");
    }
}
