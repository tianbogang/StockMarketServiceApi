using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using StockMarket.Core.Models;
using AutoMapper;
using StockMarket.Data.MongoDB;
using MongoDB.Driver;
using System.Threading;

namespace StockMarketApi.Test
{
    public class StockMarketRepositoryMongoDbTest
    {
        private IMapper mapper;

        public StockMarketRepositoryMongoDbTest()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            mapper = mappingConfig.CreateMapper();
        }

        [Fact]
        public async void GetAllAsync_ReturnsCorrectResult()
        {
            var fakeStockCollection = new List<StockDocument>()
            {
                new StockDocument { Name = "Tian Stock Company", Code = "TSC", Price = 84, PreviousPrice = 80, Exchange = "NASDAQ", Favorite = false },
                new StockDocument { Name = "Silly Stock Company", Code = "SSC", Price = 30, PreviousPrice = 32, Exchange = "NSE", Favorite = false },
                new StockDocument { Name = "Lucky Stock Company", Code = "LSC", Price = 62, PreviousPrice = 61, Exchange = "NYSE", Favorite = false },
                new StockDocument { Name = "Hunan Stock Company", Code = "HSC", Price = 105, PreviousPrice = 108, Exchange = "OTHER", Favorite = false }
            };

            var cursor = new Mock<IAsyncCursor<StockDocument>>();
            cursor.Setup(_ => _.Current).Returns(fakeStockCollection);
            cursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);

            var stockCollection = new Mock<IMongoCollection<StockDocument>>();
            stockCollection.Setup(q => q.FindAsync(
                It.IsAny<FilterDefinition<StockDocument>>(),
                It.IsAny<FindOptions<StockDocument, StockDocument>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var context = new Mock<IStockDbContext>();
            context.Setup(s => s.StockCollection).Returns(stockCollection.Object).Verifiable();

            var repo = new StockRepository(context.Object, mapper);

            var result = await repo.GetAllAsync();
            Assert.NotNull(result);
            Assert.IsType<List<Stock>>(result);

            List<Stock> stocksTable = result as List<Stock>;
            Assert.Equal(fakeStockCollection.Count(), stocksTable.Count);

            context.Verify();
        }

        [Fact]
        public async void GetOneAsync_ReturnsCorrectResult()
        {
            var fakeStockCollection = new List<StockDocument>()
            {
                new StockDocument { Name = "Tian Stock Company", Code = "TSC", Price = 84, PreviousPrice = 80, Exchange = "NASDAQ", Favorite = false },
                new StockDocument { Name = "Silly Stock Company", Code = "SSC", Price = 30, PreviousPrice = 32, Exchange = "NSE", Favorite = false },
                new StockDocument { Name = "Lucky Stock Company", Code = "LSC", Price = 62, PreviousPrice = 61, Exchange = "NYSE", Favorite = false },
                new StockDocument { Name = "Hunan Stock Company", Code = "HSC", Price = 105, PreviousPrice = 108, Exchange = "OTHER", Favorite = false }
            };

            var cursor = new Mock<IAsyncCursor<StockDocument>>();
            cursor.Setup(_ => _.Current).Returns(fakeStockCollection);
            cursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);

            var stockCollection = new Mock<IMongoCollection<StockDocument>>();
            stockCollection.Setup(q => q.FindAsync(
                It.IsAny<FilterDefinition<StockDocument>>(),
                It.IsAny<FindOptions<StockDocument, StockDocument>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);

            var context = new Mock<IStockDbContext>();
            context.Setup(s => s.StockCollection).Returns(stockCollection.Object).Verifiable();

            var repo = new StockRepository(context.Object, mapper);

            string code = "TSC";

            var result = await repo.GetOneAsync(code);
            Assert.NotNull(result);
            Assert.IsType<Stock>(result);

            Stock foundStock = result as Stock;
            Assert.Equal(code, foundStock.Code);

            code = "XXX";
            cursor.Setup(_ => _.Current).Returns((List<StockDocument>)null);

            try
            {
                var result2 = await repo.GetOneAsync(code);
            }
            catch (Exception ex)
            {
                Assert.Equal("NotFoundInDbException", ex.GetType().Name);
            }

            context.Verify();
        }

        [Fact]
        public async void AddAsync_ReturnsCorrectResult()
        {
            var stockCollection = new Mock<IMongoCollection<StockDocument>>();
            var context = new Mock<IStockDbContext>();
            context.Setup(s => s.StockCollection).Returns(stockCollection.Object).Verifiable();

            var repo = new StockRepository(context.Object, mapper);

            Stock newStock = new Stock { Name = "Zzz Stock Company", Code = "ZSC", Price = 86, PreviousPrice = 82, Exchange = "NASDAQ", Favorite = false };
            stockCollection.Setup(c => c.CountDocumentsAsync(
                It.IsAny<FilterDefinition<StockDocument>>(),
                It.IsAny<CountOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            await repo.AddAsync(newStock);
            stockCollection.Verify(c => c.InsertOneAsync(It.IsAny<StockDocument>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Once());

            Stock oldStock = new Stock { Name = "Tian Stock Company", Code = "TSC", Price = 84, PreviousPrice = 80, Exchange = "NASDAQ", Favorite = false };
            stockCollection.Setup(c => c.CountDocumentsAsync(
                It.IsAny<FilterDefinition<StockDocument>>(),
                It.IsAny<CountOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            try
            {
                await repo.AddAsync(oldStock);
            }
            catch (Exception ex)
            {
                Assert.Equal("AlreadyInDbException", ex.GetType().Name);
                stockCollection.Verify(c => c.InsertOneAsync(It.IsAny<StockDocument>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Once());
            }

            context.Verify();
        }

        [Fact]
        public async void UpdateAsync_ReturnsCorrectResult()
        {
            var stockCollection = new Mock<IMongoCollection<StockDocument>>();
            var context = new Mock<IStockDbContext>();
            context.Setup(s => s.StockCollection).Returns(stockCollection.Object).Verifiable();

            var repo = new StockRepository(context.Object, mapper);

            Stock oldStock = new Stock { Name = "Tian Stock Company", Code = "TSC", Price = 84, PreviousPrice = 80, Exchange = "NASDAQ", Favorite = false };
            stockCollection.Setup(c => c.CountDocumentsAsync(
                It.IsAny<FilterDefinition<StockDocument>>(),
                It.IsAny<CountOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await repo.UpdateAsync(oldStock);
            stockCollection.Verify(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<StockDocument>>(),
                It.IsAny<StockDocument>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()),
                Times.Once());

            Stock newStock = new Stock { Name = "Zzz Stock Company", Code = "ZSC", Price = 86, PreviousPrice = 82, Exchange = "NASDAQ", Favorite = false };
            stockCollection.Setup(c => c.CountDocumentsAsync(
                It.IsAny<FilterDefinition<StockDocument>>(),
                It.IsAny<CountOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            try
            {
                await repo.UpdateAsync(newStock);
            }
            catch (Exception ex)
            {
                Assert.Equal("NotFoundInDbException", ex.GetType().Name);
                stockCollection.Verify(c => c.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<StockDocument>>(),
                    It.IsAny<StockDocument>(),
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()),
                    Times.Once());
            }

            context.Verify();
        }

        [Fact]
        public async void RemoveAsync_ReturnsCorrectResult()
        {
            var stockCollection = new Mock<IMongoCollection<StockDocument>>();
            var context = new Mock<IStockDbContext>();
            context.Setup(s => s.StockCollection).Returns(stockCollection.Object).Verifiable();

            var repo = new StockRepository(context.Object, mapper);

            string code = "TSC";
            stockCollection.Setup(c => c.CountDocumentsAsync(
                It.IsAny<FilterDefinition<StockDocument>>(),
                It.IsAny<CountOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await repo.RemoveAsync(code);
            stockCollection.Verify(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<StockDocument>>(), It.IsAny<CancellationToken>()), Times.Once());

            code = "XXX";
            stockCollection.Setup(c => c.CountDocumentsAsync(
                It.IsAny<FilterDefinition<StockDocument>>(),
                It.IsAny<CountOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            try
            {
                await repo.RemoveAsync(code);
            }
            catch (Exception ex)
            {
                Assert.Equal("NotFoundInDbException", ex.GetType().Name);
                stockCollection.Verify(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<StockDocument>>(), It.IsAny<CancellationToken>()), Times.Once());
            }

            context.Verify();
        }
    }
}
