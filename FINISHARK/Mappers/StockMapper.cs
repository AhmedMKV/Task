using FINISHARK.DTO.Stock;
using FINISHARK.Models;

namespace FINISHARK.Mappers
{
    public static class StockMapper
    {
        public static StockDto toStockDto(this Stock StockModel)
        {
            return new StockDto
            {
                Id = StockModel.Id,
                CompanyName = StockModel.CompanyName,
                Symbol = StockModel.Symbol,
                industry = StockModel.industry,
                LastDiv = StockModel.LastDiv,
                MarketCap = StockModel.MarketCap,
                purchase = StockModel.purchase,
                Comments = StockModel.Comments.Select(s => s.toCommentsDto()).ToList(),
            };
        }

        public static Stock toCreateStockDto(this CreateStockRequestDto createStockRequestDto)
        {
            return new Stock
            {
                Symbol = createStockRequestDto.Symbol,
                CompanyName = createStockRequestDto.CompanyName,
                industry = createStockRequestDto.industry,
                LastDiv = createStockRequestDto.LastDiv,
                MarketCap = createStockRequestDto.MarketCap,
                purchase = createStockRequestDto.purchase,
            };
        }
    }
}
