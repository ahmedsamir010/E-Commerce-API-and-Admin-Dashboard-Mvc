using AdminDashboard.Helpers;
using AdminDashboard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.Core.Entities.Identity;
using System.Threading.Tasks;

namespace AdminDashboard.Controllers
{

    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AdminController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [RequireAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous] // Make sure to allow anonymous access to the login action
        [ValidateAntiForgeryToken] // Protect against cross-site request forgery (CSRF) attacks
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }
                
                var user = await _userManager.FindByEmailAsync(loginViewModel.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View(loginViewModel);
                }

                var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, true, false);
                if (!result.Succeeded || !await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    ModelState.AddModelError(string.Empty, "You are not authorized.");
                    return View(loginViewModel);
                }

            await _userManager.UpdateSecurityStampAsync(user);
           return RedirectToAction("Index", "Home"); // Redirect to the desired dashboard page upon successful login
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login"); // Redirect to the login page after logout
        }
    }
}
