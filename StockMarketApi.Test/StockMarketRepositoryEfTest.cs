using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;
using Moq;
using MockQueryable.Moq;
using StockMarket.Core.Models;
using Microsoft.EntityFrameworkCore;
using StockMarket.Data.Ef;

namespace StockMarketApi.Test
{
    public class StockMarketRepositoryEfTest
    {
        private IQueryable<Stock> fakeStocks;
        private DbContextOptions dbOptions;

        public StockMarketRepositoryEfTest()
        {
            fakeStocks = new List<Stock>()
            {
                new Stock( "Tian Stock Company", "TSC", 84, 80, "NASDAQ", false ),
                new Stock( "Silly Stock Company", "SSC", 30, 32, "NSE", false ),
                new Stock( "Lucky Stock Company", "LSC", 62, 61, "NYSE", false ),
                new Stock( "Hunan Stock Company", "HSC", 105, 108, "OTHER", false )
            }
            .AsQueryable();

            dbOptions = new DbContextOptions<StockDbContext>();
        }

        [Fact]
        public async void GetAllAsync_ReturnsCorrectResult()
        {
            var mockStocks = fakeStocks.BuildMockDbSet();

            var dbContextMock = new Mock<StockDbContext>(dbOptions);
            dbContextMock.Setup(t => t.Stocks).Returns(mockStocks.Object).Verifiable();

            var repo = new StockRepository(dbContextMock.Object);

            var result = await repo.GetAllAsync();
            Assert.NotNull(result);
            Assert.IsType<List<Stock>>(result);

            var stocksList = result as List<Stock>;
            Assert.Equal(fakeStocks.Count(), stocksList.Count);

            dbContextMock.Verify();
        }

        [Fact]
        public async void GetOneAsync_ReturnsCorrectResult()
        {
            var mockStocks = fakeStocks.BuildMockDbSet();

            var dbContextMock = new Mock<StockDbContext>(dbOptions);
            dbContextMock.Setup(t => t.Stocks).Returns(mockStocks.Object).Verifiable();

            var repo = new StockRepository(dbContextMock.Object);

            string code = "TSC";

            var result = await repo.GetOneAsync(code);
            Assert.NotNull(result);
            Assert.IsType<Stock>(result);

            Stock gotStock = result as Stock;
            Assert.Equal(code, gotStock.Code);

            code = "XXX";

            try
            {
                var result2 = await repo.GetOneAsync(code);
            }
            catch (Exception ex)
            {
                Assert.Equal("NotFoundInDbException", ex.GetType().Name);
            }

            dbContextMock.Verify();
        }

        [Fact]
        public async void AddAsync_ReturnsCorrectResult()
        {
            var mockStocks = fakeStocks.BuildMockDbSet();

            var dbContextMock = new Mock<StockDbContext>(dbOptions);
            dbContextMock.Setup(t => t.Stocks).Returns(mockStocks.Object).Verifiable();

            var repo = new StockRepository(dbContextMock.Object);

            Stock newStock = new Stock( "Zzz Stock Company", "ZSC", 86, 82, "NASDAQ", false );

            await repo.AddAsync(newStock);
            mockStocks.Verify(t => t.Add(It.IsAny<Stock>()), Times.Once());
            dbContextMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

            Stock oldStock = new Stock( "Tian Stock Company", "TSC", 84, 80, "NASDAQ", false );

            try
            {
                await repo.AddAsync(oldStock);
            }
            catch (Exception ex)
            {
                Assert.Equal("AlreadyInDbException", ex.GetType().Name);
                mockStocks.Verify(t => t.Add(It.IsAny<Stock>()), Times.Once());
                dbContextMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            }
        }

        [Fact]
        public async void UpdateAsync_ReturnsCorrectResult()
        {
            var mockStocks = fakeStocks.BuildMockDbSet();

            var dbContextMock = new Mock<StockDbContext>(dbOptions);
            dbContextMock.Setup(t => t.Stocks).Returns(mockStocks.Object).Verifiable();

            var repo = new StockRepository(dbContextMock.Object);

            Stock oldStock = new Stock("Tian Stock Company", "TSC", 84, 80, "NASDAQ", false);

            await repo.UpdateAsync(oldStock);
            mockStocks.Verify(t => t.Attach(It.IsAny<Stock>()), Times.Once());
            dbContextMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

            Stock newStock = new Stock("Zzz Stock Company", "ZSC", 86, 82, "NASDAQ", false);

            try
            {
                await repo.UpdateAsync(newStock);
            }
            catch (Exception ex)
            {
                Assert.Equal("NotFoundInDbException", ex.GetType().Name);
                mockStocks.Verify(t => t.Attach(It.IsAny<Stock>()), Times.Once());
                dbContextMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            }
        }

        [Fact]
        public async void RemoveAsync_ReturnsCorrectResult()
        {
            var mockStocks = fakeStocks.BuildMockDbSet();

            var dbContextMock = new Mock<StockDbContext>(dbOptions);
            dbContextMock.Setup(t => t.Stocks).Returns(mockStocks.Object).Verifiable();

            var repo = new StockRepository(dbContextMock.Object);

            string code = "TSC";

            await repo.RemoveAsync(code);
            mockStocks.Verify(t => t.Remove(It.IsAny<Stock>()), Times.Once());
            dbContextMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

            code = "XXX";

            try
            {
                await repo.RemoveAsync(code);
            }
            catch (Exception ex)
            {
                Assert.Equal("NotFoundInDbException", ex.GetType().Name);
                mockStocks.Verify(t => t.Remove(It.IsAny<Stock>()), Times.Once());
                dbContextMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            }
        }
    }
}
