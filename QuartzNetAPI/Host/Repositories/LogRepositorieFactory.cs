using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Host.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl.AdoJobStore.Common;
using Quartz.Util;

namespace Host.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public class LogRepositorieFactory
    {
        private readonly QuartzOptions quartzOptions;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public LogRepositorieFactory(IOptions<QuartzOptions> options)
        {
            quartzOptions = options?.Value ?? new QuartzOptions() {
                { "quartz.jobStore.dataSource", "default" },
                { "quartz.jobStore.tablePrefix", "QRTZ_" }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <returns></returns>
        public ILogRepositorie CreateLogRepositorie(string schedulerName)
        {
            string dataSourceName = quartzOptions.TryGetAndReturn("quartz.jobStore.dataSource") ?? "default";
            string tablePrefix = quartzOptions.TryGetAndReturn("quartz.jobStore.tablePrefix") ?? "QRTZ_";
            string provider = quartzOptions.TryGetAndReturn("quartz.dataSource.default.provider") ?? "";
            if (provider.IndexOf("Oracle", StringComparison.CurrentCultureIgnoreCase) > -1)
            {
                return new LogRepositorieOracle(dataSourceName, tablePrefix, schedulerName);
            }
            return new LogRepositorieDefault(dataSourceName, tablePrefix, schedulerName);
        }

        //    public static ILogRepositorie CreateLogRepositorie(string driverDelegateType, IDbProvider dbProvider)
        //    {

        //        if (driverDelegateType == typeof(SQLiteDelegate).AssemblyQualifiedName)
        //        {
        //            return new LogRepositorieSQLite();
        //        }
        //        else if (driverDelegateType == typeof(OracleDelegate).AssemblyQualifiedName)
        //        {
        //            return new LogRepositorieOracle(dbProvider);
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
    }
}
