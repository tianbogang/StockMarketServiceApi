using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;

namespace StockMarket.Data.Dapper
{
    public class StockDbContext : IStockDbContext
    {
        private readonly string connectString;

        public StockDbContext(string connectString)
        {
            this.connectString = connectString;
        }

        // public IDbConnection DbConnection => new SqlConnection(connectString);

        public async Task<List<T>> QueryAsync<T>(string sql, object param = null) where T : class
        {
            using (IDbConnection conn = new SqlConnection(connectString))
            {
                return (await conn.QueryAsync<T>(sql, param)).ToList();
            }
        }

        public async Task<T> QueryFirstAsync<T>(string sql, object param = null)
        {
            using (IDbConnection conn = new SqlConnection(connectString))
            {
                return await conn.QueryFirstOrDefaultAsync<T>(sql, param);
            }
        }

        public async Task<int> ExecuteAsync(string sql, object param = null)
        {
            using (IDbConnection conn = new SqlConnection(connectString))
            {
                return await conn.ExecuteAsync(sql, param);
            }
        }
    }
}
