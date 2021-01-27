using Quartz.Impl.AdoJobStore;
using Quartz.Impl.AdoJobStore.Common;

namespace Host.Repositories
{
    public class LogRepositorieFactory
    {
        public static ILogRepositorie CreateLogRepositorie(string driverDelegateType, IDbProvider dbProvider)
        {

            if (driverDelegateType == typeof(SQLiteDelegate).AssemblyQualifiedName)
            {
                return new LogRepositorieSQLite();
            }
            else if (driverDelegateType == typeof(MySQLDelegate).AssemblyQualifiedName)
            {
                return new LogRepositorieMySql();
            }
            else if (driverDelegateType == typeof(PostgreSQLDelegate).AssemblyQualifiedName)
            {
                return new LogRepositoriePostgreSQL();
            }
            else if (driverDelegateType == typeof(OracleDelegate).AssemblyQualifiedName)
            {
                return new LogRepositorieOracle(dbProvider);
            }
            else
            {
                return null;
            }
        }
    }
}
