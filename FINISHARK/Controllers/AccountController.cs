using FINISHARK.DTO.Account;
using FINISHARK.Interfaces;
using FINISHARK.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FINISHARK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService,SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;

            
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(c => c.UserName == loginDto.Username.ToLower());
            if (user == null)
                return Unauthorized("invalid username");

            var result = await _signInManager.CheckPasswordSignInAsync(user,loginDto.Password,false);
            if (!result.Succeeded) {
                return Unauthorized("username or password was incorrect");
            }

            return Ok(new NewUserDto
            {
                Username = user.UserName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user)

            });


        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto) 
        {
            try
            {

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var user = new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email
                };

                var createUser = await _userManager.CreateAsync(user,registerDto.Password);
                if (createUser.Succeeded)
                {
                    var role = await _userManager.AddToRoleAsync(user,"User");
                    if (role.Succeeded) {

                        return Ok(new NewUserDto
                        {
                            Username = user.UserName,
                            Email = user.Email,
                            Token = _tokenService.CreateToken(user),
                        });

                    }
                    else
                    {

                        return BadRequest(role.Errors);
                    }
                }
                else
                {
                    return BadRequest(createUser.Errors);
                }
                

            }
            catch (Exception ex) {
                return BadRequest(ex);
            
            
            
            }



        }
    }
}
