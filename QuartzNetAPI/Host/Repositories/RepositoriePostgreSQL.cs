using Dapper;
using Host.Common;
using Newtonsoft.Json.Linq;
using Npgsql;
using Quartz.Impl.AdoJobStore.Common;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Host.Repositories
{
    public class RepositoriePostgreSQL : IRepositorie
    {
        private IDbProvider DBProvider { get; }
        public RepositoriePostgreSQL(IDbProvider dbProvider)
        {
            DBProvider = dbProvider;
        }

        public async Task<int> InitTable()
        {
            using (var connection = new NpgsqlConnection(DBProvider.ConnectionString))
            {
                var check_sql = @"SELECT
	                                        COUNT (1)
                                        FROM
	                                        pg_class
                                        WHERE
	                                        relname IN (
		                                        'qrtz_blob_triggers',
		                                        'qrtz_calendars',
		                                        'qrtz_cron_triggers',
		                                        'qrtz_fired_triggers',
		                                        'qrtz_job_details',
		                                        'qrtz_locks',
		                                        'qrtz_paused_trigger_grps',
		                                        'qrtz_scheduler_state',
		                                        'qrtz_simple_triggers',
		                                        'qrtz_simprop_triggers',
		                                        'qrtz_triggers'
	                                        );";
                var count = await connection.QueryFirstOrDefaultAsync<int>(check_sql);
                //初始化 建表
                if (count == 0)
                {
                    string init_sql = await File.ReadAllTextAsync("Tables/tables_postgres.sql");
                    return await connection.ExecuteAsync(init_sql);
                }
            }
            return 0;
        }

        public async Task<bool> RemoveErrLogAsync(string jobGroup, string jobName)
        {
            try
            {
                using (var connection = new NpgsqlConnection(DBProvider.ConnectionString))
                {
                    string sql = $@"SELECT
	                                JOB_DATA
                                FROM
	                                QRTZ_JOB_DETAILS
                                WHERE
	                                JOB_NAME = @jobName
                                AND JOB_GROUP = @jobGroup";

                    var byteArray = await connection.ExecuteScalarAsync<byte[]>(sql, new { jobName, jobGroup });
                    var jsonStr = Encoding.Default.GetString(byteArray);
                    JObject source = JObject.Parse(jsonStr);
                    source.Remove("Exception");//移除异常日志 
                    var modifySql = $@"UPDATE QRTZ_JOB_DETAILS
                                    SET JOB_DATA = @jobData
                                    WHERE
	                                    JOB_NAME = @jobName
                                    AND JOB_GROUP = @jobGroup";
                    await connection.ExecuteAsync(modifySql, new { jobName, jobGroup, jobData = Encoding.Default.GetBytes(source.ToString()) });
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
