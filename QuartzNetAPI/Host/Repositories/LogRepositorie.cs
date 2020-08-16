using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using Quartz.Impl.AdoJobStore.Common;

namespace Host.Repositories
{
    public class LogRepositorieSQLite : ILogRepositorie
    {
        public async Task<bool> RemoveErrLogAsync(string jobGroup, string jobName)
        {
            try
            {
                using (var connection = new SqliteConnection("Data Source=File/sqliteScheduler.db"))
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

    public class LogRepositorieOracle : ILogRepositorie
    {
        private IDbProvider DBProvider { get; }

        public LogRepositorieOracle(IDbProvider dbProvider)
        {
            DBProvider = dbProvider;
        }

        public async Task<bool> RemoveErrLogAsync(string jobGroup, string jobName)
        {
            try
            {
                using (var connection = new OracleConnection(DBProvider.ConnectionString))
                {
                    string sql = $@"SELECT
	                                JOB_DATA
                                FROM
	                                QRTZ_JOB_DETAILS
                                WHERE
	                                JOB_NAME = :jobName
                                AND JOB_GROUP = :jobGroup";

                    var byteArray = await connection.ExecuteScalarAsync<byte[]>(sql, new { jobName, jobGroup });
                    var jsonStr = Encoding.UTF8.GetString(byteArray);
                    JObject source = JObject.Parse(jsonStr);
                    source.Remove("Exception");//移除异常日志 
                    var modifySql = $@"UPDATE QRTZ_JOB_DETAILS
                                    SET JOB_DATA = :jobData
                                    WHERE
	                                    JOB_NAME = :jobName
                                    AND JOB_GROUP = :jobGroup";
                    await connection.ExecuteAsync(modifySql, new { jobName, jobGroup, jobData = Encoding.UTF8.GetBytes(source.ToString()) });
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
