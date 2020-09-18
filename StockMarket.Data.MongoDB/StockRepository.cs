using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Driver;
using StockMarket.Core.Contracts;
using StockMarket.Core.Exceptions;
using StockMarket.Core.Models;

namespace StockMarket.Data.MongoDB
{
    public class StockRepository : IStockRepository
    {
        private readonly IStockDbContext dbContext;
        private readonly IMapper mapper;

        public StockRepository(IStockDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<Stock>> GetAllAsync(string filter = "")
        {
            using (IAsyncCursor<StockDocument> query = await dbContext.StockCollection.FindAsync(_ => true))
            {
                if (query == null)
                {
                    throw new NotFoundInDbException();
                }
                List<StockDocument> stocksDoc = query.ToList();

                List<Stock> allStocks = new List<Stock>();
                foreach (StockDocument stockDoc in stocksDoc)
                {
                    allStocks.Add(mapper.Map<Stock>(stockDoc));
                }
                return allStocks;
            }
        }

        public async Task<Stock> GetOneAsync(string code)
        {
            using (IAsyncCursor<StockDocument> query = await dbContext.StockCollection.FindAsync(stockDoc => stockDoc.Code == code))
            {
                if (query == null)
                {
                    throw new NotFoundInDbException();
                }
                StockDocument stockDoc = query.FirstOrDefault();
                if (stockDoc == null)
                {
                    throw new NotFoundInDbException();
                }
                return mapper.Map<Stock>(stockDoc);
            }
        }

        public async Task AddAsync(Stock stock)
        {
            long nFound = await dbContext.StockCollection.CountDocumentsAsync(stockDoc => stockDoc.Code == stock.Code);
            if (nFound > 0)
            {
                throw new AlreadyInDbException();
            }
            StockDocument stockDoc = mapper.Map<StockDocument>(stock);
            await dbContext.StockCollection.InsertOneAsync(stockDoc);
        }

        public async Task UpdateAsync(Stock stock)
        {
            long nFound = await dbContext.StockCollection.CountDocumentsAsync(stockDoc => stockDoc.Code == stock.Code);
            if (nFound <= 0)
            {
                throw new NotFoundInDbException();
            }
            StockDocument newStockDoc = mapper.Map<StockDocument>(stock);
            await dbContext.StockCollection.ReplaceOneAsync(stockDoc => stockDoc.Code == stock.Code, newStockDoc);
        }

        public async Task RemoveAsync(string code)
        {
            long nFound = await dbContext.StockCollection.CountDocumentsAsync(stockDoc => stockDoc.Code == code);
            if (nFound <= 0)
            {
                throw new NotFoundInDbException();
            }
            await dbContext.StockCollection.DeleteOneAsync(stockDoc => stockDoc.Code == code);
        }
    }
}
