using System;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using Quartz.Impl.AdoJobStore.Common;
using Quartz.Util;

namespace Host.Repositories
{
    /// <summary>
    /// 默认实现，参数化sql支持@param的都可以使用
    /// </summary>
    public class LogRepositorieDefault : ILogRepositorie
    {
        private readonly string dataSourceName;
        private readonly string tablePrefix;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSourceName"></param>
        /// <param name="tablePrefix"></param>
        public LogRepositorieDefault(string dataSourceName, string tablePrefix)
        {
            this.dataSourceName = dataSourceName;
            this.tablePrefix = tablePrefix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobGroup"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public async Task<bool> RemoveErrLogAsync(string jobGroup, string jobName)
        {
            using (var connection = DBConnectionManager.Instance.GetConnection(dataSourceName))
            {
                string sql = $@"SELECT
	                                JOB_DATA
                                FROM
	                                {tablePrefix}JOB_DETAILS
                                WHERE
	                                JOB_NAME = @jobName
                                AND JOB_GROUP = @jobGroup";

                var byteArray = await connection.ExecuteScalarAsync<byte[]>(sql, new { jobName, jobGroup });
                var jsonStr = Encoding.Default.GetString(byteArray);
                JObject source = JObject.Parse(jsonStr);
                source.Remove("Exception");//移除异常日志 
                var modifySql = $@"UPDATE {tablePrefix}JOB_DETAILS
                                    SET JOB_DATA = @jobData
                                    WHERE
	                                    JOB_NAME = @jobName
                                    AND JOB_GROUP = @jobGroup";
                await connection.ExecuteAsync(modifySql, new { jobName, jobGroup, jobData = source.ToString() });
                return true;
            }
        }
    }

    /// <summary>
    /// Oracle实现
    /// </summary>
    public class LogRepositorieOracle : ILogRepositorie
    {
        private readonly string dataSourceName;
        private readonly string tablePrefix;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSourceName"></param>
        /// <param name="tablePrefix"></param>
        public LogRepositorieOracle(string dataSourceName, string tablePrefix)
        {
            this.dataSourceName = dataSourceName;
            this.tablePrefix = tablePrefix;
        }

        public async Task<bool> RemoveErrLogAsync(string jobGroup, string jobName)
        {
            try
            {
                using (var connection = DBConnectionManager.Instance.GetConnection(dataSourceName))
                {
                    string sql = $@"SELECT
	                                JOB_DATA
                                FROM
	                                {tablePrefix}JOB_DETAILS
                                WHERE
	                                JOB_NAME = :jobName
                                AND JOB_GROUP = :jobGroup";

                    var byteArray = await connection.ExecuteScalarAsync<byte[]>(sql, new { jobName, jobGroup });
                    var jsonStr = Encoding.UTF8.GetString(byteArray);
                    JObject source = JObject.Parse(jsonStr);
                    source.Remove("Exception");//移除异常日志 
                    var modifySql = $@"UPDATE {tablePrefix}JOB_DETAILS
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
