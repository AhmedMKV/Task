using System.ComponentModel.DataAnnotations;
using FINISHARK.DTO.Comment;

namespace FINISHARK.DTO.Stock
{
    public class StockDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 1)]
        public string Symbol { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal purchase { get; set; }

        [Range(0, double.MaxValue)]
        public decimal LastDiv { get; set; }

        [Required]
        [StringLength(100)]
        public string industry { get; set; } = "";

        [Range(0, long.MaxValue)]
        public long MarketCap { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}
