using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using StockMarket.Core.Contracts;
using StockMarket.Core.Dtos;
using StockMarket.Core.Models;

namespace StockMarket.Bll
{
    public class StockService : IStockService
    {
        private readonly IStockRepository repository;

        public StockService(IStockRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<Stock>> GetStocksAsync(string filter = "")
        {
            return await repository.GetAllAsync(filter);
        }

        public async Task<Stock> GetStockAsync(string code)
        {
            return await repository.GetOneAsync(code);
        }

        public async Task AddStockAsync(Stock stock)
        {
            await repository.AddAsync(stock);
        }

        public async Task UpdateStockAsync(Stock stock)
        {
            await repository.UpdateAsync(stock);
        }

        public async Task DeleteStockAsync(string code)
        {
            await repository.RemoveAsync(code);
        }

        public async Task PatchStockAsync(string code, JsonPatchDocument<Stock> patchDoc)
        {
            Stock thisStock = await repository.GetOneAsync(code);  // will throw NotFoundInDbException if not found
            patchDoc.ApplyTo(thisStock);
            await repository.UpdateAsync(thisStock);
        }

        public async Task PatchStockAsync(StockDto stockDto)
        {
            Stock thisStock = await repository.GetOneAsync(stockDto.Code);  
            thisStock.PreviousPrice = thisStock.Price;
            thisStock.Price = stockDto.Price;
            await repository.UpdateAsync(thisStock);
        }
    }
}
