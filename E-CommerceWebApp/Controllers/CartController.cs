using DataBaseAccess;
using E_CommerceWebApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using ModelClasses;
using ModelClasses.ViewModel;

namespace E_CommerceWebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public CartController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;   
        }
        public IActionResult CartIndex()
        {
            var claim = _signInManager.IsSignedIn(User);
            if (claim)
            {
                var userId = _userManager.GetUserId(User);
                CartIndexViewModel cartIndexViewModel = new CartIndexViewModel()
                {
                    ProductList = _dbContext.UserCarts.Include(x => x.Product)
                        .Where(x => x.UserId.Contains(userId!)).ToList(),
                };
                var count = _dbContext.UserCarts.Where(x => x.UserId.Contains(userId)).Count();
                HttpContext.Session.SetInt32(CartCount.sessionCount, count);
                return View(cartIndexViewModel);
            }
            return View();
        }

        [Authorize]
        public async Task<IActionResult> AddToCart(int productId, string? returnUrl)
        {
            var productAddToCart = await _dbContext.Products
                .FirstOrDefaultAsync(x => x.Id == productId);
            
            var checkIfUserSignedInOrNot = _signInManager.IsSignedIn(User);
            if (checkIfUserSignedInOrNot)
            {
                var user = _userManager.GetUserId(User);
                if (user != null)
                {
                    // check if the signed in user has any cart
                    var getTheCartIfAnyExistForTheUser = await _dbContext.UserCarts
                        .Where(x => x.UserId.Contains(user)).ToListAsync();
                    if (getTheCartIfAnyExistForTheUser.Count() > 0)
                    {
                        // check if the item is already in the cart or not
                        var getTheQuantity = getTheCartIfAnyExistForTheUser
                            .FirstOrDefault(x => x.ProductId == productId);
                        if (getTheQuantity != null)
                        {
                            // if the item is already in the cart just increase the quantity by 1 and update the cart
                            getTheQuantity.Quantity = getTheQuantity.Quantity + 1;
                            _dbContext.UserCarts.Update(getTheQuantity);
                        }
                        else
                        {
                            // user has a cart but adding a new item to the existing cart
                            UserCart newItemToCart = new UserCart 
                            {
                                ProductId = productId,
                                UserId = user,
                                Quantity = 1,
                            };
                            await _dbContext.UserCarts.AddAsync(newItemToCart);
                        }
                    }
                    else
                    {
                        // user has no carts. adding a brand new cart for user
                        UserCart newItemToCart = new UserCart 
                        {
                            ProductId = productId,
                            UserId = user,
                            Quantity = 1,
                        };
                        await _dbContext.UserCarts.AddAsync(newItemToCart); 
                    }
                    await _dbContext.SaveChangesAsync();
                }

            }

            if (returnUrl != null)
            {
                return RedirectToAction("CartIndex", "Cart");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        public IActionResult MinusItem(int productId)
        {
           // get the item which we need to minus a quantity
            var itemToMinus = _dbContext.UserCarts
                .FirstOrDefault(x => x.ProductId == productId);
            if (itemToMinus != null)
            {
                if (itemToMinus.Quantity - 1 == 0)
                {
                    _dbContext.UserCarts.Remove(itemToMinus);
                }
                else
                {
                    itemToMinus.Quantity -= 1;
                    _dbContext.UserCarts.Update(itemToMinus);
                }
                _dbContext.SaveChanges();
            }
            return RedirectToAction(nameof(CartIndex));
        }
        public IActionResult DeleteItemCart(int productId)
        {
           // get the item which we need to remove 
            var itemToRemove = _dbContext.UserCarts
                .FirstOrDefault(x => x.ProductId == productId);
            if (itemToRemove != null)
            {
                _dbContext.UserCarts.Remove(itemToRemove);
                _dbContext.SaveChanges();
            }
            return RedirectToAction(nameof(CartIndex));
        }
    }
}