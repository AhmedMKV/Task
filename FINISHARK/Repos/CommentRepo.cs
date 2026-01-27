using FINISHARK.Data;
using FINISHARK.DTO.Comment;
using FINISHARK.Interfaces;
using FINISHARK.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FINISHARK.Repos
{
    public class CommentRepo : ICommentRepo
    {
        private readonly ApplicationDbContext _context;

        public CommentRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Comment>> GetCommentsAsync()
        {
            var Comments = await _context.Comments.Include(s=>s.User).ToListAsync();
            return Comments;
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments.Include(s => s.User).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment> UpdateCommentAsync(int id, Comment comment)
        {
            var Commentmodel = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
            if (Commentmodel == null)
            {
                return null;
            }
            Commentmodel.Title = comment.Title;
            Commentmodel.Content = comment.Content;
            await _context.SaveChangesAsync();
            return Commentmodel;
        }

        public async Task<Comment?> DeleteCommentAsync(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
            if (comment == null)
            {
                return null;
            }
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return comment;
        }
    }
}
