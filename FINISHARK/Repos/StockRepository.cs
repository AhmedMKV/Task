using FINISHARK.Data;
using FINISHARK.DTO.Stock;
using FINISHARK.Helpers;
using FINISHARK.Interfaces;
using FINISHARK.Models;
using Microsoft.EntityFrameworkCore;

namespace FINISHARK.Repos
{
    public class StockRepository : IStockRepo
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Stock> CreateStockAsync(Stock stock)
        {
            await _context.AddAsync(stock);
            await _context.SaveChangesAsync();
            return stock;
        }

        public async Task<Stock?> DeleteStockAsync(int id)
        {
            var stock = await _context.Stock.FindAsync(id);
            if (stock == null)
            {
                return null;
            }
            _context.Stock.Remove(stock);
            await _context.SaveChangesAsync();
            return stock;
        }

        public async Task<List<Stock>> GetAllStockAsync(QueryObject query)
        {
            var stocks = _context.Stock.Include(x => x.Comments).ThenInclude(a=>a.User).AsQueryable();
            if (!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.Contains(query.CompanyName));
            }
            if (!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));
            }
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDescending
                        ? stocks.OrderByDescending(s => s.Symbol)
                        : stocks.OrderBy(s => s.Symbol);
                }
            }

            var SkipNumber = (query.PageNumber - 1) * query.PageSize;

            return await stocks
                .OrderBy(s => s.Id)
                .Skip(SkipNumber)
                .Take(query.PageSize)
                .ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context
                .Stock.Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Stock> UpdateStockAsync(
            int id,
            UpdateStockRequestDto updateStockRequestDto
        )
        {
            var stock = await _context.Stock.FirstOrDefaultAsync(x => x.Id == id);
            if (stock == null)
            {
                return null;
            }
            stock.Symbol = updateStockRequestDto.Symbol;
            stock.industry = updateStockRequestDto.industry;
            stock.purchase = updateStockRequestDto.purchase;
            stock.MarketCap = updateStockRequestDto.MarketCap;
            stock.CompanyName = updateStockRequestDto.CompanyName;
            stock.LastDiv = updateStockRequestDto.LastDiv;
            await _context.SaveChangesAsync();
            return stock;
        }

        public Task<bool> StockExists(int id)
        {
            return _context.Stock.AnyAsync(x => x.Id == id);
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return await _context.Stock.FirstOrDefaultAsync(s=>s.Symbol == symbol);
            
        }
    }
}
