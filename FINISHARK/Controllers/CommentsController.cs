using FINISHARK.DTO.Comment;
using FINISHARK.Extensions;
using FINISHARK.Interfaces;
using FINISHARK.Mappers;
using FINISHARK.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FINISHARK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepo _commentRepo;
        private readonly IStockRepo _stockRepo;
        private readonly UserManager<AppUser> _userManager;

        public CommentsController(ICommentRepo commentRepo, IStockRepo stockRepo, UserManager<AppUser> userManager)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetComments()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var Comments = await _commentRepo.GetCommentsAsync();
            var CommentDto = Comments.Select(s => s.toCommentsDto());
            return Ok(CommentDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment.toCommentsDto());
        }

        [HttpPost("{stockId}")]
        public async Task<IActionResult> CreateComment(
            [FromRoute] int stockId,
            CreateCommentRequestDto createCommentRequestDto
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!await _stockRepo.StockExists(stockId))
            {
                return BadRequest("Stock Doesn't exist");
            }

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            var Comment = createCommentRequestDto.toCommentFromCreate(stockId);
            Comment.AppUserId = appUser.Id;
            await _commentRepo.CreateCommentAsync(Comment);
            return CreatedAtAction(
                nameof(GetCommentById),
                new { id = Comment.Id },
                Comment.toCommentsDto()
            );
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateComment(
            [FromRoute] int id,
            [FromBody] UpdateCommentDto updateCommentDto
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var Comment = updateCommentDto.toCommentFromUpdate();
            var CommentMOdel = await _commentRepo.UpdateCommentAsync(id, Comment);
            return Ok(CommentMOdel.toCommentsDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var comment = await _commentRepo.DeleteCommentAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment.toCommentsDto());
        }
    }
}
