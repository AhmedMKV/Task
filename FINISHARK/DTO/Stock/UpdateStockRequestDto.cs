namespace FINISHARK.DTO.Stock
{
    public class UpdateStockRequestDto
    {
        public string Symbol { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public decimal purchase { get; set; }

        public decimal LastDiv { get; set; }

        public string industry { get; set; } = "";

        public long MarketCap { get; set; }
    }
}
