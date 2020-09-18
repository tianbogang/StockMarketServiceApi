using System.Collections.Generic;
using System.Threading.Tasks;
using StockMarket.Core.Models;

namespace StockMarket.Core.Contracts
{
    public interface IStockRepository
    {
        Task<IEnumerable<Stock>> GetAllAsync(string filter = "");
        Task<Stock> GetOneAsync(string code);
        Task AddAsync(Stock entity);
        Task UpdateAsync(Stock entity);
        Task RemoveAsync(string code);
    }
}
