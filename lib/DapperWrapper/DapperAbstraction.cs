using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DapperWrapper
{
    // Dev note: we do not handle connection open/close here,
    // since dapper opens the connection and closes it if it wasn't done so manually.
    // we want to preserve that behaviour for quick queries.
    /// <summary>
    /// A wrapper around Dapper to allow unit testing and mocks/stubs
    /// </summary>
    public class DapperAbstraction
    {
        public virtual Task<IEnumerable<T>> QueryAsync<T>(
            IDbConnection dbConnection,
            string sql,
            object param = null!,
            IDbTransaction transaction = null!,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return dbConnection.QueryAsync<T>(sql, param);
        }

        public virtual Task<int> ExecuteAsync(
            IDbConnection dbConnection,
            string sql,
            object param = null!,
            IDbTransaction transaction = null!,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return dbConnection.ExecuteAsync(
                sql,
                param,
                transaction,
                commandTimeout,
                commandType);
        }
    }
}
