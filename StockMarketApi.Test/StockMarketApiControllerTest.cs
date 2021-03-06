﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Xunit;
using Moq;
using StockMarket.Core.Exceptions;
using StockMarket.Core.Dtos;
using StockMarket.Api.Controllers;
using StockMarket.Core.Models;
using StockMarket.Core.Contracts;

namespace StockMarketApi.Test
{
    public class StockMarketApiControllerTest
    {
        private readonly List<Stock> fakeStocks;
        private readonly ILogger<StockController> logger;

        public StockMarketApiControllerTest()
        {
            fakeStocks = new List<Stock>()
            {
                new Stock( "Tian Stock Company", "TSC", 84, 80, "NASDAQ", false ),
                new Stock( "Silly Stock Company", "SSC", 30, 32, "NSE", false ),
                new Stock( "Lucky Stock Company", "LSC", 62, 61, "NYSE", false ),
                new Stock( "Hunan Stock Company", "HSC", 105, 108, "OTHER", false )
            };

            var mock = new Mock<ILogger<StockController>>();
            logger = mock.Object;
        }

        [Fact]
        public async void Get_ReturnsCorrectResult()
        {
            var service = new Mock<IStockService>();
            service.Setup(x => x.GetStocksAsync("")).ReturnsAsync(fakeStocks).Verifiable();

            var controller = new StockController(service.Object, logger);

            var result = await controller.Get();
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            OkObjectResult jsonResult = result as OkObjectResult;
            Assert.NotNull(jsonResult.Value);
            Assert.IsType<List<Stock>>(jsonResult.Value);

            List<Stock> gotStocks = jsonResult.Value as List<Stock>;
            Assert.Equal(fakeStocks.Count, gotStocks.Count);

            service.Verify();
        }

        [Fact]
        public async void GetByCode_ReturnsCorrectResult()
        {
            var service = new Mock<IStockService>();
            service.Setup(x => x.GetStockAsync(It.IsAny<string>())).ReturnsAsync((string code) =>
                fakeStocks.FirstOrDefault<Stock>(s => s.Code == code)
            )
            .Verifiable();

            var controller = new StockController(service.Object, logger);

            string code = "TSC";

            var result = await controller.Get(code);
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            OkObjectResult jsonResult = result as OkObjectResult;
            Assert.NotNull(jsonResult.Value);
            Assert.IsType<Stock>(jsonResult.Value);

            Stock gotStock = jsonResult.Value as Stock;
            Assert.Equal(code, gotStock.Code);

            code = "XXX";

            var result2 = await controller.Get(code);
            Assert.NotNull(result2);
            Assert.IsType<NotFoundObjectResult>(result2);

            code = "";

            var result3 = await controller.Get(code);
            Assert.NotNull(result3);
            Assert.IsType<BadRequestObjectResult>(result3);

            service.Verify();
        }

        [Fact]
        public async void Post_ReturnsCorrectResult()
        {
            var service = new Mock<IStockService>();
            service.Setup(x => x.AddStockAsync(It.IsAny<Stock>())).Callback((Stock newStock) =>
            {
                if (fakeStocks.Any(s => s.Code == newStock.Code))
                    throw new AlreadyInDbException();
            });

            var controller = new StockController(service.Object, logger);

            Stock newStock = new Stock( "Zzz Stock Company", "ZSC", 86, 82, "NASDAQ", false );

            var result = await controller.Post(newStock);
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            Stock oldStock = new Stock( "Tian Stock Company", "TSC", 84, 80, "NASDAQ", false );

            var result2 = await controller.Post(oldStock);
            Assert.NotNull(result2);
            Assert.IsType<ConflictObjectResult>(result2);

            Stock badStock = new Stock( "", "", 0, 0, "", false );

            var result3 = await controller.Post(badStock);
            Assert.NotNull(result3);
            Assert.IsType<BadRequestObjectResult>(result3);
        }

        [Fact]
        public async void Put_ReturnsCorrectResult()
        {
            var service = new Mock<IStockService>();
            service.Setup(x => x.UpdateStockAsync(It.IsAny<Stock>())).Callback((Stock oldStock) =>
            {
                if (!fakeStocks.Any(s => s.Code == oldStock.Code))
                    throw new NotFoundInDbException();
            });

            var controller = new StockController(service.Object, logger);

            Stock oldStock = new Stock("Tian Stock Company", "TSC", 84, 80, "NASDAQ", false);

            var result = await controller.Put(oldStock);
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            Stock newStock = new Stock("Zzz Stock Company", "ZSC", 86, 82, "NASDAQ", false);

            var result2 = await controller.Put(newStock);
            Assert.NotNull(result2);
            Assert.IsType<NotFoundObjectResult>(result2);

            Stock badStock = new Stock("", "", 0, 0, "", false);

            var result3 = await controller.Put(badStock);
            Assert.NotNull(result3);
            Assert.IsType<BadRequestObjectResult>(result3);
        }

        [Fact]
        public async void Delete_ReturnsCorrectResult()
        {
            var service = new Mock<IStockService>();
            service.Setup(x => x.DeleteStockAsync(It.IsAny<string>())).Callback((string code) =>
            {
                if (!fakeStocks.Any(s => s.Code == code))
                    throw new NotFoundInDbException();
            });

            var controller = new StockController(service.Object, logger);

            string code = "TSC";

            var result = await controller.Delete(code);
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            code = "XXX";

            var result2 = await controller.Delete(code);
            Assert.NotNull(result2);
            Assert.IsType<NotFoundObjectResult>(result2);

            code = "";

            var result3 = await controller.Delete(code);
            Assert.NotNull(result3);
            Assert.IsType<BadRequestObjectResult>(result3);
        }

        [Fact]
        public async void PatchByJson_ReturnsCorrectResult()
        {
            var service = new Mock<IStockService>();
            service.Setup(x => x.PatchStockAsync(It.IsAny<string>(), It.IsAny<JsonPatchDocument<Stock>>())).Callback((string code, JsonPatchDocument<Stock> patchDoc) =>
            {
                if (!fakeStocks.Any(s => s.Code == code))
                    throw new NotFoundInDbException();
            });

            var controller = new StockController(service.Object, logger);

            string code = "TSC";
            JsonPatchDocument<Stock> patchDoc = new JsonPatchDocument<Stock>();
            patchDoc.Operations.Add(
                new Microsoft.AspNetCore.JsonPatch.Operations.Operation<Stock>() { op = "replace", path = "/Favorite", value = true }
            );

            var result = await controller.Patch(code, patchDoc);
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            code = "XXX";

            var result2 = await controller.Patch(code, patchDoc);
            Assert.NotNull(result2);
            Assert.IsType<NotFoundObjectResult>(result2);

            code = "";

            var result3 = await controller.Patch(code, patchDoc);
            Assert.NotNull(result3);
            Assert.IsType<BadRequestObjectResult>(result3);
        }

        [Fact]
        public async void PatchByDto_ReturnsCorrectResult()
        {
            var service = new Mock<IStockService>();
            service.Setup(x => x.PatchStockAsync(It.IsAny<StockDto>())).Callback((StockDto stockDto) =>
            {
                if (!fakeStocks.Any(s => s.Code == stockDto.Code))
                    throw new NotFoundInDbException();
            });

            var controller = new StockController(service.Object, logger);

            StockDto stockDto = new StockDto( "TSC", 76 );

            var result = await controller.Patch(stockDto);
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            StockDto stockDto2 = stockDto with { Code = "XXX" };

            var result2 = await controller.Patch(stockDto2);
            Assert.NotNull(result2);
            Assert.IsType<NotFoundObjectResult>(result2);

            StockDto stockDto3 = stockDto with { Code = "" };

            var result3 = await controller.Patch(stockDto3);
            Assert.NotNull(result3);
            Assert.IsType<BadRequestObjectResult>(result3);
        }
    }
}
