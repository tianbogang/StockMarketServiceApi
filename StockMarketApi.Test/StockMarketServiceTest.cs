using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using Xunit;
using Moq;
using StockMarket.Core.Exceptions;
using StockMarket.Core.Dtos;
using StockMarket.Core.Models;
using StockMarket.Core.Contracts;
using StockMarket.Bll;

namespace StockMarketApi.Test
{
    public class StockMarketServiceTest
    {
        private readonly List<Stock> fakeStocks;

        public StockMarketServiceTest()
        {
            fakeStocks = new List<Stock>()
            {
                new Stock( "Tian Stock Company", "TSC", 84, 80, "NASDAQ", false ),
                new Stock( "Silly Stock Company", "SSC", 30, 32, "NSE", false ),
                new Stock( "Lucky Stock Company", "LSC", 62, 61, "NYSE", false ),
                new Stock( "Hunan Stock Company", "HSC", 105, 108, "OTHER", false )
            };
        }

        [Fact]
        public async void GetStocksAsync_ReturnsCorrectResult()
        {
            var repo = new Mock<IStockRepository>();
            repo.Setup(x => x.GetAllAsync("")).ReturnsAsync(fakeStocks).Verifiable();

            var service = new StockService(repo.Object);

            var result = await service.GetStocksAsync();
            Assert.NotNull(result);
            Assert.IsType<List<Stock>>(result);

            var stocksList = result as List<Stock>;
            Assert.Equal(fakeStocks.Count(), stocksList.Count);

            repo.Verify();
        }

        [Fact]
        public async void GetStockByCodeAsync_ReturnsCorrectResult()
        {
            var repo = new Mock<IStockRepository>();
            repo.Setup(x => x.GetOneAsync(It.IsAny<string>())).ReturnsAsync((string code) => {
                Stock theStock = fakeStocks.FirstOrDefault<Stock>(s => s.Code == code);
                if (theStock == null)
                    throw new NotFoundInDbException();
                else
                    return theStock;
            })
            .Verifiable();

            var service = new StockService(repo.Object);

            string code = "TSC";

            var result = await service.GetStockAsync(code);
            Assert.NotNull(result);
            Assert.IsType<Stock>(result);

            Stock gotStock = result as Stock;
            Assert.Equal(code, gotStock.Code);

            code = "XXX";

            try
            {
                var result2 = await service.GetStockAsync(code);
                Assert.Null(result2); // never reach it

            }
            catch (Exception ex)
            {
                Assert.Equal("NotFoundInDbException", ex.GetType().Name);
            }

            repo.Verify();
        }

        [Fact]
        public async void AddStockAsync_ReturnsCorrectResult()
        {
            var repo = new Mock<IStockRepository>();
            repo.Setup(x => x.AddAsync(It.IsAny<Stock>())).Callback((Stock newStock) => {
                if (fakeStocks.Any(s => s.Code == newStock.Code))
                    throw new AlreadyInDbException();
            });

            var service = new StockService(repo.Object);

            Stock newStock = new Stock("Zzz Stock Company", "ZSC", 86, 82, "NASDAQ", false);

            await service.AddStockAsync(newStock);
            repo.Verify(t => t.AddAsync(It.IsAny<Stock>()), Times.Once());

            Stock oldStock = new Stock("Tian Stock Company", "TSC", 84, 80, "NASDAQ", false);

            try
            {
                await service.AddStockAsync(oldStock);

            }
            catch (Exception ex)
            {
                Assert.Equal("AlreadyInDbException", ex.GetType().Name);
                repo.Verify(t => t.AddAsync(It.IsAny<Stock>()), Times.Exactly(2));
            }
        }

        [Fact]
        public async void UpdateStockAsync_ReturnsCorrectResult()
        {
            var repo = new Mock<IStockRepository>();
            repo.Setup(x => x.UpdateAsync(It.IsAny<Stock>())).Callback((Stock oldStock) => {
                if (!fakeStocks.Any(s => s.Code == oldStock.Code))
                    throw new NotFoundInDbException();
            });

            var service = new StockService(repo.Object);

            Stock oldStock = new Stock("Tian Stock Company", "TSC", 84, 80, "NASDAQ", false);

            await service.UpdateStockAsync(oldStock);
            repo.Verify(t => t.UpdateAsync(It.IsAny<Stock>()), Times.Once());

            Stock newStock = new Stock("Zzz Stock Company", "ZSC", 86, 82, "NASDAQ", false);

            try
            {
                await service.UpdateStockAsync(newStock);
            }
            catch (Exception ex)
            {
                Assert.Equal("NotFoundInDbException", ex.GetType().Name);
                repo.Verify(t => t.UpdateAsync(It.IsAny<Stock>()), Times.Exactly(2));
            }
        }

        [Fact]
        public async void DeleteStockAsync_ReturnsCorrectResult()
        {
            var repo = new Mock<IStockRepository>();
            repo.Setup(x => x.RemoveAsync(It.IsAny<string>())).Callback((string code) => {
                if (!fakeStocks.Any(s => s.Code == code))
                    throw new NotFoundInDbException();
            });

            var service = new StockService(repo.Object);

            string code = "TSC";

            await service.DeleteStockAsync(code);
            repo.Verify(t => t.RemoveAsync(It.IsAny<string>()), Times.Once());

            code = "XXX";

            try
            {
                await service.DeleteStockAsync(code);
            }
            catch (Exception ex)
            {
                Assert.Equal("NotFoundInDbException", ex.GetType().Name);
                repo.Verify(t => t.RemoveAsync(It.IsAny<string>()), Times.Exactly(2));
            }
        }


        [Fact]
        public async void PatchStockByJsonAsync_ReturnsCorrectResult()
        {
            var repo = new Mock<IStockRepository>();
            repo.Setup(x => x.GetOneAsync(It.IsAny<string>())).ReturnsAsync((string code) => {
                Stock theStock = fakeStocks.FirstOrDefault<Stock>(s => s.Code == code);
                if (theStock == null)
                    throw new NotFoundInDbException();
                else
                    return theStock;
            })
            .Verifiable();
            repo.Setup(x => x.UpdateAsync(It.IsAny<Stock>())).Callback((Stock oldStock) => {
                if (!fakeStocks.Any(s => s.Code == oldStock.Code))
                    throw new NotFoundInDbException();
            });

            var service = new StockService(repo.Object);

            string code = "TSC";
            JsonPatchDocument<Stock> patchDoc = new JsonPatchDocument<Stock>();
            patchDoc.Operations.Add(
                new Microsoft.AspNetCore.JsonPatch.Operations.Operation<Stock>() { op = "replace", path = "/Favorite", value = true }
            );

            await service.PatchStockAsync(code, patchDoc);

            repo.Verify();
            repo.Verify(t => t.UpdateAsync(It.IsAny<Stock>()), Times.Once());

            code = "XXX";

            try
            {
                await service.PatchStockAsync(code, patchDoc);
            }
            catch (Exception ex)
            {
                Assert.Equal("NotFoundInDbException", ex.GetType().Name);
                repo.Verify();
                repo.Verify(t => t.UpdateAsync(It.IsAny<Stock>()), Times.Once());
            }
        }

        [Fact]
        public async void PatchStockByDtoAsync_ReturnsCorrectResult()
        {
            var repo = new Mock<IStockRepository>();
            repo.Setup(x => x.GetOneAsync(It.IsAny<string>())).ReturnsAsync((string code) => {
                Stock theStock = fakeStocks.FirstOrDefault<Stock>(s => s.Code == code);
                if (theStock == null)
                    throw new NotFoundInDbException();
                else
                    return theStock;
            })
            .Verifiable();
            repo.Setup(x => x.UpdateAsync(It.IsAny<Stock>())).Callback((Stock oldStock) => {
                if (!fakeStocks.Any(s => s.Code == oldStock.Code))
                    throw new NotFoundInDbException();
            });

            var service = new StockService(repo.Object);

            StockDto stockDto = new StockDto( "TSC", 81 );

            await service.PatchStockAsync(stockDto);

            repo.Verify();
            repo.Verify(t => t.UpdateAsync(It.IsAny<Stock>()), Times.Once());

            StockDto stockDto2 = stockDto with { Code = "XXX" };

            try
            {
                await service.PatchStockAsync(stockDto2);
            }
            catch (Exception ex)
            {
                Assert.Equal("NotFoundInDbException", ex.GetType().Name);
                repo.Verify();
                repo.Verify(t => t.UpdateAsync(It.IsAny<Stock>()), Times.Once());
            }
        }
    }
}
