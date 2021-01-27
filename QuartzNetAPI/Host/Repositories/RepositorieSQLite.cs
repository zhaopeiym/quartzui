using Dapper;
using Host.Common;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using Quartz.Impl.AdoJobStore.Common;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Host.Repositories
{
    public class RepositorieSQLite : IRepositorie
    {
        private IDbProvider DBProvider { get; }

        public RepositorieSQLite(IDbProvider dbProvider)
        {
            DBProvider = dbProvider;
        }

        public async Task<int> InitTable()
        {
            if (!Directory.Exists("File")) Directory.CreateDirectory("File");

            using (var connection = new SqliteConnection(DBProvider.ConnectionString))
            {
                var check_sql = @$"SELECT
	                                        count(1)
                                        FROM
	                                        sqlite_master
                                        WHERE
	                                        type = 'table'
                                        AND name IN (
	                                        'QRTZ_JOB_DETAILS',
	                                        'QRTZ_TRIGGERS',
	                                        'QRTZ_SIMPLE_TRIGGERS',
	                                        'QRTZ_SIMPROP_TRIGGERS',
	                                        'QRTZ_CRON_TRIGGERS',
	                                        'QRTZ_BLOB_TRIGGERS',
	                                        'QRTZ_CALENDARS',
	                                        'QRTZ_PAUSED_TRIGGER_GRPS',
	                                        'QRTZ_FIRED_TRIGGERS',
	                                        'QRTZ_SCHEDULER_STATE',
	                                        'QRTZ_LOCKS'
                                        );";
                var count = await connection.QueryFirstOrDefaultAsync<int>(check_sql);
                //初始化 建表
                if (count == 0)
                {
                    string init_sql = await File.ReadAllTextAsync("Tables/tables_sqlite.sql");
                    return await connection.ExecuteAsync(init_sql);
                }
            }
            return 0;
        }
        public async Task<bool> RemoveErrLogAsync(string jobGroup, string jobName)
        {
            try
            {
                using (var connection = new SqliteConnection(DBProvider.ConnectionString))
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
                    await connection.ExecuteAsync(modifySql, new { jobName, jobGroup, jobData = source.ToString() });
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
