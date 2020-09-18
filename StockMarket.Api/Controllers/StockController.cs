using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockMarket.Api.Validators;
using StockMarket.Core.Contracts;
using StockMarket.Core.Dtos;
using StockMarket.Core.Exceptions;
using StockMarket.Core.Models;

namespace StockMarket.Api.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StockController : ControllerBase
    {
        private readonly IStockService stockService;
        private readonly ILogger<StockController> logger;

        public StockController(IStockService stockService, ILogger<StockController> logger)
        {
            this.stockService = stockService;
            this.logger = logger;
        }

        /// <summary>
        /// Get a list of all stocks.
        /// </summary>        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var stocks = await stockService.GetStocksAsync();
            if (stocks == null || stocks.Count() <= 0) 
                return NotFound("Get all stocks: No data.");

            logger.LogInformation($"Get all stocks: {stocks.Count()} stocks returned.");
            return Ok(stocks);
        }

        /// <summary>
        /// Get the stock by code.
        /// </summary>
        /// <param name="code"></param>  
        [HttpGet("{code}", Name = "Get")]
        public async Task<IActionResult> Get(string code)
        {
            if (String.IsNullOrEmpty(code)) 
                return BadRequest($"Invalid stock code");

            try
            {
                Stock stock = await stockService.GetStockAsync(code);
                if (stock == null)
                    return NotFound($"No stock with such a code: {code}");
                else
                    return Ok(stock);
            }
            catch (NotFoundInDbException ntEx)
            {
                return NotFound(ntEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Add a new stock.
        /// </summary>
        /// <param name="stock"></param>  
        [HttpPost]
        public async Task<IActionResult> Post(Stock stock)
        {
            var validator = new AddUpdateStockValidator();
            var validationResult = await validator.ValidateAsync(stock);
            if (!validationResult.IsValid) 
                return BadRequest(validationResult.Errors);

            try
            {
                await stockService.AddStockAsync(stock);
                return Ok("Stock added");
            }
            catch (AlreadyInDbException aldEx)
            {
                return Conflict(aldEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing stock.
        /// </summary>
        /// <param name="stock"></param>  
        [HttpPut]
        public async Task<IActionResult> Put(Stock stock)
        {
            var validator = new AddUpdateStockValidator();
            var validationResult = await validator.ValidateAsync(stock);
            if (!validationResult.IsValid) 
                return BadRequest(validationResult.Errors);

            try
            {
                await stockService.UpdateStockAsync(stock);
                return Ok("Stock updated");
            }
            catch (NotFoundInDbException ntEx)
            {
                return NotFound(ntEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete the stock by code.
        /// </summary>
        /// <param name="code"></param>  
        [HttpDelete("{code}")]
        public async Task<IActionResult> Delete(string code)
        {
            if(String.IsNullOrEmpty(code)) 
                return BadRequest($"Invalid stock code: {code}");

            try
            {
                await stockService.DeleteStockAsync(code);
                return Ok("Stock deleted");
            }
            catch (NotFoundInDbException ntEx)
            {
                return NotFound(ntEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Patch the stock by code and a JsonPatchDocument.
        /// </summary>
        /// <param name="stockDto"></param>  
        [HttpPatch("{code}", Name = "Patch")]
        public async Task<IActionResult> Patch(string code, [FromBody] JsonPatchDocument<Stock> patchDoc)
        {
            if (String.IsNullOrEmpty(code)) 
                return BadRequest($"Invalid stock code");

            try
            {
                await stockService.PatchStockAsync(code, patchDoc);
                return Ok("Stock patched");
            }
            catch (NotFoundInDbException ntEx)
            {
                return NotFound(ntEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Patch the stock by StockDto.
        /// </summary>
        /// <param name="stockDto"></param>  
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody] StockDto stockDto)
        {
            var validator = new UpdateStockDtoValidator();
            var validationResult = await validator.ValidateAsync(stockDto);
            if (!validationResult.IsValid) 
                return BadRequest(validationResult.Errors);

            try
            {
                await stockService.PatchStockAsync(stockDto);
                return Ok("Stock price updated");
            }
            catch (NotFoundInDbException ntEx)
            {
                return NotFound(ntEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
