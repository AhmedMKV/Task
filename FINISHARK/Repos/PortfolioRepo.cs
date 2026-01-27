using FINISHARK.Data;
using FINISHARK.Interfaces;
using FINISHARK.Models;
using Microsoft.EntityFrameworkCore;

namespace FINISHARK.Repos
{
    public class PortfolioRepo : IPortfolioRepo
    {
        private readonly ApplicationDbContext _context;
        public PortfolioRepo(ApplicationDbContext context)
        {
            _context = context;
            
        }

        public async Task<Portfolio> CreatePortfolio(Portfolio portfolio)
        {

             await _context.Portfolios.AddAsync(portfolio);
            await _context.SaveChangesAsync();
            return portfolio;
            
        }

        public async Task<Portfolio?> DeletePortfolio(AppUser user, string Symbol)
        {
            var portfolioModel = await _context.Portfolios.FirstOrDefaultAsync(s => s.AppUserId == user.Id && s.stock.Symbol.ToLower() == Symbol.ToLower());
            if (portfolioModel == null)
            {
                return null;

            }

            _context.Portfolios.Remove(portfolioModel);
            await _context.SaveChangesAsync();
            return portfolioModel;
        }

        public async Task<List<Stock>> GetUserPortfolio(AppUser user)
        {
            return await _context.Portfolios.Where(x => x.AppUserId == user.Id)
                 .Select(stock => new Stock
                 {
                     Id = stock.StockID,
                     Symbol = stock.stock.Symbol,
                     CompanyName = stock.stock.CompanyName,
                     industry = stock.stock.industry,
                     purchase = stock.stock.purchase,
                     LastDiv = stock.stock.LastDiv,
                     MarketCap = stock.stock.MarketCap,

                 }).ToListAsync();
        }
    }
}
