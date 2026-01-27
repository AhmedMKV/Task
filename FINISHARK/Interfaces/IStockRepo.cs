using FINISHARK.DTO.Stock;
using FINISHARK.Helpers;
using FINISHARK.Models;

namespace FINISHARK.Interfaces
{
    public interface IStockRepo
    {
        Task<List<Stock>> GetAllStockAsync(QueryObject query);
        Task<Stock?> GetByIdAsync(int id);

        Task<Stock?> GetBySymbolAsync(string symbol);
        Task<Stock> CreateStockAsync(Stock stock);

        Task<Stock> UpdateStockAsync(int id, UpdateStockRequestDto updateStockRequestDto);

        Task<Stock?> DeleteStockAsync(int id);
        Task<bool> StockExists(int id);
    }
}
