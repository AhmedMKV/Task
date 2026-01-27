using FINISHARK.Models;

namespace FINISHARK.Interfaces
{
    public interface IPortfolioRepo
    {

        Task<List<Stock>> GetUserPortfolio(AppUser user);
        Task<Portfolio> CreatePortfolio(Portfolio portfolio);

        Task<Portfolio?> DeletePortfolio(AppUser user , string Symbol);
    }
}
