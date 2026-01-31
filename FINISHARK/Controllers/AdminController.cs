using FINISHARK.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FINISHARK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("check-admin")]
        public async Task<IActionResult> CheckAdminStatus()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Unauthorized();
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            return Ok(new { IsAdmin = isAdmin });
        }
    }
}
