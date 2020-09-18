using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using StockMarket.Core.Models;
using StockMarket.Data.Dapper;

namespace StockMarketApi.Test
{
    public class StockMarketRepositoryDapperTest
    {
        private List<Stock> fakeStocks;

        public StockMarketRepositoryDapperTest()
        {
            fakeStocks = new List<Stock>()
            {
                new Stock { Name = "Tian Stock Company", Code = "TSC", Price = 84, PreviousPrice = 80, Exchange = "NASDAQ", Favorite = false },
                new Stock { Name = "Silly Stock Company", Code = "SSC", Price = 30, PreviousPrice = 32, Exchange = "NSE", Favorite = false },
                new Stock { Name = "Lucky Stock Company", Code = "LSC", Price = 62, PreviousPrice = 61, Exchange = "NYSE", Favorite = false },
                new Stock { Name = "Hunan Stock Company", Code = "HSC", Price = 105, PreviousPrice = 108, Exchange = "OTHER", Favorite = false }
            };
        }

        [Fact]
        public async void GetAllAsync_ReturnsCorrectResult()
        {
            var dbContextMock = new Mock<IStockDbContext>();
            dbContextMock.Setup(d => d.QueryAsync<Stock>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(fakeStocks);

            var repo = new StockRepository(dbContextMock.Object);

            var result = await repo.GetAllAsync();
            Assert.NotNull(result);
            Assert.IsType<List<Stock>>(result);

            var stocksTable = result as List<Stock>;
            Assert.Equal(fakeStocks.Count(), stocksTable.Count);
        }

        [Fact]
        public async void GetOneAsync_ReturnsCorrectResult()
        {
            var dbContextMock = new Mock<IStockDbContext>();
            dbContextMock.Setup(d => d.QueryFirstAsync<Stock>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((string _, object p) =>
                {
                    string code = p.GetType().GetProperty("Code").GetValue(p) as string;
                    return fakeStocks.FirstOrDefault<Stock>(s => s.Code == code);
                });

            var repo = new StockRepository(dbContextMock.Object);

            string code = "TSC";

            var result = await repo.GetOneAsync(code);
            Assert.NotNull(result);
            Assert.IsType<Stock>(result);

            Stock foundStock = result as Stock;
            Assert.Equal(code, foundStock.Code);

            code = "XXX";

            var result2 = await repo.GetOneAsync(code);
            Assert.Null(result2);
        }

        [Fact]
        public async void AddAsync_ReturnsCorrectResult()
        {
            var dbContextMock = new Mock<IStockDbContext>();
            dbContextMock.Setup(d => d.QueryAsync<object>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((string _, object p) =>
                {
                    string code = p.GetType().GetProperty("Code").GetValue(p) as string;
                    return fakeStocks.Any<Stock>(s => s.Code == code) ? new List<object>() { 1 } : new List<object>();
                });
            dbContextMock.Setup(d => d.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(1);

            var repo = new StockRepository(dbContextMock.Object);

            Stock newStock = new Stock { Name = "Zzz Stock Company", Code = "ZSC", Price = 86, PreviousPrice = 82, Exchange = "NASDAQ", Favorite = false };

            await repo.AddAsync(newStock);
            dbContextMock.Verify(c => c.QueryAsync<object>(It.IsAny<string>(), It.IsAny<object>()), Times.Once());
            dbContextMock.Verify(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Once());

            Stock oldStock = new Stock { Name = "Tian Stock Company", Code = "TSC", Price = 84, PreviousPrice = 80, Exchange = "NASDAQ", Favorite = false };

            try
            {
                await repo.AddAsync(oldStock);
            }
            catch (Exception ex)
            {
                Assert.Equal("AlreadyInDbException", ex.GetType().Name);
                dbContextMock.Verify(c => c.QueryAsync<object>(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(2));
                dbContextMock.Verify(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Once());
            }
        }

        [Fact]
        public async void UpdateAsync_ReturnsCorrectResult()
        {
            var dbContextMock = new Mock<IStockDbContext>();
            dbContextMock.Setup(d => d.QueryAsync<object>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((string _, object p) =>
                {
                    string code = p.GetType().GetProperty("Code").GetValue(p) as string;
                    return fakeStocks.Any<Stock>(s => s.Code == code) ? new List<object>() { 1 } : new List<object>();
                });
            dbContextMock.Setup(d => d.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(1);

            var repo = new StockRepository(dbContextMock.Object);

            Stock oldStock = new Stock { Name = "Tian Stock Company", Code = "TSC", Price = 84, PreviousPrice = 80, Exchange = "NASDAQ", Favorite = false };

            await repo.UpdateAsync(oldStock);
            dbContextMock.Verify(c => c.QueryAsync<object>(It.IsAny<string>(), It.IsAny<object>()), Times.Once());
            dbContextMock.Verify(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Once());

            Stock newStock = new Stock { Name = "Zzz Stock Company", Code = "ZSC", Price = 86, PreviousPrice = 82, Exchange = "NASDAQ", Favorite = false };

            try
            {
                await repo.UpdateAsync(newStock);
            }
            catch (Exception ex)
            {
                Assert.Equal("NotFoundInDbException", ex.GetType().Name);
                dbContextMock.Verify(c => c.QueryAsync<object>(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(2));
                dbContextMock.Verify(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Once());
            }
        }

        [Fact]
        public async void RemoveAsync_ReturnsCorrectResult()
        {
            var dbContextMock = new Mock<IStockDbContext>();
            dbContextMock.Setup(d => d.QueryAsync<object>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((string _, object p) =>
                {
                    string code = p.GetType().GetProperty("Code").GetValue(p) as string;
                    return fakeStocks.Any<Stock>(s => s.Code == code) ? new List<object>() { 1 } : new List<object>();
                });
            dbContextMock.Setup(d => d.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(1);

            var repo = new StockRepository(dbContextMock.Object);

            string code = "TSC";

            await repo.RemoveAsync(code);
            dbContextMock.Verify(c => c.QueryAsync<object>(It.IsAny<string>(), It.IsAny<object>()), Times.Once());
            dbContextMock.Verify(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Once());

            code = "XXX";

            try
            {
                await repo.RemoveAsync(code);
            }
            catch (Exception ex)
            {
                Assert.Equal("NotFoundInDbException", ex.GetType().Name);
                dbContextMock.Verify(c => c.QueryAsync<object>(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(2));
                dbContextMock.Verify(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Once());
            }
        }
    }
}
