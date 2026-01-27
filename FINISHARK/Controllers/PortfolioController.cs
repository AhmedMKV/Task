using FINISHARK.Extensions;
using FINISHARK.Interfaces;
using FINISHARK.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FINISHARK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepo _stockRepo;
        private readonly IPortfolioRepo _portfolioRepo;
        public PortfolioController(UserManager<AppUser> userManager, IStockRepo stockRepo, IPortfolioRepo portfolioRepo)
        {
            _userManager = userManager;
            _stockRepo = stockRepo;
            _portfolioRepo = portfolioRepo;
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolios() {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            

            var userPortfolio = await _portfolioRepo.GetUserPortfolio(appUser);
            return Ok(userPortfolio);


            
            
        
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string Symbol)
        {
            var Username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(Username);
            var stock = await _stockRepo.GetBySymbolAsync(Symbol);
            if (stock == null) return BadRequest("Stock not found");
            var userPortfolio = await _portfolioRepo.GetUserPortfolio(user);
            if (userPortfolio.Any(e => e.Symbol.ToLower() == Symbol.ToLower())) return BadRequest("Cannot add same stock to portfolio");
            var portfolioModel = new Portfolio
            {

                StockID = stock.Id,
                AppUserId = user.Id,

            };
            var portfolio = await _portfolioRepo.CreatePortfolio(portfolioModel);
            if(portfolio == null)
            {
                return StatusCode(500, "Could not create");
            }
            else
            {
                return Created();
            }


        }

    [HttpDelete]
    [Authorize]

    public async Task<IActionResult> DeletePortfolio(string Symbol)
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            var userPortfolio = await _portfolioRepo.GetUserPortfolio(user);

            var filteredStock = userPortfolio.Where(s=>s.Symbol.ToLower() == Symbol.ToLower()).ToList();

            if (filteredStock.Count() == 1) {
                await _portfolioRepo.DeletePortfolio(user,Symbol);
            }
            else
            {
                return BadRequest("Stock is not in your portfolio");
            }
            return Ok();



        }
    }
}
