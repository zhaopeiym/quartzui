using System;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.IO;
using Dapper;
using Host;
using Host.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Polly;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl.AdoJobStore.Common;
using Quartz.Simpl;
using Quartz.Util;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class QuartzSchedulerServiceCollectionExtensions
    {
        /// <summary>
        /// 使用Quartz
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddQuartzScheduler(this IServiceCollection services)
        {
            //httpfactory
            services.AddHttpClient("polly",
                    (client => { client.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest"); }))
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
                {
                    //从试3此
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(5)
                }));

            services.AddSingleton<HttpJob>();

            //注册scheduler
            services.AddSingleton((ServiceProvider) =>
            {
                var setting = ServiceProvider.GetService<IOptions<QuartzOptions>>().Value;

                var nv = new NameValueCollection();
                foreach (var kv in setting)
                {
                    nv.Add(kv.Key, kv.Value);
                }

                var factory = new StdSchedulerFactory(nv);
                var scheduler = factory.GetScheduler().GetAwaiter().GetResult();
                scheduler.JobFactory = new HttpJobFactory(ServiceProvider);
                // LogProvider.SetCurrentLogProvider(new SerilogLogProvider());
                var provider = setting.TryGetAndReturn("quartz.dataSource.default.provider") ?? "";
                string dataSourceName = setting.TryGetAndReturn("quartz.jobStore.dataSource") ?? "default";

                //是否是sqlite
                if (provider.IndexOf("sqlite", StringComparison.CurrentCultureIgnoreCase) > -1)
                {
                    using (var conn = DBConnectionManager.Instance.GetConnection(dataSourceName))
                    {
                        var fileInfo=new FileInfo(conn.DataSource);
                        if (!fileInfo.Directory.Exists)
                        {
                            fileInfo.Directory.Create();
                        }

                        if (!fileInfo.Exists || fileInfo.Length==0)
                        {
                            //自动生成表
                            string sql = File.ReadAllTextAsync("Tables/tables_sqlite.sql").Result;
                            conn.ExecuteAsync(sql).GetAwaiter().GetResult();
                        }
                    }
                }
                return scheduler;
            });

            //托管服务
            services.AddHostedService<QuartzSchedulerService>();

            return services;
        }
    }
}