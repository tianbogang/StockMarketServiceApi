using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using StockMarket.Core.Dtos;
using StockMarket.Core.Models;

namespace StockMarket.Core.Contracts
{
    public interface IStockService
    {
        Task<IEnumerable<Stock>> GetStocksAsync(string filter = "");
        Task<Stock> GetStockAsync(string code);
        Task AddStockAsync(Stock stock);
        Task UpdateStockAsync(Stock stock);
        Task DeleteStockAsync(string code);
        Task PatchStockAsync(string code, JsonPatchDocument<Stock> patchDoc);
        Task PatchStockAsync(StockDto stockDto);
    }
}
