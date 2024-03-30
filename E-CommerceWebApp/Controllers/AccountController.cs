using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelClasses;
using ModelClasses.ViewModel;

namespace E_CommerceWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        // go to view and throw model to view 
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            ViewData["returnUrl"] = returnUrl;
            return View(loginViewModel);
        }

        // use action to login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login, string returnUrl = null)
        {
            // to check sign in result and return correct view
            if (ModelState.IsValid)
            {
                var signInResult = await _signInManager.PasswordSignInAsync
                    (login.Email, login.Password, isPersistent: false, lockoutOnFailure: false);

                if (signInResult.Succeeded)
                {
                    login.LoginStatus = "login successful. thank you";
                    if (returnUrl != null && Url.IsLocalUrl(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            login.LoginStatus = "unsuccessfully login wrong Email Or Password";

            return View(login);
        }

        // go ot view and model to view 
        [HttpGet]
        public IActionResult Register()
        {
            RegisterViewModel registerViewModel = new RegisterViewModel();
            return View(registerViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {

            // add user info from view
            var user = new ApplicationUser
            {
                FirstName = register.ApplicationUser.FirstName,
                LastName = register.ApplicationUser.LastName,
                Email = register.Email,
                UserName = register.UserName,
                Address = register.ApplicationUser.Address,
                City = register.ApplicationUser.City,
            };
            // check user info and password
            var registration = await _userManager.CreateAsync(user, register.Password);
            if (registration.Succeeded)
            {

                
                await _signInManager.SignInAsync(user, isPersistent: false);
                register.StatusMessage = "Register Successfully";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                register.StatusMessage = "unsuccessfully to register password must contain uppercase and lowercase letters, numbers and symbols";
            }
            return View(register);
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
