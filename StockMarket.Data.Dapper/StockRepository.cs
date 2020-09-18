using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockMarket.Core.Contracts;
using StockMarket.Core.Models;
using StockMarket.Core.Exceptions;

namespace StockMarket.Data.Dapper
{
    public class StockRepository : IStockRepository
    {
        private readonly IStockDbContext dbContext;

        public StockRepository(IStockDbContext context)
        {
            dbContext = context;
        }

        public async Task<bool> StockFound(string code)
        {
            var result = await dbContext.QueryAsync<object>(StockDbQuery.StockExists, new { Code = code });
            return (result.Count() > 0);
        }

        public async Task<IEnumerable<Stock>> GetAllAsync(string filter = "")
        {
            if (String.IsNullOrEmpty(filter))
            {
                return await dbContext.QueryAsync<Stock>(StockDbQuery.GetStocks);
            }
            else
            {
                return await dbContext.QueryAsync<Stock>(StockDbQuery.GetStocksWhere, new { Filter = filter });
            }
        }

        public async Task<Stock> GetOneAsync(string code)
        {
            Stock stock = await dbContext.QueryFirstAsync<Stock>(StockDbQuery.GetStockByCode, new { Code = code });
            return stock;
        }

        public async Task AddAsync(Stock stock)
        {
            var found = await StockFound(stock.Code);
            if (found)
            {
                throw new AlreadyInDbException();
            }

            await dbContext.ExecuteAsync(StockDbQuery.AddStock, stock);
        }

        public async Task UpdateAsync(Stock stock)
        {
            var found = await StockFound(stock.Code);
            if (!found)
            {
                throw new NotFoundInDbException();
            }

            await dbContext.ExecuteAsync(StockDbQuery.UpdateStock, stock);
        }

        public async Task RemoveAsync(string code)
        {
            var found = await StockFound(code);
            if (!found)
            {
                throw new NotFoundInDbException();
            }

            await dbContext.ExecuteAsync(StockDbQuery.DeleteStock, new { Code = code });
        }
    }
}
