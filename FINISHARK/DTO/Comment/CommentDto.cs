using System.ComponentModel.DataAnnotations;

namespace FINISHARK.DTO.Comment
{
    public class CommentDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500, MinimumLength = 5)]
        public string Content { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? StockId { get; set; }
    }
}
