using FINISHARK.DTO.Comment;
using FINISHARK.Models;

namespace FINISHARK.Interfaces
{
    public interface ICommentRepo
    {
        Task<List<Comment>> GetCommentsAsync();
        Task<Comment?> GetByIdAsync(int id);
        Task<Comment> CreateCommentAsync(Comment coment);
        Task<Comment> UpdateCommentAsync(int id, Comment comment);
        Task<Comment?> DeleteCommentAsync(int id);
    }
}
