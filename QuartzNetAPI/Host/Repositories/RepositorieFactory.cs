using Quartz.Impl.AdoJobStore;
using Quartz.Impl.AdoJobStore.Common;

namespace Host.Repositories
{
    public class RepositorieFactory
    {
        public static IRepositorie CreateRepositorie(string driverDelegateType, IDbProvider dbProvider)
        {

            if (driverDelegateType == typeof(SQLiteDelegate).AssemblyQualifiedName)
            {
                return new RepositorieSQLite(dbProvider);
            }
            else if (driverDelegateType == typeof(MySQLDelegate).AssemblyQualifiedName)
            {
                return new RepositorieMySql(dbProvider);
            }
            else if (driverDelegateType == typeof(PostgreSQLDelegate).AssemblyQualifiedName)
            {
                return new RepositoriePostgreSQL(dbProvider);
            }
            else if (driverDelegateType == typeof(OracleDelegate).AssemblyQualifiedName)
            {
                return new RepositorieOracle(dbProvider);
            }
            else
            {
                return null;
            }
        }
    }
}
