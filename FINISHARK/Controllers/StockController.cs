using FINISHARK.Data;
using FINISHARK.DTO.Stock;
using FINISHARK.Helpers;
using FINISHARK.Interfaces;
using FINISHARK.Mappers;
using FINISHARK.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FINISHARK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStockRepo _stockRepo;
        private readonly UserManager<AppUser> _userManager;

        public StockController(ApplicationDbContext context, IStockRepo stockRepo, UserManager<AppUser> userManager)
        {
            _context = context;
            _stockRepo = stockRepo;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStocks([FromQuery] QueryObject query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var Stocks = await _stockRepo.GetAllStockAsync(query);
            var StockDto = Stocks.Select(s => s.toStockDto());
            return Ok(StockDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBYId([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var stock = await _stockRepo.GetByIdAsync(id);
            if (stock == null)
            {
                return NotFound();
            }
            return Ok(stock.toStockDto());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStock(
            [FromBody] CreateStockRequestDto stockRequestDto
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var StockModel = stockRequestDto.toCreateStockDto();
            await _stockRepo.CreateStockAsync(StockModel);
            return CreatedAtAction(
                nameof(GetBYId),
                new { id = StockModel.Id },
                StockModel.toStockDto()
            );
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStock(
            [FromRoute] int id,
            [FromBody] UpdateStockRequestDto updateStockRequestDto
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var stock = await _stockRepo.UpdateStockAsync(id, updateStockRequestDto);
            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock.toStockDto());
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStock([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var stock = await _stockRepo.DeleteStockAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
