using FINISHARK.DTO.Comment;
using FINISHARK.Models;

namespace FINISHARK.Mappers
{
    public static class CommentMapper
    {
        public static CommentDto toCommentsDto(this Comment comment)
        {
            return new CommentDto
            {
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                Id = comment.Id,
                StockId = comment.StockId,
                Title = comment.Title,
                CreatedBy = comment.User.UserName
            };
        }

        public static Comment toCommentFromCreate(
            this CreateCommentRequestDto createCommentRequestDto,
            int stockId
        )
        {
            return new Comment
            {
                Content = createCommentRequestDto.Content,
                Title = createCommentRequestDto.Title,
                StockId = stockId,
            };
        }

        public static Comment toCommentFromUpdate(this UpdateCommentDto updateCommentDto)
        {
            return new Comment
            {
                Content = updateCommentDto.Content,
                Title = updateCommentDto.Title,
            };
        }
    }
}
