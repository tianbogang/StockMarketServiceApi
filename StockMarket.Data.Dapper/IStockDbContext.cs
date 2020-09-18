using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockMarket.Data.Dapper
{
    public interface IStockDbContext
    {
        Task<List<T>> QueryAsync<T>(string sql, object param = null) where T : class;
        Task<T> QueryFirstAsync<T>(string sql, object param = null);
        Task<int> ExecuteAsync(string sql, object param = null);
    }
}
