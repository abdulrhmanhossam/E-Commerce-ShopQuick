using DataBaseAccess;
using E_CommerceWebApp.Models;
using E_CommerceWebApp.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelClasses.ViewModel;
using System.Diagnostics;

namespace E_CommerceWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, 
               SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _dbContext = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Index(string? searchName, string? searchCategoryName)
        {
            var claim = _signInManager.IsSignedIn(User);
            if (claim)
            {
                var userId = _userManager.GetUserId(User);
                var count = _dbContext.UserCarts.Where(x => x.UserId.Contains(userId)).Count();
                HttpContext.Session.SetInt32(CartCount.sessionCount, count);
            }
            HomePageViewModel homePageViewModel = new HomePageViewModel();

            if (!string.IsNullOrEmpty(searchName))
            {
                // Trim leading and trailing spaces
                searchName = searchName.Trim();

                homePageViewModel.ProductList = _dbContext.Products
                    .Where(productName => EF.Functions
                    .Like(productName.ProductName.ToLower(),
                     $"%{searchName.ToLower()}%"))
                    .ToList();

                homePageViewModel.Categories = _dbContext.Categories.ToList();
            }
            else if(!string.IsNullOrEmpty(searchCategoryName))
            {
                var categoryName = _dbContext.Categories
                    .FirstOrDefault(x => x.CategoryName == searchCategoryName);
                
                homePageViewModel.ProductList = _dbContext.Products
                    .Where(x => x.CategoryId == categoryName!.Id).ToList();
                
                homePageViewModel.Categories = _dbContext.Categories
                    .Where(x => x.CategoryName.Contains(searchCategoryName));
                
            }
            else
            {
                homePageViewModel.ProductList = _dbContext.Products.ToList();
                homePageViewModel.Categories = _dbContext.Categories.ToList();
            }

            return View(homePageViewModel);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
