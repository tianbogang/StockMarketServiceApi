using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StockMarket.Core.Contracts;
using StockMarket.Core.Models;
using StockMarket.Core.Exceptions;

namespace StockMarket.Data.Ef
{
    public class StockRepository : IStockRepository
    {
        private readonly StockDbContext dbContext;

        public StockRepository(StockDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Stock>> GetAllAsync(string filter = "")
        {
            if(String.IsNullOrEmpty(filter))
            {
                return await dbContext.Stocks.AsNoTracking().ToListAsync();
            }
            else
            {
                return await dbContext.Stocks.AsNoTracking().Where(s => s.Code.Contains(filter)).ToListAsync();
            }
        }

        public async Task<Stock> GetOneAsync(string code)
        {
            Stock stock = await dbContext.Stocks.FirstOrDefaultAsync<Stock>(s => s.Code == code);
            if (stock == null)
            {
                throw new NotFoundInDbException();
            }
            else
            {
                return stock;
            }
        }

        public async Task AddAsync(Stock stock)
        {
            if (await dbContext.Stocks.AnyAsync<Stock>(s => s.Code == stock.Code))
            {
                throw new AlreadyInDbException();
            }
            dbContext.Stocks.Add(stock);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Stock stock)
        {
            if (await dbContext.Stocks.AnyAsync<Stock>(s => s.Code == stock.Code))
            {
                dbContext.Stocks.Attach(stock);
                // dbContext.Entry(stock).State = EntityState.Modified;  // not testable
                dbContext.SetModified(stock);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                throw new NotFoundInDbException();
            }
        }

        public async Task RemoveAsync(string code)
        {
            Stock thisStock = await dbContext.Stocks.FirstOrDefaultAsync(s => s.Code == code);
            if (thisStock != null)
            {
                dbContext.Stocks.Remove(thisStock);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                throw new NotFoundInDbException();
            }
        }
    }
}
